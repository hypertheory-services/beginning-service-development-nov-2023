# Using a ConfigMap

Narrative: For our OnCallApi, the data in the `schedule.json` is *baked into* each of the running replicas. We would like to share this data across replicas.

We *could* use a Volume (a PVC), but since the list is so small, we will just provide it with a ConfigMap for right now.

This is also useful from an educational perspective because we can use the same technique in it's more typical application - providing runtime configuration.

The good new is we don't have to change anything about our running images.

If you make a request for the schedule, and look at the logs for the pod, you will see in the logged information that it is reading that data from `/app/data/schedule.json`. We will create a ConfigMap with that data, and then mount it to the containers at that location.

In the `deployments` folder of your OnCallApi, create a file called `schedule-config-map.YAML`

It should look like this:

> Note: I changed the schedule a bit. Queenie, the boss, is going to cover Monday this week, since Bob has a "thing" (Bob often has a "thing" and it is starting to get on both Sue and Queenie's nerves a bit). The vertical bar (|) after the property name "schedule" indicates that what follows is an arbitrary string.

```YAML
apiVersion: v1
kind: ConfigMap
metadata:
  name: schedule-config-map
  namespace: hypertheory
data:
  schedule.json: |
    {
      "sunday": {
        "name": "Help Desk",
        "email": "help@company.com",
        "phone": "(800) 233-1212"
      },
      "monday": {
        "name": "Queenie McMillian",
        "email": "ceo@company.com",
        "phone": "(555) 555-1210"
      },
      "tuesday": {
        "name": "Sue Jones",
        "email": "sue@company.com",
        "phone": "(555) 555-1212"
      },
      "wednesday": {
        "name": "Sue Jones",
        "email": "sue@company.com",
        "phone": "(555) 555-1212"
      },
      "thursday": {
        "name": "Bob Smith",
        "email": "bob@company.com",
        "phone": "(555) 555-1211"
      },
      "friday": {
        "name": "Bob Smith",
        "email": "bob@company.com",
        "phone": "(555) 555-1211"
      },
      "saturday": {
        "name": "Help Desk",
        "email": "help@company.com",
        "phone":  "(800) 233-1212"
      }
    }
```

Use `kubectl` to apply this to our cluster.

```
kubectl apply -f .\deployments\schedule-config-map.yaml
```

Like most things, you can "describe" or "get" it.

```
kubectl describe configmap schedule-config-map
```

## Update the Deployment

We will configure our deployment for the oncallapi to use this configmap and mount it as a volume in the appropriate location in our container.

Here is the file:

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: oncall-api
  namespace: hypertheory
spec:
  replicas: 1
  selector:
    matchLabels:
      app: oncall-api
  template:
    metadata:
      labels:
        app: oncall-api
    spec:
      containers:
      - name: oncall-api
        image: oncallapi:v0.1
        volumeMounts:
          - mountPath: /app/data/
            name: schedule-config
        resources:
          limits:
            memory: "128Mi"
            cpu: "500m"
        ports:
        - containerPort: 80
      volumes:
        - name: schedule-config
          configMap:
            name: schedule-config-map

```

And here is a diff of the additions:
```diff
apiVersion: apps/v1
kind: Deployment
metadata:
  name: oncall-api
  namespace: hypertheory
spec:
  replicas: 1
  selector:
    matchLabels:
      app: oncall-api
  template:
    metadata:
      labels:
        app: oncall-api
    spec:
      containers:
      - name: oncall-api
        image: oncallapi:v0.1
+        volumeMounts:
+          - mountPath: /app/data/
+            name: schedule-config
        resources:
          limits:
            memory: "128Mi"
            cpu: "500m"
        ports:
        - containerPort: 80
+     volumes:
+        - name: schedule-config
+          configMap:
+            name: schedule-config-map


```

## Changing the Configmap

If you make a change to the data in the configmap and apply it, that will be available to each of the services.

This is how it is *supposed* to work. At the time of this writing, .NET Core has an issue with this. Because the configmap is mounted to our container as a `symlink`, .NET does not (yet) know how to notice a change in a file through a symlink. Currently, unfortunately, that means you have to restart the pod for it to read any changed data.

There is no `kubectl restart pod...` command.

Two techniques:

- During Development: You can just change the number of replicas to 0, apply the file, then set it back to 1 and re-apply.
    - This isn't suitable for production, obviously, as the service would just 'disappear' for a moment or two until you restart it.
- Do a "rolling" deploy
    - This will start up a new replica set, direct traffic to it, and then remove the old one.
    - The command for this: `kubectl rollout restart deployment oncall-api`
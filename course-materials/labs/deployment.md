# Creating And Applying a Kubernetes Deployment

In this lab we will create a Kubernetes deployment. 

## Narrative

We have several services that we will eventually deploy that need to be able to look up who the the developer is "On Call" for a specific day of the week.

We've been *talking* for days about how to get some requirements for this, but in the end we realized they don't really have a policy for it. There is a list on a bulletin board that Carl (the IT Manager) keeps, and he says it hasn't changed for months. They really WANT to have this be a more automated process, have the ability to edit it, all that. But they'll get back to us. They are working out the details with HR and all that. You know. "Compliance", "DEI", all that jazz. We'll just have to wait.

But we can't wait. This is a bottle neck for us. There are several services we are developing that need this information.

In the spirit of *moving quick* and *Hypothesis Driven Development* we have designed the interface for the service that will provide this information. It will expose an HTTP interface, and services can make a `GET` request to the service, and it will return some data like this:

```json
{
  "sunday": {
    "name": "Help Desk",
    "email": "help@company.com",
    "phone": "(800) 233-1212"
  },
  "monday": {
    "name": "Sue Jones",
    "email": "sue@company.com",
    "phone": "(555) 555-1212"
  },
  "tuesday": {
    "name": "Bob Smith",
    "email": "bob@company.com",
    "phone": "(555) 555-1211"
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
    "name": "Sue Jones",
    "email": "sue@company.com",
    "phone": "(555) 555-1212"
  },
  "saturday": {
    "name": "Help Desk",
    "email": "help@company.com",
    "phone":  "(800) 233-1212"
  }
}
```

As a matter of fact, *FOR NOW* that is *exactly* the data it will return. We simply took a camera snapshot of Karl's whiteboard, but some JSON structure around it, etc. A single JSON object map of enumerated days of the week and some information about who is on call. (currently, we outsource the weekends to a company), Sue and Bob swap days.

We *briefly* considered making this more elaborate, by adding path variables where you could request things like:

`GET /today`

Or

`GET /friday`

But we put that on the backlog. We are good with this for right now, it will unblock us and we will continue.

## (Fictional) Developer Narrative

"Well, we'll need a Database. Let's set up a meeting the the DBA"

**RESPONSE**: "Really though?"

What is the simplest thing that will work so we can continue?

Deciding to just return a hardcoded response from the API felt a *little* too barbaric, so we returned a hardcoded json file. It is contained in the `data/schedule.json` file.

Our API has just one resource, you can do a `GET` request to `/`.  

## The Code

Take a look at the code in the `backend/OnCallApi` folder. You can open it with Visual Studio, or just browse it here. This is a .NET Core 6 Minimal API.  No Controllers, nothing. 

The only added *classroom* code here is really the line in the function that handles the get in the `Program.cs` file:

```c#
 app.Logger.LogInformation($"Reading data from {file}");
```

> Spoiler: This will become important to us later.

## Running the Code

If you navigate your terminal to the folder containing the application and type: `dotnet run` you should see the application start up. It is running at `http://localhost:5080`. Open a browser, or Postman, or whatever, and make a `GET` request to it. You should see our data.

In the logged output you should see the big long path of the file for the schedule.json.

That's it. That's the service. About as "micro" as you can get, but still useful. We love it.

Let's build a container for it.

## Building a Container

Kubernetes only agrees to run code if it is in an OCI-compliant container. We will build a container for this. For this demo, I just used the "canned" Visual Studio `Dockerfile` for .NET APIs and such. It's a multi-stage build, not bad. You can change it around to meet your requirements and stuff (different base images, whatever). This is good enough for us. Don't even need to edit it for this one.

One thing to notice in the `Dockerfile` is the instruction (about line 5) to `EXPOSE 80`. When we were running it in our dev machine outside of a container, .NET just sort of picked a random unused TCP port. Here this is going to *assume* it is running on port 80.

Shut down the running API.

Normally we could just run `docker build...` and all that, but we shut down docker. 

There are several (8!) ways to do this, but what we are going to do is tell our local installation of the `docker` command line tool to do it's stuff using the container stuff in our Minikube cluster. 

> Note: All the ways to do this are documented here: https://minikube.sigs.k8s.io/docs/handbook/pushing/

In our command prompt we are going to run the command, using Powershell: 

```powershell
& minikube -p minikube docker-env --shell powershell | Invoke-Expression
```

Once that is done, when you run `docker` commands, they will be run in the minikube environment.

For example, running `docker ps` will show you all the stuff Minikube is doing (Minikube runs Kubernetes in Docker, which runs your container. *mind blown*).

```
CONTAINER ID   IMAGE                  COMMAND                  CREATED       STATUS       PORTS     NAMES
b39f4de8e0b9   6e38f40d628d           "/storage-provisioner"   2 hours ago   Up 2 hours             k8s_storage-provisioner_storage-provisioner_kube-system_b28671ae-f5f7-4046-b622-4b39a77785dc_0
d32be360ce48   8d147537fb7d           "/coredns -conf /etc…"   2 hours ago   Up 2 hours             k8s_coredns_coredns-78fcd69978-g56xh_kube-system_f3cfbd9b-a196-4ebc-9ba6-b54bfc42ae2b_0
fddf9a71da0d   k8s.gcr.io/pause:3.5   "/pause"                 2 hours ago   Up 2 hours             k8s_POD_storage-provisioner_kube-system_b28671ae-f5f7-4046-b622-4b39a77785dc_0
0fe54261bcd5   6120bd723dce           "/usr/local/bin/kube…"   2 hours ago   Up 2 hours             k8s_kube-proxy_kube-proxy-w84rt_kube-system_74beabd7-71c1-4abe-8a21-ae740ec93cc6_0
77539fcf16b6   k8s.gcr.io/pause:3.5   "/pause"                 2 hours ago   Up 2 hours             k8s_POD_coredns-78fcd69978-g56xh_kube-system_f3cfbd9b-a196-4ebc-9ba6-b54bfc42ae2b_0
ffb84e146332   k8s.gcr.io/pause:3.5   "/pause"                 2 hours ago   Up 2 hours             k8s_POD_kube-proxy-w84rt_kube-system_74beabd7-71c1-4abe-8a21-ae740ec93cc6_0
1b91fa11885d   53224b502ea4           "kube-apiserver --ad…"   2 hours ago   Up 2 hours             k8s_kube-apiserver_kube-apiserver-minikube_kube-system_cb093049061ab7bf7cf2a3c49f13adf6_0
3e368d89cfe1   05c905cef780           "kube-controller-man…"   2 hours ago   Up 2 hours             k8s_kube-controller-manager_kube-controller-manager-minikube_kube-system_cf61f8185359bbfecb994d4d92683b56_0
74a7d1f975fc   004811815584           "etcd --advertise-cl…"   2 hours ago   Up 2 hours             k8s_etcd_etcd-minikube_kube-system_119ad617087e3218bcd1c8c5c8c589b2_0
ef0fe00f00e1   0aa9c7e31d30           "kube-scheduler --au…"   2 hours ago   Up 2 hours             k8s_kube-scheduler_kube-scheduler-minikube_kube-system_eee9e2da42102bf0a05e1e7b00e318bf_0
6fc0d477ed95   k8s.gcr.io/pause:3.5   "/pause"                 2 hours ago   Up 2 hours             k8s_POD_kube-scheduler-minikube_kube-system_eee9e2da42102bf0a05e1e7b00e318bf_0
9bd5cf3b0657   k8s.gcr.io/pause:3.5   "/pause"                 2 hours ago   Up 2 hours             k8s_POD_kube-controller-manager-minikube_kube-system_cf61f8185359bbfecb994d4d92683b56_0
495b6035107d   k8s.gcr.io/pause:3.5   "/pause"                 2 hours ago   Up 2 hours             k8s_POD_kube-apiserver-minikube_kube-system_cb093049061ab7bf7cf2a3c49f13adf6_0
45c01ea42443   k8s.gcr.io/pause:3.5   "/pause"                 2 hours ago   Up 2 hours             k8s_POD_etcd-minikube_kube-system_119ad617087e3218bcd1c8c5c8c589b2_0         
```

So now when we run `docker build` it will use Minikube to build the file, and it will add it to the registry that is available to Minikube for our deployment.

> NOTE: **BIG FAT IMPORTANT NOTE**: That command to set your docker environment is only for *that* terminal while you have it open. If you close it, you'll have to do it again!

Run this to build your container image:

```powershell
docker build -t oncallapi:v0.1 .
```

When it is done, you are good to go. You can verify with `docker images`:
```
REPOSITORY                                TAG       IMAGE ID       CREATED          SIZE
oncallapi                                 v0.1      cfd6ccfc8fac   46 seconds ago   208MB
mcr.microsoft.com/dotnet/sdk              6.0       dc243ad423c7   3 days ago       716MB
mcr.microsoft.com/dotnet/aspnet           6.0       53451db35067   3 days ago       208MB
... etc. etc.
```

## Creating the Deployment

We will now begin to build our deployment YAML file. In the code base with your OnCallApi, create a new directory in the root of the project called `deployments`. Remember, according to the Twelve Factor App, we keep all this in the source code repository.

We will start with create a Deployment. Deployments do double-duty. They create the deployment that Kubernetes will use to monitor our application, and the Pod for our deployment to run.

This deployment will be very simple for now. 

Create a file in the `deployments` directory called `deployment.yaml`.

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: oncall-api
  namespace: hypertheory
spec:
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
        resources:
          limits:
            memory: "128Mi"
            cpu: "500m"
        ports:
        - containerPort: 80
```
This is about as basic as they come. Some key pieces:

- `apiVersion: apps/v1` - this tells our cluster API how to process this.
- `kind: Deployment` - kind of obvious, but there are other 'kinds', as we will see.
- `metadata:`
    - `name: oncall-api`: Must be unique in your namespace. 
    - `namespace: hypertheory`: Not strictly required, but I'm a firm believer in adding this.
- `template:` - This is information for the pods that are created. For example they will each have the label `app: oncall-api`.
    - `spec:` This lists out the containers that will be a part of this Pod. The Pod will only have one container.
        - `containers:`An array of the containers.
        - `-name: oncall-api`: The name of the first container
            - `image: oncallapi:v0.1`: The image to use. This must be available to the cluster.
            - `resources:` An object with `limits` and `requests`. Limits are the max this should be allowed to get to, requests (left out here), are our way of saying 'don't schedule this on a node without this much memory and CPU available'. [Read More About Request and Limits](https://kubernetes.io/docs/concepts/configuration/manage-resources-containers/)
        - `ports:` A list of ports exposed by the container. Can be more than one.
            - `containerPort: 80` - remember our container `EXPOSE 80`?

That should be enough to get that going. Let's apply this to our desired state for the cluster and let it do it's thing to make us proud.

From a terminal window, use the following command to apply this deployment. The `-f` flag gives the path to the deployment, so yours may change depending on where you are running it. In this example I'm running it from the root folder of the project, where the `deployments` directory lives.

```shell
kubectl apply -f .\deployments\deployment.yaml
```

If there were no errors *validating* the deployment, you should get a `deployment.apps/oncall-api created  ` message. Keep in mind that doesn't mean it is 'done', it just means the 'work order' (so to speak) looks acceptable.

You can check the progress by doing something like: 

```
kubectl get deployments
```
Which will produce something like:

```
NAME         READY   UP-TO-DATE   AVAILABLE   AGE
oncall-api   1/1     1            1           6s    
```

You can also see the pod it created by doing:

```
kubectl get pods
```
which will return something like:
```
NAME                         READY   STATUS    RESTARTS   AGE
oncall-api-74ddff749-l89s5   1/1     Running   0          11s
```

Notice the name on the pod has some random characters on the end? Deployments don't. 
If we create multiple `replicas` of our pods, each one will be given a unique name.


### Describing (getting information about) Objects

You can use the `kubectl describe` command to get an inside view of the way Kubernetes 'sees' an object. This can be useful for debugging issues as well.

```
kubectl describe deployment oncall-api
```

Will return something like this:
```
Name:                   oncall-api
Namespace:              hypertheory
CreationTimestamp:      Sat, 29 Jan 2022 14:48:23 -0500
Labels:                 <none>
Annotations:            deployment.kubernetes.io/revision: 1
Selector:               app=oncall-api
Replicas:               1 desired | 1 updated | 1 total | 1 available | 0 unavailable
StrategyType:           RollingUpdate
MinReadySeconds:        0
RollingUpdateStrategy:  25% max unavailable, 25% max surge
Pod Template:
  Labels:  app=oncall-api
  Containers:
   oncall-api:
    Image:      oncallapi:v0.1
    Port:       80/TCP
    Host Port:  0/TCP
    Limits:
      cpu:        500m
      memory:     128Mi
    Environment:  <none>
    Mounts:       <none>
  Volumes:        <none>
Conditions:
  Type           Status  Reason
  ----           ------  ------
  Available      True    MinimumReplicasAvailable
  Progressing    True    NewReplicaSetAvailable
OldReplicaSets:  <none>
NewReplicaSet:   oncall-api-74ddff749 (1/1 replicas created)
Events:
  Type    Reason             Age   From                   Message
  ----    ------             ----  ----                   -------
  Normal  ScalingReplicaSet  20m   deployment-controller  Scaled up replica set oncall-api-74ddff749 to 1    
```

The Events section at the bottom can be really helpful in troubleshooting issues.

You can `kubectl describe` about anything (namespaces, pods, services, ingress, etc.).



### Viewing Logs

If you want to see the logs from a pod, you can use:

```
kubectl logs oncall-api-74ddff749-l89s5
```

> Note: Make sure you use *your* pod name. It *will* be different than mine.

## Let's Scale our Pods!

We can have multiple instances of our Pods running (for HA, or load balancing). This can be done through `autoscaling` (based on resource usage, etc.), or you can just sort of say "I want 20 of these things running", or whatever. We won't go *that* nuts because we don't have a ton of resources here. 

In the `spec` portion of your Deployment, add a `replicas` property and set it to 3.

```diff
spec:
+   replicas: 3
    selector:    
```

Tell the Kubernetes API that *this* is now your desired state.

```
kubectl apply -f .\deployments\deployment.yaml
```

> I get a kick out of this. I feel like Darth Vader saying "I am altering the deal. Pray I don't alter it further." 

Now ask it about the pods with `kubectl get pods`. You should see something like:

```
NAME                         READY   STATUS    RESTARTS   AGE
oncall-api-74ddff749-cvg5l   1/1     Running   0          5s
oncall-api-74ddff749-ddm4x   1/1     Running   0          5s
oncall-api-74ddff749-l89s5   1/1     Running   0          33m    
```

Change it to 2 replicas and apply it again. Then get them pods:

```
NAME                         READY   STATUS    RESTARTS   AGE
oncall-api-74ddff749-cvg5l   1/1     Running   0          2m
oncall-api-74ddff749-l89s5   1/1     Running   0          35m   
```

We need to talk about networking (and we will! next!), but how can we see if these things are *really* working? These Pods aren't actually exposed on the network. They are part of the cluster's network. And even if they are in the cluster, and other pods need to talk to them, which *replica* will it talk to? All that and more coming up. But we can do a quick *developer* test to make sure it is up and running by doing a `port forward`.

## Port Forwarding

You can *temporarily* make it look like a Pod is available on your local network by using a facility of kubectl called "port forwarding". You tell it to start up a little server (basically) that is running on your local machine at a specific TCP Port, and then just forward any requests to that port to the port exposed on your pod.

So, pick a pod from the results of `kubectl get pods`, and then run the following command:

```
kubectl port-forward pods/oncall-api-74ddff749-cvg5l 1337:80 
```

This says we want to `port-forward` the pod `oncall-api-74ddfff749-cvgl` on only local machines TCP port 1337 to port 80 on that pod (that's the container port, remember?).

When you run this it will take over your terminal and keep running until you kill it (Ctrl+C real hard should do it).

But first, open a browser or something and hit `http://localhost:1337` and you should see our oncall data.



## Finishing Up

Let's reset our replicas to 1 and reapply it. Your final `deployment.yaml` should look like this:

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
        resources:
          limits:
            memory: "128Mi"
            cpu: "500m"
        ports:
        - containerPort: 80
```

> Note: the default for replicas is one, so you could just delete that line.
# Claiming Storage and Deploying SQL Server

## First Big Note To Be Totally Redundant And Stuff

What we are doing here is *fine*, but really a thing like this should be set up by the pros and seen as an *attached* resource to our application.

In other words, deploying something like SQL Server, Redis, Etc. is it's own "App", and should have it's own code base and all that stuff.

And we've talked about using clusters, replication, and StatefulSets...

So with that warning:

## Claim Some Storage

Remember, your cluster administrator will configure actual, physical storage for you. They will have types of storage you can use (and, of course, you can request others). You can see the storage classes available on your cluster by just asking:

```
kubectl get storageclass
```

With Minikube, you'll see a pretty boring output:

```
NAME                 PROVISIONER                RECLAIMPOLICY   VOLUMEBINDINGMODE   ALLOWVOLUMEEXPANSION   AGE
standard (default)   k8s.io/minikube-hostpath   Delete          Immediate           false                  23h    
```

Notice a couple of things:

- the name "standard", is marked as (default). If we don't specify a specific storage class (we won't) this is what we'll use.
- the "provisioner" is "k8s.io/minikube-hostpath" - this means it will store it somewhere in our Minikube VM. We delete the Minikube cluster, this sucker is *gone*.

All of that is ok for us, and actually, may be ok in most instances. In other words, we are just going to make a claim on some of the default storage that is available, and let the cluster administrator worry about where that physically is.

Create a directory for your SQL Deployments.

In that directory, create a `pvc.yaml` file. This will be our Persistent Volume Claim.

```yaml
apiVersion: v1
kind: PersistentVolumeClaim
metadata:
  name: sql-pvc
  namespace: hypertheory
spec:
  resources:
    requests:
      storage: 1Gi
  accessModes:
    - ReadWriteOnce
  # This means it can be mounted as read/write by a single node. Careful - multiple pods on the same node can read and write to it.
  # we are using the default storage class and volumeMode here.

```

Use `kubectl` to apply this to the cluster. We are going to lay claim to 1 Gigabyte of the storage. Should be plenty. Notice the comments.

## Create a Deployment that uses that Storage

We will create a deployment for our Sql Server. Microsoft provides an image, and we'll use that. That image requires some environment variables to be set in order for it to start up. These include the version of SQL Server we have a license for (we'll use SQL Express), an administrator (SA) password, etc. You certainly wouldn't want your applications running as SA on a SQL Database, but I feel doing all that correctly would turn this into a SQL Server class. You get it. I hope.

Create a `deployment.yaml` file in your directory that looks like this:

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: mssql
spec:
  selector:
    matchLabels:
      app: mssql
  template:
    metadata:
      labels:
        app: mssql
    spec:
      containers:
        - name: mssql
          image: mcr.microsoft.com/mssql/server:2019-latest
          resources:
            limits:
              memory: "2Gi" # REALLY important to limit Sql Server. It will, by default, take 80% of available memory at startup.
              cpu: "500m"
          ports:
            - containerPort: 1433
          env:
            - name: MSSQL_PID
              value: "Express" # You must select the appropriate product ID for the license you have.
            - name: ACCEPT_EULA
              value: "Y" # You accept the end-user license agreement
            - name: SA_PASSWORD
              value: "TokyoJoe138!" # This should be in a secret, but since this is just for local development....
          volumeMounts:
            - mountPath: /var/opt/mssql
              name: mssqldb
      volumes:
        - name: mssqldb
          persistentVolumeClaim:
            claimName: sql-pvc
```

There are some interesting notes in the comments here. Make sure you read them. Notice in particular the `volumeMounts` on the container. How did I know to create a mount to `/var/opt/mssql` and what goes there? I looked it up. That is where SQL Server will store databases and stuff - in other words, the stuff that needs to persist across running instances of SQL Server.

The `volumes` entry at the bottom is where we reference our claim. The name of this is used above in the `volumeMounts` entry on the container.

Apply this deployment and let it spin up. 

> NOTE: We didn't build *this* container image, so Minikube will download it from it's configured registries. It assumes Docker Hub by default, but you can modify this or add to it using https://minikube.sigs.k8s.io/docs/handbook/registry/

## Create a Service

We will expose this on our cluster with a service. Again, we use a service to create a "stable" name for the pod so that our code can refer to it and use it and stuff. However, since this is only ever going to be a single replica scenario (again, probably tired of hearing this, but scaling something like SQL Server by just incrementing `replicas` in your deployment is going to hurt real bad. ), we can just have this service expose the IP address of whatever Pod is actually running out container. There is no need for the indirection game. This is what is known as a "Headless" service.

Create a `service.yaml` in your deployments directory and make it look like this:

```yaml
apiVersion: v1
kind: Service
metadata:
  name: mssql-service
spec:
  clusterIP: None # "Headless - just gives the IP of the Pod running Sql Server"
  selector:
    app: mssql
  ports:
    - port: 1433
      targetPort: 1433
```

The tiny little thing that makes this a "headless" service is annotated in the comments.

Deploy that bad boy.


# Creating a Service Object for our OnCallApi

In this lab we will create a service object for our OnCallApi.

In the `deployments` folder for your OnCallApi, create a file called `service.yaml`.

```yaml
apiVersion: v1
kind: Service
metadata:
  name: oncall-api-service
spec:
  selector:
    app: oncall-api
  ports:
  - port: 80

```

Deploy the service. 

My naming convention is to name the service the same as the pod plus `-service` suffix.

The `spec:selector:app:` field is how this service will *know* it is to be the service for that specific deployment.

If you run `kubectl get services` you will see something like:

```
NAME                 TYPE        CLUSTER-IP      EXTERNAL-IP   PORT(S)   AGE
oncall-api-service   ClusterIP   10.109.196.56   <none>        80/TCP    3m28s
```

To see the Endpoint that was created, you can run `kubectl get endpoints`:
```
NAME                 ENDPOINTS       AGE
oncall-api-service   172.17.0.3:80   3m51s
```

> Note: Your IP Addresses will vary.

The `TYPE` in the services is interesting. There are three possible types (and ClusterIP is the default, since we didn't specify).

- **ClusterIp**:
    - This means this service is available on an *internal* IP inside the cluster. Nothing outside the cluster can access it.
    - The default, and most restrictive.
    - Can be set to 'none' to just return the IP address of the Pod directly. (somewhat advanced scenario)
- **NodePort**:
    - This exposes the service to outside clients (external to the cluster). 
    - A port is assigned and mapped to the Pod.
    - Add the nodePort value to the ports with the value (30000 - 32767).
    - Not very efficient and not very secure. Consider ingress for this.
- **LoadBalancer**:
    - Also exposes the services to external clients.
    - Clusters must provide their own load balancing mechanism.
    - Useful if you need more sophisticated load balancing than just 'random'.
    - An extension of the NodePort Type.

> Note: The TYPE doesn't really have anything to do with load balancing- each of them can do load balancing across multiple replicas. The LoadBalancer type allows cloud providers and various distributions of Kubernetes to provide a custom load balancer (and cloud providers usually charge more for this), with more sophisticated load balancing algorithms

If you do a port forward on the service to access you container, it should work dandy!




# Kubernetes

> **SPECIAL NOTE** This is designed to give application developers an overall sense of what it is like to write Cloud Native code. It is *not* intended to be a thorough introduction to *anything*. Where your company may have policies on things, even important things, (like security), I have left those things out. The idea here is inspired by how I learned to drive a car. My mother took me on my first outing as the driver in a large, nearly empty (of living people and cars) cemetary. "If you hit anyone, chances are they are already dead. Don't worry about it!" I can still hear her saying. This is Kubernetes in a cemetary. It is a great way to *learn*, but once you hit the road (or the super-highway), there is a lot of other stuff you have to be careful about. Those things are no less important than what I am showing here, but it's hard to deal with those if you haven't taken a few turns in the cemetary first to build your confidence. You don't want your *first* left turn to be in a multi-lane road.
> Some of these topics will be covered in more detail in later classes.


This document serves as a general overview of our application orchestration platform, [Kubernetes](https://kubernetes.io).

Kubernetes is available as Open Source software using the Apache 2.0 License at https://github.com/kubernetes/kubernetes. It was originally designed by Google, but now the [Cloud Native Computing Foundation](https://www.cncf.io/projects/kubernetes/) now maintains it. It is a 'Graduated' level project on the CNCF. 

Kubernetes is written in the [Go](https://go.dev) programming language, also a product of Google.

This document will start with an overview of Kubernetes architecture aside from any specific distribution or cloud hosting provider. Then it will discuss the specifics of how we are currently using Kubernetes with this application, and then, finally, how contributors can create a local development version of our infrastructure for testing, etc.

## Running Kubernetes

While you *can* download binaries and build from source from the [Github repo](https://github.com/kubernetes/kubernetes), it isn't quite 'batteries included'. A Kubernetes installation has some 'gaps' that need to be filled by systems administrators. A big example is it provides not mechanism for our services to interact with the outside world. Kubernetes specifies a service called *Ingress*, but does not have a default implementation. In other words, if you want to run "bare" Kubernetes, you have some work you have to do.

Part of the rationale behind this is actually responsible for the success of Kubernetes as the 'preferred container orchestration platform' over it's competitors like Docker Swarm, Apache Mesos, and Nomad - it isn't too *opinionated* about *how* things get provided, allowing for enterprises and cloud hosting providers to 'plug in' solutions that make sense in their environment.

Most people that run Kubernetes run a particular *distribution* of Kubernetes. Some of these are Free and or open source implementations, others are commercial offerings.

Choices in running Kubernetes fall into a few categories, which I'll list out below:

### Cloud Hosted

Cloud hosting providers, like Amazon, Google, Microsoft Azure and others offer "pay as you go" Kubernetes services. In the last few years these have become very compelling, not the least of which is because of the pricing (can be expensive, but cheaper than your own server bunker). Creating a Kubernetes cluster on these services is incredibly easy. Digital Ocean for example has basically "1 click" experience. You can have your own three-node Kubernetes cluster in about 5 minutes, and it'll cost you less than $50 a month, and they bill by the hour. Like all cloud hosting providers, a main advantage is that you can easily scale the resources as you grow.

### On Prem and Hybrid

Two big names for on-premises Kubernetes offerings are from Rancher and Redhat.

Redhat's commercial offering is OpenShift. OpenShift is a distribution of Kubernetes known for it's superior support, excellent observability, and automated tooling for common tasks, including increased security and compliance tools.

Rancher makes K3S, a smaller distribution that is meant for "edge" scenarios (probably not going to run your 50 billion a year company on it, but it is good for smaller installations. People run it on Raspberry PI computer clusters!). 

I have a five-node Rancher Kubernetes cluster running at home on micro-pcs. Took about two hours to set up.

These offerings allow you to do 'hybrid', in that some of the nodes (or storage) in your cluster can be on cloud hosted platforms, or you cluster can scale to those platforms under increased temporary demand.

### Developer Installations

To help with the Dev/Prod Parity factor from the Twelve Factor App guidance, developers run small Kubernetes clusters on their development machines. These are by definition single node (machine) clusters, which is a little oxymoronic.

- [K3S (includes Rancher Desktop)](https://k3s.io/)
    - Rancher's K3S is a good solution for this because of it's ease of installation.
    - It must run on a Linux operating system (like all Kubernetes distributions) but does not have built in support for provisioning virtual machines on a developers workstation for providing this.
    - Tools like Vagrant and HyperV are good options to use along with K3S.
- [Docker Desktop](https://docs.docker.com/desktop/kubernetes/)
    - Docker Desktop includes support for running a single-node Kubernetes cluster for developer testing.
    - It has some limitations that, for me, put it out of my 'to use' list:
        - It does not allow you to select a particular version of Kubernetes. Whatever they package with Docker Desktop is what you get. This is a violation of the Dev/Prod Parity Factor.
        - It also does not supply an Ingress so you are limited to port forwarding to test your services. This isn't a complete show stopper, but still limiting.
- [Minikube](https://minikube.sigs.k8s.io/docs/start/)
    - Minikube is a SIG project of the Kubernetes project as a whole, and part of "the family". 
    - It allows you to create a Kubernetes cluster on your local machine that uses either Docker, HyperV or other virtualization support.
    - It has "plugins" that allow Ingress and Ingress-DNS.
    - You can specify which version of Kubernetes you want to run.

- [Red Hat CodeReady Containers](https://developers.redhat.com/products/codeready-containers/overview)
    - Allows you to run OpenShift on your local machines.
    - Fairly new technology - replaces previous tool "MiniShift"
    - Promising for those running an OpenShift cluster.
    - It requires a license to use.




## Kubernetes Architecture

The most important thing to understand is that Kubernetes is a tool for creating a computer _cluster_. A cluster is a bunch of computers that you treat as a single computer. Of course, most Kubernetes clusters are made out of a _lot_ of computers (called **nodes**), and _somebody_ has to be responsible for them - security, configuration, all that stuff. But from the point of view of a developer, with a cluster, specifically Kubernetes, you have an API. You send _stuff_ to that API, and you don't worry so much about how the whole thing is configured. Unless you want to. And if you do, that is cool and very valuable. We are going to emphasis the _abstractions_ that are provided by Kubernetes for developers.

### The General Kuberentes _Vibe_

I want to give you a sense of how you, as a developer, might think about Kubernetes.

When we write software on our local machine, we rely on the operating system to provide some functionality for us. And the less we have to think about how that functionality is provided and implemented, the more we are able to stay focused on the work we are trying to accomplish. If we need to open a file on the hard drive, there are APIs available for that, and we just _use_ them. If we want to open a network connection, we just do it. All that sort of _ambient_ stuff that is available to us as developers that is just a given.

In distributed applications, which are applications that are spread across multiple physical machines, across networks, etc. it becomes much more complicated. Especially in an orchestrated environment, where the software we are writing might be actually running in multiple _replicas_ simultaneously for scalability and fault-tolerance. Our applications will depend on other applications running in our cluster, and Kubernetes will move those suckers around on us, like a shell game. Those services (and ours!) will be upgraded, downgraded, rescheduled, scaled, etc.

Now all that stuff will be explored later, but the point is, we sort of need a new operating system that takes all that kind of stuff into account, handles it for us, and lets us focus on what we are supposed to be doing. You know, the whole 'delivering value for the customers' thing.

That is a really good metaphor, I think.

> Kubernetes is an operating system for distributed applications to run on.

Now, when we are working on our local machines, our operating system has a set of things that it can provide (file system, memory management, networking, etc.), and then we also need _other_ things. We might need a database, and most operating systems don't ship with a database that is a good choice for developers to use. So, we might pick one, like [MongoDb](https://mongodb.com), or Sql Server, or whatever, and download it, install it, configure it, etc.

Kubernetes will take care of that kind of thing for you. You just sort of tell it what you need (this is called the **desired state**), in a pretty clear way, and it will do its darndest to get it all together for you. And unlike most operating systems, it'll keep it's eye on it after it is created and make sure it's all working great, and if it isn't, it'll fix it.

An installed Kubernetes cluster is really a set of promises of things that can be provided to software running on that cluster. Some of those things are "batteries included", they just are there and ready to go. Others have to be provided by the people that install and configure your cluster. For example, one of the things a Kubernetes cluster will provide for your applications is a way for software running _outside_ of the cluster to make a network connection to software running _inside_ the cluster. You "tell" the cluster, through your desired state configuration, that "Hey, this little web server I asked you to run for me? Make it so that people can connect to it from the outside world by going to `https://our-site.com`. That feature in Kubernetes is called **Ingress**. And there isn't one included "in the box", so to speak. So, if you are going to create a cluster, you have to decide, amongst many options, how you are going to fulfill that promise when a developer asks for it.

Then there are things that you, as an application developer can ask for, but you might need some "higher level" configuration to provide it. An obvious and common example is just some disk space that you can write to or read from. In your configuration you'll just ask for (claim) some disk space, but some cluster administrator is going to have to decide where those bits are actually going to be written. (see "persistent volumes" and "persistent volume claims" below.)

### Key Kubernetes Abstractions

#### Cluster

A Kubernetes cluster is at least one machine with the Kubernetes software installed and running on it.

#### Nodes

> Nodes are machines that run Kubernetes and workloads assigned to the cluster. A kubernetes cluster is made up of nodes.

A node is a machine (real or "virtual") that is part of the cluster. Each cluster must have at least one node, but usually has many. Some of the nodes are assigned jobs and duties related to cluster management. We call these "master" nodes. Other nodes are added to just run our applications. These are called "worker" nodes. Running a cluster is a big job, so in production you'll rarely have just one node. Nodes can be dynamically added (or removed) from the cluster while it's running. It's even possible to have nodes on your cluster running "on prem", and then patch some more into it that are running on some cloud provider somewhere.

The group of master nodes is called the **control plane**. The control plane is the "brains" of the cluster. It exposes an API that we interact with, it schedules workloads to be run on worker nodes, it keeps track of how things are going (how the _desired state_ we want compares to the _actual state_), etc.

#### The Control Plane

The control planes' components control the cluster. They also detect and respond to events (starting new pods, rescheduling pods, etc.).

The control plane has many components. These are services that run on the cluster.

- `kube-apiserver`: Exposes the Kubernetes HTTP API. 
- `etcd`: A key-value store for storing configuration information, etc.
- `kube-scheduler`: Watches for new Pods with no assigned nodes and selects nodes for them to run on.
- `kube-controller-manager`: Runs controller processes. Contains:
    - Node Controller: Watches for nodes to go down and responds.
    - Job Controller: Watches Job objects that are one-off tasks, and creates Pods to run them on to completion.
    - Endpoints Controller: Joins services and pods.
    - Service Account & Token Controller: Creates default accounts and API access tokens for new namespaces.
- `cloud-controller-manager`: For cloud specific tasks - maps Kubernetes functionality to cloud provided code.

#### The Kubernetes API

You interact with a Kubernetes cluster through an API (provided by t he `kube-apiserver` service in the control plane). It is an HTTP API, and you can develop applications that use it. It is documented at [Kubernetes API Overview](https://kubernetes.io/docs/reference/generated/kubernetes-api/v1.23/).

Mostly you use command line tools:

- `kubeadm`: A command line tool for creating Kubernetes clusters and bringing new nodes into the cluster. Limited use by developers.
- `kubectl`: The primary command line tool for interacting with a Kubernetes cluster. [Kubectl Overview](https://kubernetes.io/docs/reference/kubectl/overview/)


#### Nodes

Nodes are each machine (virtual or real) in a Kubernetes Cluster. Nodes each have the following components running on them:

- `kubelet`: Makes sure that containers are running in a Pod. It makes sure they are running and healthy.
- `kube-proxy`: A network proxy that runs on each node. Maintains the network rules for the node, including *egress*.
- Container Runtime: Kubernetes originally used Docker to run containers, but now is more commonly using containerd, or CRI-O. Kubernetes supports any container runtime that supports the Kubernetes Container Runtime Interface (CRI).

| Lab | Instructions |
|-----|--------------|
| Creating a Minikube Local Cluster | [Instructions](../labs/cluster.md) |


### Kubernetes Objects

Kubernetes Objects are the "thingies" (abstractions) used to organize and run a Kubernetes cluster. [Kubernetes Objects](https://kubernetes.io/docs/concepts/overview/working-with-objects/kubernetes-objects/)

Kubernetes Objects are defined through the API. We can you *imperative* commands to create, modify, and delete objects using the `kubectl` command, such as `kubectl create namespace games`, however it is usually best to represent interactions with the API with a [YAML formatted](https://www.cloudbees.com/blog/yaml-tutorial-everything-you-need-get-started) file that can be version controlled as part of your codebase.

To create a namespace using the API using a declarative YAML document you might do something like this:

```yaml
kind: Namespace
apiVersion: v1
metadata:
  name: games
  labels:
    name: games
```

`kubectl` can then *apply* this to the cluster by converting it to the appropriate JSON format and submitting it to the API:

```shell
kubectl apply -f .\namespace.yaml
```

The `-f` flag is the name of the file to apply to the cluster. (if the path refers to a directory, *all* the YAML files in that directory will be applied).

Applying YAML files to the cluster is almost always an *idempotent* operation. In other words, it doesn't hurt to do it multiple times. The API will tell you there were no changes, but no harm done. (think updating a customer's email to 'bob@aol.com' multiple times in a database).
#### Namespaces

Namespaces in Kubernetes are the mechanism for isolating groups of resources within a cluster. Names of resources need to be unique to a namespace, but not the cluster. 

They are used when you have a large organization with many teams and/or projects. Namespaces can have a `ResourceQuota` object that limit the resources available across that namespace. [See Resource Quotas](https://kubernetes.io/docs/tasks/administer-cluster/manage-resources/quota-memory-cpu-namespace/), including CPU, and memory.

Using [Role Based Access Security](https://kubernetes.io/docs/reference/access-authn-authz/rbac/), users can have their access controlled to a namespace.

Not **all** Kubernetes objects belong to a namespace, but most do.

Examples of objects that *do not* belong to a namespace include:

- nodes
- persistentVolumes

#### Connecting to a Cluster and Setting Context

When we run the `kubectl` command against a cluster, for example, how does it know *which* cluster? How does it know who I am and if I have permissions to do such a thing?

The [`kubeconfig` file](https://kubernetes.io/docs/concepts/configuration/organize-cluster-access-kubeconfig/) provides all that information. By default it looks for the file named `config` in your home directories `.kube` folder. You can change this by setting an environment variable called `KUBECONFIG` with the path to your configuration file, or `kubectrl` has a `--kubeconfig` option where you can give the path.

Authentication against a cluster is up to the cluster administrator, but is typically done with either tokens or certificates.

One kubeconfig file can contain information about multiple clusters and multiple users. A cluster and a user for that cluster are united in a 'context'. So you can say:

- I want to connect to the `prod` cluster with the user `jill`.
- I want to connect to the `minikube` cluster with the user `jeff`.
- I want to connect to the `prod` cluster with the user `sam`.

Contexts in the kubeconfig file also specify your current namespace.

#### Names

Each Kubernetes object in your cluster has a name that is unique to that resource (in the namespace if a namespaced resource). 

Most resource types require a name that can be used as a DNS subdomain name, So:

- No more than 253 characters
- Contain only lowercase alphanumeric characters, '-', or '.'
- Start with an alphanumeric character
- End with an alphanumeric character



#### Labels, Selectors, and Annotations

**Labels** are key/value pairs that are attached to Kubernetes objects, such as pods. Labels are a way to attach metadata that is useful to the user, but not the system itself. They are mostly used to organize objects into sets of related objects within a namespace. [Examples of common labels](https://kubernetes.io/docs/concepts/overview/working-with-objects/common-labels/).

They can be used for query operations like the following:

```shell
kubectl get pods -l 'environment in (production, qa)'
```

This would return all the objects that have an `environment` label with the values of either `production` or `qa`.

**Selectors** are special labels used by some Kubernetes objects to specify sets of other resources to which they reference. This is commonly seen in `service` objects (discussed later), that provide a *stable name* for a Pod.


#### Pods and Deployments

Pods are the "unit of deployment" in Kubernetes. Pods are the things that run our code (workloads). Our code runs in a container on a Pod. (The name "Pod" is play on the docker whale logo - the collective noun for a group of whales is a pod.).

While you *can* directly create Pods on a Kubernetes cluster, you will mostly likely create them *indirectly* by creating an object called a *Deployment*.  A deployment is an object that describes a pod, but also gives Kubernetes information to monitor and watch that pod, including things like resource limits and requests. By using a Deployment, you can also specify *replicas* of Pods. Replicas are multiple copies of the same pods (for HA (high availability), and load balancing) that each can be provided unique configuration.

A Pod can have one or more containers. A Pod is a single unit of deployment, however, so those containers will be co-located on the same node.

| Lab | Instructions |
|-----|--------------|
| Creating a Deployment | [Instructions](../labs/deployment.md) |

### Kubernetes Networking

> Note: [Cluster Networking](https://kubernetes.io/docs/concepts/cluster-administration/networking/) is a good resource.

Every Pod gets its own IP address. You do not need to explicitly create links between pods and you almost never need to deal with mapping container ports to host ports.

This means that if several Pods are deployed to the same Node, each one is kind of like it's own virtual machine. You don't have to worry about things like 'Is this TCP port available on this node?' - you don't even usually want to know *what* node your Pod is running on! 

There are six different networking challenges in Kubernetes:

- Container-To-Container Networking within a Pod
- Pod-To-Pod Networking 
- Pod-To-Service Networking
- External to Service Networking (Ingress or Services)
- Internal Pod to External Service (Egress)

#### Container-To-Container Networking within a Pod

When you have a multi-container Pod, those containers can reliably communicate with each other (and should communicate with each other) through `localhost`. Remember, all the containers in a Pod are always deployed to a single node.

While this is the *easiest* type of networking, it should be used with caution. It is useful in specific cases (sidecars, adapters, ambassadors,), but it greatly limits your ability to scale your application across many nodes.

#### Services

While Pod to Pod networking can be done, we've seen that Pods get a unique name, randomly generated. Pods are also *ephemeral*. For example, as we scaled the number of replicas of our pod, new pods were created (and discard when we scaled down). Pods can also be automatically scheduled to run on another node, and the name would change. When we deploy a new version of the container, the pod is not reused. A new one is created, and then the other (old) one is removed when the new one signals it is ready for work.

Also consider when we have multiple replicas of a Pod, it wouldn't do us any good if each client to that service had hard-coded the name of just *one* of those specific Pods.

Basically, this means you can't *reliably* write static code that talks to other Pods. Their name keeps changing!

This is where the Kubernetes object of kind Service comes in. 

> Note: I find the name really confusing. "Service" is such an overloaded term. Here it has special meaning as a Kubernetes object.

The purpose of a Service is to provide "An abstract way to expose an application running on a set of Pods as a network service". Keep in mind the term "set" here is used mathematically - 1 or more.

Services typically *select* the pods they are responsible for.

> Advanced Note: Services, like deployments, actually create two Kubernetes objects when they have a selector. One is the service itself, and the other is an Endpoint. If you leave off the selector, no Endpoint is created for you. This can be useful when you might want a service to "point" at one thing in one environment and another in a different environment.

| Lab | Instructions |
|-----|--------------|
| Creating a Service | [Instructions](../labs/service.md) |

### Kubernetes Storage and Configuration

When talking about storage, the first thing we have to be clear on is the difference between stateful and stateless services.

For example, our OnCallApi is a *stateless* service. While it has some *data* (the `/data/schedule.json` file) that is baked in as part of the container image and is (currently) immutable. While our API may create some objects in memory for each request, those objects are safely discarded after each request. 

As application developers, pretty much *all* of our services should be written this way. 

But we also have services that need to persist data between calls - we use databases, caches, that sort of thing for doing that work. And remember, in Kubernetes (and the Twelve Factor App model), those things are *Backing Services*, and attached resources for our service.

"Real world" things that are written to the local file system, for example, in the kinds of applications we build are things like logs. In Kubernetes, we often use tools to monitor those logs (as a "stream of events" and shuttle them off to some instrumentation software to provide observability). This can be done by our own code, or by using a multi-container pod with a `sidecar` that simply is a sink for the logging and it takes care of sending it where it needs to go. Tools like a *service mesh* (Istio is an example, but there are others), can provide this sort of thing for you automatically.

Sometimes Web Applications where we have multiple replicas that are load balanced have some ephemeral state, even if their content is static, in the form of Sessions. Sessions have to be maintained *outside* in some way (using Redis, or a database, etc.).

#### Stateful Services Can Be Really Complex

But what about running databases, caches, all that stuff where we need persistent storage?

It's true, if you are running Stateful services (like caches, databases, etc.) in your cluster, you must have a way to reliably store the data. Kubernetes, as we will see below, can provide for that. But it's a huge topic because you also have to have *in depth* knowledge of how your particular technology choice for your stateful service handles scaling (replication? clustering?). Many common database technology has various ways to scale horizontally, but many come from an era where it is assumed you will scale *vertically*. You just continue to upgrade the server, adding more storage, more processors, more memory, etc. They aren't really designed to be distributed or clustered. Microsoft SQL Server largely falls into this category. While it is available to run in a container, configuring it to run in a scaled replica set is challenging and will require much configuration of the software, research, planning, and *then*, Kubernetes can help with the storage part of that using StatefulSets (mentioned below).

For example, you may decide, for High Availability, you want two instances of your SQL Server database running. You might configure one of them as the "fail over" server - it just sits idly by and can take over when the first one fails. This is complex stuff.

Many databases can be scaled horizontally, but often you have to have just one replica that can be written to, and the others are read-only. How do we make Kubernetes aware of that? (again, lots of database configuration, then StatefulSets).

More Cloud Native (meaning designed to run in our Kubernetes cluster) solutions rely on various types of *clustering*.  Clustering allows multiple replicas to be read from *and* written to (again, a cluster is when you have multiple instances of something that you treat as one). This is really useful in a Kubernetes environment because it can allow developers to bring the data "closer" to where it is needed improving performance in geographically distributed clusters. Examples of this kind of software are a Redis cluster, Apache Kafka, and CockroachDB. But these aren't just things that you are easily going to create a *deployment* for, pointing it at a container image, and saying "give me 12 of these suckers". 

I don't want to scare you off of these things, but they aren't things we are going to be able to cover and implement in a simple class like this one. They take an amount of coordination and technical knowledge about your particular clustering solution.

##### What About Running out Database *outside* of the Cluster?

This is often done. It is an understandable choice. For example, if you are building a cloud hosted cluster, your hosting provider may provide data storage in a cluster as a *service*. It's something outside of your cluster, managed by "them". In an enterprise environment, you may already have infrastructure in place with the data needed to support your applications.

The Pros of this are:

- It is familiar. "I know my database, and I know how to run a query against it"
- It is known. "We have DBAs, etc. that make sure that data is not corrupted, backed up, restored, all that"
- It is secure. "It's inside our firewall, and we have less of a chance of accidentally exposing data we don't mean to"
- It is programmable. "We do a lot of our business rule processing with things like stored procedure and triggers"
- It's super easy to use cloud hosting data services.

The Cons are:

- It isn't "Cloud Native": Not to be buzzword compliant, but this means you lose a lot of benefit in terms of scaling and portability. You aren't going to be able to provision more instances of your DB2 database "in the cloud"
- It is almost certainly a "shared database"
    - Violates the Microservice goals of team autonomy (independent deployability)
    - It is going to be *slower* in almost every case.
        - Cannot geographically distribute our data to cut latency
        - Some services will modify the data, some will read. In most databases these are competing concerns in database optimization.
            - "You can make a database query fast, or make it highly concurrent"

> **Note**: It is a good practice to use Kubernetes network policies to limit *egress* traffic to services outside of your cluster. Consider in your environment having one service that is responsible for accessing data outside of the cluster. Any changes to an outside database should go through this service, and the service (or another), can be responsible for returning the data your services need. In other words *abstract* away outside data from your architecture. 

Back to just talking about storage:


Container images are *ephemeral* and doubly so when running in a cluster. When a container is running in a container runtime, the runtime supplies the container *another* layer at the "top" of the stack of layers that make up the container that is read/write. The rest of the layers, as you recall, are read only. This is a temporary layer. It is discarded when the container is removed. That means anything you write to the filesystem is lost.

With containers you can mount a `volume` that is durable storage (either a place on the local machine's hard drive or a volume that is maintained by the container runtime), that is persistent. However, this won't really work in a cluster, because that container has no guarantee of restarting on the same node.

Kubernetes has a great (if not slightly complicated) way of abstracting storage away from developer concerns. The cluster administrator will different kinds of storage (called a `StorageClass`) that could refer to some local storage on a server somewhere, a NFS share (NFS is 'Network File System', a common way to share storage in the Linux world, like 'shares' in Windows), various cloud storage (Azure Disk, Google, Amazon Block Storage, etc). It is a pluggable architecture, so many different kinds of storage may be available.

The work of providing and using storage is split between the cluster administrator and the developers in the following way:

- Cluster Administrators
    - Provide StorageClass(es) in the cluster that allow the cluster to access various kinds of storage.
    - They provide `PersistentVolumes` that are *uses* of this storage class.
    - They provide facilities to backup and otherwise secure the data stored there.

- Developers
    - Make claims on that storage (`PersistentVolumeClaims`) stating they need to provision a certain amount of storage.
    - They use those claims as mounted volumes in their deployments.


#### StatefulSets

Creating stateful services, as mentioned above, is challenging. It is outside the scope of this course (though we will return to it in the Event Driven Architecture Course with Kafka).

For a good overview, see this video:

https://www.youtube.com/watch?v=zj6r_EEhv6s&t=1543s

#### ConfigMaps and Secrets and Environment Variables

For most stateless service developers, state that needs to be shared across your services is pretty minimal and is related to application configuration. 

ConfigMaps are kubernetes objects that can hold up to 1mb of data and make it available to your application. The configmap appears to your application as a volume. 

Secrets are ways to have the cluster hold things like credentials, api keys, and certificates and provide these to your containers during startup.

Environment Variables can be configured per container.

| Lab | Instructions |
|-----|--------------|
| Using a ConfigMap | [Instructions](../labs/configmap.md) |

#### Storage

Let's say we have read all the stuff about Stateful sets, etc. We understand. We know this is hard, but we just want a little itty-bitty database, not high-availability, all that, for storing a little bit of data for our service.

| Lab | Instructions |
|-----|--------------|
| Using Storage, Deploye SQL Server | [Instructions](../labs/sql.md) |



#### Health Checks

We should, in our services, write them in a way so that our cluster can find out how they are doing, health-wise.

Kubernetes can do a series of various *probes* during the life of our app, and then take action depending on the results of those things.

Kubernetes has three kinds of probes:

1. Liveness Probe
    - This will determine if the application process has crashed or deadlocked. If it fails, Kubernetes will stop the pod and create a new one.
2. Readiness Probe
    - This is to determine whether the application is ready to start accepting new requests. It might need a cup of coffee first, or to download some information from a remote service, prime a cache, etc. Until this succeeds, Kubernetes won't send it any traffic.
3. Startup Probe
    - This is used to say that it is "awake" during startup. After this succeeds, it will move on to Readiness probes, and when that succeeds, it will periodically use the Liveness probe to "check in".

Providing these in your API is super helpful for Kubernetes to be able to reliably manage your apps. I will admit, though, that the "right" way to do it is a fraught conversation among developers.

Some overall starting advice for this comes from https://andrewlock.net/deploying-asp-net-core-applications-to-kubernetes-part-6-adding-health-checks-with-liveness-readiness-and-startup-probes/.

Andrew says there can be two kinds of probes:

- **Smart Probes** verify the application is working correctly - it can make requests to other services, connect to the database, etc. 
- **Dumb Probes** just say the thing hasn't crashed. Basically, they can accept a request.

The reccomendation is:

- Use **dumb** liveness probes.
- Use **smart** startup probes.
- For readiness probes, it's more complicated.

Liveness can be just a simple "I'm good".

Startup may want to check that it can connect to it's attached resources. This can be helpful for not accidentally starting up a misconfigured application (maybe a typo in a config map or something).

Readiness is more complicated - you can get into issues where you were able to start, but then somehow not "ready" a bit redundant. Checking your attached resources here *again* can cause cascading failures or deadlocks.


| Lab | Instructions |
|-----|--------------|
| Health Checks | [Instructions](../labs/health.md) |

#### Init Container
- for migrations.

| Lab | Instructions |
|-----|--------------|
| Init Containers | [Instructions](../labs/init.md) |

#### Ingress

| Lab | Instructions |
|-----|--------------|
| Init Containers | [Instructions](../labs/ingress.md) |

#### Egress

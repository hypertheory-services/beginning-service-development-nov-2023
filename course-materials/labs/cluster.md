# Creating a Local Cluster For Development

In this lab we will create a local Kubernetes Cluster for development using Minikube.

Before creating our cluster, we will configure MiniKube to use:

| Feature | Value |
|---------|-------|
| cpus  | 2 |
| memory | 7168mb |
| driver | hyperv |

You can see, on Windows, your number of CPUs by using the Task Manager. Go to the Performance Tab and click on CPU. 

Minikube defaults to using docker for the driver, but we will use HyperV.

## Turning Off Docker Desktop

For our work we will turn off Docker desktop on our host machine. If you have a beefy developer machine, there is not usually a problem leaving both running, but since the VMs we are using in class are limited with processor and memory we will not strain our machines by having both running. Later I will show you how we can use Minikube as our *docker context* so that we can still build and work with containers.

- [ ] If Docker Desktop is Running - Stop it
  - If you click on the Windows task tray (bottom right), you should see the whale icon. Click it and stop Docker Desktop.

## Check if you have a Minikube Cluster Already Configured

From a command prompt, run:

```shell
minikube status
```

If you see something *other* than the following:

```shell
🤷  Profile "minikube" not found. Run "minikube profile list" to view all profiles.
👉  To start a cluster, run: "minikube start"
```

Run this:

```shell
minikube delete
```

To view your Minikube configuration:

```shell
minikube config view
```

To set our configuration, run the following commands:

```shell
minikube config set cpus 2
minikube config set memory 7168
minikube config set driver hyperv
```

Running `minikube config view` should show roughly the following:

```shell
- cpus: 2
- driver: hyperv
- memory: 7168
```

When we start our cluster, it will provision a VM in HyperV, and download Kubernetes, and start the cluster with the name `minikube`. It will also create (or alter) our kubeconfig file.

> *NOTE*: If you want to change the version of Kubernetes, you can start Minikube like this: `minikube start --kubernetes-version=v1.11.10`

Run the following the start your cluster:

```
minikube start
```

After it completes (it'll take a minute), you can run `minikube status` and you should see the following, indicating the control plan is up and running:

```
type: Control Plane
host: Running
kubelet: Running
apiserver: Running
kubeconfig: Configured
```

If you run, from your command prompt (I'm assuming here you are using Powershell) the following:

```
cat ~\.kube\config
```

> note: `kubectl config view` shows the same information. I just wanted to be clear on where the file is.

Your kubeconfig will be displayed.

```yaml
apiVersion: v1
clusters:
- cluster:
    certificate-authority: C:\Users\jeff\.minikube\ca.crt
    extensions:
    - extension:
        last-update: Sat, 29 Jan 2022 12:43:51 EST
        provider: minikube.sigs.k8s.io
        version: v1.24.0
      name: cluster_info
    server: https://172.22.27.150:8443
  name: minikube
contexts:
- context:
    cluster: minikube
    extensions:
    - extension:
        last-update: Sat, 29 Jan 2022 12:43:51 EST
        provider: minikube.sigs.k8s.io
        version: v1.24.0
      name: context_info
    namespace: default
    user: minikube
  name: minikube
current-context: minikube
kind: Config
preferences: {}
users:
- name: minikube
  user:
    client-certificate: C:\Users\jeff\.minikube\profiles\minikube\client.crt
    client-key: C:\Users\jeff\.minikube\profiles\minikube\client.key
```

At the top is the the list of clusters. We have one - named minikube.

At the bottom is our user, also named minikube. Users are identified to the cluster using certificates (issued by the Minikube).

In the middle is the `context`. As you can have many clusters and users, you can have many contexts. A context is basically a user + a cluster.

Each cluster has a namespace. That is your 'default' namespace (which is, coincidentally, called 'default').

You can only have one `current-context`, which is, again, Minikube.


Using this file, you can now work with the cluster using the `kubectl` command. For example, to see a list of namespaces in the cluster, you can run:

```
kubectl get namespaces
```

You should see the following:

```
NAME              STATUS   AGE
default           Active   12m
kube-node-lease   Active   12m
kube-public       Active   12m
kube-system       Active   12m
```

> Note: namespaces that start with `kube-` are the namespaces used by the control plane, etc. You should not start namespaces with `kube-` it is special and reserved.

## Namespace 

Let's create a namespace for our application. This should be the namespace your actual application will be in in other environments. Here we will use `hypertheory`.

Since you, as a developer, will probably *not* be creating namespaces (that is usually the work of the cluster administrators), we will consider this as sort of one-off "setup" work to gain Dev/Prod parity. Therefore, we will use the imperative kubectl command (as we don't need a YAML file in our source code for this).

```
kubectl create namespace hypertheory
```

After you run that, if you run `kubectl get namespaces` you should see our namespace added to the list.

```
NAME              STATUS   AGE
default           Active   17m
hypertheory       Active   3s
kube-node-lease   Active   17m
kube-public       Active   17m
kube-system       Active   17m
```

### Setting the Default Namespace

When we run commands with `kubectl` it will assume your current namespace (remember, it is the *default* namespace in our `kubeconfig`).

`kubectl` *does* allow you to pass an optional argument for which namespace should be the target of the command, but that can be error prone, especially on our development cluster. (in a 'real' cluster you will most likely only have access to a particular namespace within a context).

To set `hypertheory` in the kubeconfig file as our current namespace, you can run the following command:

```
kubectl config set-context --current --namespace=hypertheory
```

> Note: Directly editing your `kubeconfig` file is kind of bad form. let the tools do it for you.

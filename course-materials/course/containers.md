# Containers

Containerized applications have a history. We will start with some of the background, and some of the technologies that lead us to this point.

## Background - the Need for Containerized Applications

When you run applications on servers in production, the safest way is at a minimum one server per application. That way that one application has little opportunity to negatively impact any other applications on that machine. Use up all the memory, the hard drive, access stuff it shouldn’t be accessing, etc. That would be awesome. And with unlimited resources, maybe a good way to do it. But *wow* would that be expensive!

### Virtual Machines

Another way to do it is to take a single server and run multiple isolated operating systems on it. “Virtual Machines”. However, that isn’t a great use of resources, either. Each of those Virtual Machines (VMs) are *heavy*, and don’t scale well. Imagine your desktop with five copies of Microsoft Word running. Now imagine your desktop with five copies of windows running, each running a single copy of Microsoft word. Kind of like that. 

### Linux Containers
So, the Linux folks figured out a happy medium a while ago. This is something that is baked into the heart of Linux (the “kernel”) called *namespaces*. It’s a way to segment a machine so that software running in different namespaces can be lied to. Each namespace can be told to “report” it has X amount of RAM, Y amount of processor, access to this particular disk mount, etc. To the applications living in a namespace, they are blissfully unaware that they are living in a little glass jar inside of a much bigger universe. They can’t see their neighbors, and they can’t ask for more than they are allotted.

Linux Namespaces are awesome. They aren’t easy. And they don’t really have much in the way of affordances to help developers define and install them. As a developer, you basically deliver your code to the production people, and, if they are into it, they can define a namespace for your app. They will set limits on its resources. It’s considered none of your business really.

If you want, the Wikipedia article is pretty ok about this. [Check it out]([Linux namespaces - Wikipedia](https://en.wikipedia.org/wiki/Linux_namespaces)).

Well, part of the whole DevOps revolution is that this disconnect between the people that write the code (Devs), and the people that make it run reliably and securely (Ops) just doesn’t work in practice. Not well. It served us for a while, maybe, but we’ve outgrown it. What if the Ops people don’t give us the *correct* amount of resources for our application? What if we are sending them new versions really quickly, and they become the bottleneck? The Operations folks have done glorious work over the decades of letting developers live in ignorant bliss about things like security, reliability, dependencies, all that. It turned into a game of virtual Battleship. We’d keep making guesses until we’d sink their carrier, and them blame them!

### Enter Docker 
Docker was created as a way for us to responsibly close that gap. As part of our applications deliverable, we will, in pretty simple terms, define exactly what we, as developers, need for this application to run. What dependencies, what networking access, memory, disk, all that. You do it in a pretty abstract way. Like “I wrote this assuming this can have TCP port 80. And that I can write to a directory called `/etc/mydata`, and I think I can run with about 2 GB of ram, but might need up to 4. And oh, I run on this version of Linux, and need these other things installed. I need some linux utilities, like `curl`, I need NodeJS (or .NET Core 3.1, or Python, or whatever).  And  - oh, the data I need lives on or corporate database, so I’ll need a connection to that.
Now, you could just put this in an email or something. A set of instructions for the Ops people to follow when configuring your namespace.
As you can imagine, this might create a lot of back-and-forth. Lots of Teams meetings. Lots of mistakes.
Docker lets you specify all that, and then actually *run it* in an **exact** replica of how it will run on their machines. Instant feedback! Run your tests against it! Debug into it! Do a tiny bit of load testing on it to see how it behaves. Docker builds a *container image* of your application that you can *run* which means the difference between your environment and the production (or test, or qa, or compliance, or whatever) environment is almost completely eliminated. No more “It runs on my machine!” Problems. Remember, DevOps is about *confidence*. What can we do to increase the confidence that the code we produce will work as designed when it gets in front of a user. This is *huge*.

Somewhere near the end of your developer pipeline, the instructions you wrote on how the container image should be constructed (the `Dockerfile` will be used to build the container image that will move to the next environment. It would be pretty creepy if you hadn’t actually *tried it* before you shipped it, wouldn’t it. 

**You must be able to run and verify your container images locally if you are going to sign off on them going to the next environment**.

So, let’s be responsible. If you aren’t running your containers locally, running your tests against your containerized applications, verifying them for yourself in front of God and your team, you’ve kind of short-circuited the whole inspiration behind all of this stuff. You are still saying “hey, running code in production is your problem, not mine!” And you are missing the point.

And if you are creating something that you feel you can't run locally in a container in a meaningful way because it is "too big", or "has too many dependencies on outside things" - that are all *huge* red flags about your architecture that are being shown to you. Don't sweep this feedback under the rug!

### Kubernetes and Containers
Modern Kubernetes runs containers. For a long time it used the same Docker application we use on our developer machines to run those containers. But Docker not only runs containers. It builds them. It has an API. It works as a mini container registry on your machine. In other words, Docker is kind of a *monolith*. Kubernetes only needs to run them. So now it just has the stuff to pull down those container images from your registry at the end of your pipeline and run them.

## Container Concepts

A **container** is, conceptually, a standard unit of software that includes our software application and all of its dependencies so that we can ensure our software runs reliably across different environments.

As stated above, containers are a standardized way to deliver software that runs on the Linux kernel using linux *namespaces* to isolate the application from other processes running on that machine. 

> **Note**: Containers run on linux. The word "Containers" is nearly synonymous with the company "Docker", who developed the initial tooling and specification for containerizing applications. Now containers are "standardized" by the [Open Container Initiative](https://opencontainers.org/), and several vendors and open source tools can build, run, and manage containerized applications built to the OCI specification. 

### Working with Containers on Non-Linux Operating Systems
Containers are *not* virtual machines, but they have several of the same benefits including isolation. Containers run on the Linux kernel, so to build and run containers, you must have access to a Linux kernel. On Linux operating systems, this is a given, of course. On the Mac OS you must provide a virtual machine running the Linux kernel. You can do that on Windows as well, but Microsoft ships a linux kernel as part of the Windows operating system known as the Windows Subsystem for Linux (WSL). WSL is not a full linux distribution, and its use by Docker does not require a distribution to be installed on your Windows machine. WSL *does* allow you to run various Linux distributions on Windows, which can be installed from the Windows Store.

A popular tool for building, working with and running containerized application is **Docker Desktop**. Docker Desktop connects to the Linux VM (or WSL) and exposes an API and tooling that makes this transparent.

While there are other alternatives, this course will focus on using Docker Desktop. The Docker Desktop application is commercial software, and must be used according to its license. 

### Terminology

This is meant to serve as a sort of glossary of terms and concepts related to Docker and containers. Much of this content is based on the Docker documentation and [Microsoft's documentation](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/container-docker-introduction/docker-terminology).

- **Container Image**
    - A package with your application and the dependencies needed to create a **Container**. 
    - Think of this like a "program" on your hard drive. A **Container** is a running instance of that program.
- **Dockerfile**
    - The specially formatted text file that contains instruction on how to build a **Container Image**. 
    - The first line almost always contains a `FROM` instruction specifying an existing container image to "start with".
    - Additional commands create additional "layers" in the resultant container image that build upon the previous layer.
- **Build**
    - The process of making a container image from a Dockerfile.
- **Container**
    - A running (or paused (stopped)) instance of a container image. 
    - You can have multiple instances of a container image running.
    - Containers are read-only.
- **Volumes**
    - A writeable filesystem that the container can use. 
- **Tag**
    - A mark or label you can apply to a container image.
    - Tags typically take the form of `registry/respository:version` For example:
        - `mcr.microsoft.com/sqlserver:2017`
            - `mcr.microsoft.com` refers to the Microsoft Container Registry.
            - `sqlserver` is the repository (repo).
            - `2017` is the tagged "version".
    - The *assumed default* registry is `hub.docker.com`.
        - a container image tagged like `redis:latest` should be assumed to refer to the container image `redis` stored at `hub.docker.com`, and `:latest` means you will take whatever the owner of the repo says is the latest release.
        - `docker pull redis` and `docker pull redis:latest` would do essentially the same thing.
- **Multi-Stage Build**
    - When building container images, especially in automated builds, we use containers to build our application, install dependencies, even run tests, etc. The artifacts of the build process (if successful) do not rely on the the tooling needed to run a container.
    - The goal is always to "ship" a container that is the "smallest"
        - There is no reason to ship, for example, the dotnet compiler with our application.
        - Size is related to how quickly we can elevate code.
        - Reducing the dependencies not needed in production also limits threat exposure.
    - A multi-stage build creates multiple container images during the build process, culminating in a slim container that is suitable for publishing.
- **Repository**
    - A collection of related container images, each labeled with a tag that indicates the version.
    - Some repos contain many versions of a container image (some with SDKs, etc, some 'slim')
    - The version segment of the tag (the part after the colon) is not standardized, but should be within an organization.
        - These are known as **Docker Trusted Registries** (DTR)
        - Some examples of versioning schemes:
            - Semantic Versioning (three part, `major.minor.patch`) [See Semver.org](https://semver.org)
            - Semantic Versioning without patch (two part, `major.minor`)
        - A short SHA hash of the commit related to the tagged release in the source control repo.
- **Registry**
    - A service that stores container images.
    - Google (gcr), Microsoft (mcs), Amazon (aks), and Google (ghcr.io) are examples of publicly available container registries.
    - Docker Desktop includes a container registry for local use.
        - When you run `docker run someimage:v1` it looks *first* in the local registry, then the configured external registries. 
    - Enterprise organizations can maintain their own container registries.
        - For example, your build pipeline may have *write* access to the corporate registry, and the cluster may have *read* access.
- **Multi-Arch Image**
    - Because hardware architectures vary, Docker can automatically select a version of an image for the particular architecture.
- **Compose**
    - A command line tool (`docker compose`) and a YAML file format for running multi-container applications on a single machine.
- **Cluster**
    - A set of machines (or virtual machines) so that your running containers can span multiple machines.
    - Various tools have been created to allow clustering for running container images.
        - Examples include:
            - Docker Swarm
            - Azure Service Fabric
            - Apache Mezos 
        - For all intents and purposes, Kubernetes is the reigning champ and the assumed default for clustering.

## Running Container Images

## Building Container Images

Container images are built from *layers* of files. Each file is identified by a SHA256 hash of its content. [See "A Bit About Hashing" if this is new to you.](./hashing.md).

Normally we give a file a name that is hopefully semantically related to the content of the file (for example `notes.txt`). But by naming files using the hash of their content, and regenerating the file name each time the file is changed, we have certainty that the content of the file is immutable. Changed or added content produces a new hash, which in turn means a new file, since the name is the hash.

In your Dockerfile, certain commands (namely `RUN`, `COPY` and `ADD`) create new layers. Other commands might create *intermediary layers*, but they don't ultimately impact the size of your final container image.

### Let's Build One

> The code for this demo is in the `demos/docker0` folder in the root of this repository.

Let's say we want to have a docker image that, when run, displays a greeting. The simplest thing would be something like this:

`Dockerfile`:
```Dockerfile
FROM ubuntu:20.04
ENTRYPOINT ["echo", "from the container!"]
```

The `FROM` command (usually the first command in a Dockerfile) says 'I'll start with this Linux distribution and version. It's the closest to what I need to run my stuff".

The `ENTRYPOINT` says "when you are up and running, run this command. When this is done, you are done. Shut down". So we are using the Linux `echo` command to print to the terminal (really STDOUT) a message.

To build our container image, we will navigate using our terminal to the directory where the Dockerfile lives and execute the following command:

```bash
docker build -t greet .
```

`docker` is a global command - it's on your `PATH` when you install Docker Desktop. 

`build` is the *subcommand* it means we want to, uh, build a container image.

`-t greet` says we want to tag this container image as `greet`

`.` Is the build context. This is important and we often forget it (especially when copying and pasting.) We have to provide it, but it isn't too important in this example, so I'll go into that later.

When you run the command, if you typed it all correctly, you will see a bunch of stuff flying across your screen. I get a kick out of that. Makes me feel like I'm Neo hacking the Matrix. Most likely, it will discover that it doesn't have our base image (`ubuntu:20.04`) in the local docker registry. If it isn't there, it will go download it. If it is there, it will use it. 

> Note: This is the first step into a new world of software philosophy you will find with docker and Kubernetes. You say what you want, and the tools are programmed to 'make it so'. It *could* tell you that the image isn't available locally, that you must first `docker pull ubuntu:20.04` first, but they are kind and just do it for you. This is `declarative` programming - we say what we want, and leave it to the tools to make it happen. Rad as heck.

Question: Where did it pull that `ubuntu:20.04` image from?

Answer: Most likely [Docker Hub](https://hub.docker.com). In your work environment you may not be able to pull images from that registry. Scary stuff might be put up there. Your installation of docker will need to be configured to pull from a trusted registry.

If it succeeded in building your image, just for fun, use your up-arrow key in the terminal to run the command again. Notice how much quicker it was? It really didn't have to do much.

The last line of the output is probably something like:

```bash
...
naming to docker.io/library/greet
```
Since we didn't *formally* tag our container image, it assumes you will want to put it on the `docker.io` registry. It also assumes you want to have the repository called `greet`, and since we didn't put a version, it assumes this is the 'latest' version.

You can see the image in your local registry by running the following command:

```bash
docker images
```

You should see some output like this:

```bash
REPOSITORY                        TAG       IMAGE ID       CREATED          SIZE
greet                             latest    da62877a7ead   3 weeks ago      72.8MB
```

> You may see other images here, depending on when you've run this.

You can see the repository (greet), the tag (latest), the IMAGE ID (a shortened hash), when it was CREATED, and the SIZE (72.8MB)

But wait! Mine says it was created 3 weeks ago! (yours will vary depending on when you run this). But I just did it!

This is because our Dockerfile really doesn't have any *layers* other than Ubuntu:20.04, and that was (at the time of this writing) about 3 weeks ago. 

> Note: Where are the 'layers' stored? It depends. If you are running Docker on Windows with WSL, there is a folder somewhere in there with the files.

You can get a sense of the container image by running the following command:

```bash
docker inspect greet
```

This will print out a huge honking JSON formatted file with all sorts of information about your container image. 

```json
[
    {
        "Id": "sha256:da62877a7eadf60842f7a8fd292763c7e5e3628daf8bf2b8214e99d7f6003ae4",
        "RepoTags": [
            "greet:latest"
        ],
        "RepoDigests": [],
        "Parent": "",
        "Comment": "buildkit.dockerfile.v0",
        "Created": "2022-01-07T02:25:30.389665393Z",
        "Container": "",
        "ContainerConfig": {
            "Hostname": "",
            "Domainname": "",
            "User": "",
            "AttachStdin": false,
            "AttachStdout": false,
            "AttachStderr": false,
            "Tty": false,
            "OpenStdin": false,
            "StdinOnce": false,
            "Env": null,
            "Cmd": null,
            "Image": "",
            "Volumes": null,
            "WorkingDir": "",
            "Entrypoint": null,
            "OnBuild": null,
            "Labels": null
        },
        "DockerVersion": "",
        "Author": "",
        "Config": {
            "Hostname": "",
            "Domainname": "",
            "User": "",
            "AttachStdin": false,
            "AttachStdout": false,
            "AttachStderr": false,
            "Tty": false,
            "OpenStdin": false,
            "StdinOnce": false,
            "Env": [
                "PATH=/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin"
            ],
            "Cmd": null,
            "Image": "",
            "Volumes": null,
            "WorkingDir": "",
            "Entrypoint": [
                "echo",
                "from the container!"
            ],
            "OnBuild": null,
            "Labels": null
        },
        "Architecture": "amd64",
        "Os": "linux",
        "Size": 72776453,
        "VirtualSize": 72776453,
        "GraphDriver": {
            "Data": {
                "MergedDir": "/var/lib/docker/overlay2/db904b39bf32b6e52097bb4a481abb3dc402fc8a45746a6f1dae962bf182c45c/merged",
                "UpperDir": "/var/lib/docker/overlay2/db904b39bf32b6e52097bb4a481abb3dc402fc8a45746a6f1dae962bf182c45c/diff",
                "WorkDir": "/var/lib/docker/overlay2/db904b39bf32b6e52097bb4a481abb3dc402fc8a45746a6f1dae962bf182c45c/work"
            },
            "Name": "overlay2"
        },
        "RootFS": {
            "Type": "layers",
            "Layers": [
                "sha256:0eba131dffd015134cb310c284b776c1e44d330146cd2f0e30c4e464d0b76d24"
            ]
        },
        "Metadata": {
            "LastTagTime": "2022-01-28T19:35:50.268260303Z"
        }
    }
]                                                                               
```
There is all sorts of juicy information in there, but I want to focus on just a couple pieces:

- `Image ID` - the formal (SHA256 hash) of the image. 
    - It will start with `sha256:` indicating the algorithm used to produce the hash.
    - Then the big long hash, but if you look a the first bit of the hash, it should match what you saw in the output of `docker images`.
- `Env` - is the set of environment variables available to the container when it runs. Here we have just the path.
- `Architecture`: This is the processor architecture. When I ran this on my home workstation it was `amd64`. What is yours?
- `Config:EntryPoint`: This is the meat. This is the command we told it to run. It didn't 'bake' this into the image. 

If you run the following command:

```bash
docker image history greet:latest
```

You will see the 'history' of this container image, including a listing of the *intermediary* layers that were created during the build.

The intermediary layers are created during the build, but they are gone. Deleted and no more. Not needed.

```bash
IMAGE          CREATED       CREATED BY                                      SIZE      COMMENT
da62877a7ead   3 weeks ago   ENTRYPOINT ["echo" "from the container!"]       0B        buildkit.dockerfile.v0
<missing>      3 weeks ago   /bin/sh -c #(nop)  CMD ["bash"]                 0B
<missing>      3 weeks ago   /bin/sh -c #(nop) ADD file:122ad323412c2e70b…   72.8MB      
```

Let's run this thing and see what happens!

You can run this from anywhere on your machine. `docker` is a global command on your path, and the image registry is local on our machine as well.

```bash
docker run greet
```

In a second or two you will see the output. Impressive.

```bash
from the container!  
```

What it just did is create a container from your docker image, and run it at the `ENTRYPOINT`. That echoed out to the STDOUT our message, then had nothing else to do.

The container is still 'there' though. It isn't running anymore, but it is there, like a zombie. You can see running containers by issuing the following command:

```bash
docker ps
```
If you run that, by default, it only shows *running* containers. Do see the ones that aren't running anymore, you can add a flag to the command to say show 'all' containers:

```bash
docker ps -a
```

Now you should see something like the following:

```bash
CONTAINER ID   IMAGE     COMMAND                  CREATED         STATUS                     PORTS     NAMES
93deb6a0f7df   greet     "echo 'from the cont…"   3 minutes ago   Exited (0) 3 minutes ago             gallant_mendeleev   
```

The big information here is the container id, the image, the command (our entry point), when it was created, the status, any exposed ports, and the name. The name will be different on your machine. When you `run` a docker container image if you don't supply a name, it will pick a random name for you. It's some adjective, an underscore, and a name of a famous scientist, engineer, or programmer. Cute. Consider making yours your new hacker name.

If you want to create a name for an container when you run it, you can do this:

```bash
docker run --name bozo greet
```

I picked the name `bozo`. (note, not my real hacker name).

Now when I run `docker ps -a` I see this:

```bash
CONTAINER ID   IMAGE     COMMAND                  CREATED          STATUS                      PORTS     NAMES
e3d24ebb1e19   greet     "echo 'from the cont…"   57 seconds ago   Exited (0) 58 seconds ago             bozo
93deb6a0f7df   greet     "echo 'from the cont…"   7 minutes ago    Exited (0) 7 minutes ago              gallant_mendeleev   
```

To remove a container when you are done with it you can run the following:

```bash
docker rm bozo
```

If you are running a container image like ours, that just sort of starts up, runs, and then quits, you can tell it in the run command to immediately remove the container when it is exited.

```shell
docker run --rm greet
```

Later we will look at stopping and restarting a container.

If you want to remove the container image (to free up space, or whatever), you can run the following, with the image name:

```bash
docker rmi greet:latest
```
If you have any running containers based on that image, you will have to remove them first.


> Note: `ps` is a linux command for showing running processes.

### Let's Go Farther

Ok, this is good and does what the customer wanted (prints a greeting). But we didn't even really write any software! They will laugh us out of the software developers club! How will we introduce bugs for the QA folks to find! This is a disgrace.

This time, to really up our software developer credibility, we will write our message to the console as before, but we will do it in the a *hardcore* language. C. 

> Note: the code for this is in the `demos/docker1` folder in your repository. But write it by hand. It'll be good for your LinkedIn profile to say you have written containerized application in C. 

Create a folder somewhere for our project. 

In that folder create an empty Dockerfile.

In the folder, create *another* folder called `src`. That's the cool kid place to put your source code. 

> Note: I pronounce 'src' as Cersei, as in Cersei Lannister. Because I'm a nerd.

In the `src` folder create a file called `hello.c`.

Put the following code in it:

```c
#include <stdio.h>

int main(void) {
    printf("I'm running in a container!\n");
    return (0);
}
```

> Note: I'm not going to explain that. Nobody understands C.

Ok, let's make our Dockerfile again. In the Dockerfile in the folder *above* the `src` folder, add the following:

```Dockerfile
FROM ubuntu:20.04
RUN apt-get update
RUN apt-get install -y build-essential
WORKDIR /app
COPY src/hello.c /app/
RUN gcc -o hello hello.c
ENTRYPOINT ["/app/hello"]
```

Let's try to parse that.

- `FROM`: We know this one. Our handy ubuntu:20.04 base image. It's big, but mighty.
- `RUN apt-get update`: Ok, this is getting into the weeds. Second line, and already I have to get all deep here.

Ubuntu is a distribution of Linux. This means it is a bunch of code that does operating system stuff and contains utilities that live on top of the Linux kernel. It has some *basic* stuff in it, like a shell (bash), etc. but it doesn't have any tools to compile C applications.

Linux distributions have various ways of safely and securely installing software to use on the operating system. Ubuntu uses a tool called `apt`. (the *Advanced Package Tool*) There are other so called 'package managers' for other distributions of Linux. Debian distributions tend to use `Dpkg`, RedHat used `RPM`. Macintosh uses an operating system called Darwin, and users in that world seem to prefer a tool called Homebrew. 

We Windows users are used to just going out and downloading stuff we need, or maybe installing it through the Microsoft store. But we need a way to *script* that kind of thing. So some Windows users use a tool called 'Chocolately', and the command is `choco`. (note, this is an homage to the `nuget` package manager we use in .NET.  Get it? Chocolatey Nuget? yum.). Microsoft has been rolling out their own tool called `winget`. It's cool. 

But Ubuntu uses `apt`, and we are saying here, on the first `RUN` command, with `apt-get update` to go download the list of all the available packages so when I install something, I'm getting the latest goodness.

> Important Note: You *May* (ahem) work in an environment where access to package manager (like NPM, Nuget, and the apt package repository) are blocked and mediated in some way. Your `Dockerfile` will need commands to authenticate against the proxy or other tool that allows you access to this. See your local documentation for details.


So, back to our list:

- `RUN apt-get update`: Update the list of packages from our configured package source.
- `RUN apt-get install -y build-essential` There is a package available called `build-essential` which contains the stuff we need to compile a C application (namely the GCC compiler, and the headers and standard library and stuff). the `-y` flag means "don't ask me if I'm ok with installing this. Just do it!"
- `WORKDIR /app`: This tells the docker build agent that it should put our output into this directory in our container.
- `COPY src/hello.c /app/`: *Kind of* self explanatory. But copy the contents of the `src/hello.c` file into our `/app/` working directory. This is where that build context thing comes in. I'll talk about that more below.
- `RUN gcc -o hello hello.c`: Run is sort of obvious, but `gcc` (the Gnu C Compiler) is going to output (`-o`) a file called `hello` when it compiles our `hello.c` file. Notice we don't put the path stuff here. It assumes the `WORKDIR`. 
- `ENTRYPOINT ["/app/hello"]`: Remember, `ENTRYPOINT` is where it starts running when the container is started (by us with `docker run`)

#### That Build Context (.) Thing

When we run `docker build -t sometag .` that period at the end is an indication of the 'build context'. What docker does is copies the entire directory indicated by that final argument (for us, the period) into a location that is accessible to the docker daemon building the container image. (that's why we don't put a path on the `COPY src/hello.c /app/` above for the `src` folder. It refers to the the *build context*). So, be careful where you run `docker build`. If you are in a folder with a gig of images or something, it will have to copy them all.

### Let's Build It.

In the directory with the Dockerfile, run the following command:

> Note: You don't *have* to run `docker build` in the directory with the `dockerfile`. You can use the `-f` flag to tell it where to find it.
```bash
docker build -t greeting-c:v0.1 .
```
Here I'm being a little more explicit about the version part of my tag. 

After some gnawing and `apt-updating` and `apt-getting`, a little compiling, it should finish. `docker images` should now show:

```bash
REPOSITORY                        TAG       IMAGE ID       CREATED             SIZE
greeting-c                        v0.1      73ff03f37c7a   About an hour ago   345MB
```

(Your created time should be sooner. I took a break while writing this.).

It's BIG. Way bigger than the former image we created (which for me was about 72MB).

Let's inspect it with `docker inspect greeting-c:v0.1`:

```json
[
    {
        "Id": "sha256:73ff03f37c7ac2e42f0bdfbc1c76824674090a60c254ab47baac64c25f6c3679",
        "RepoTags": [
            "greeting-c:v0.1",
            "hello-world:latest"
        ],
        "RepoDigests": [],
        "Parent": "",
        "Comment": "buildkit.dockerfile.v0",
        "Created": "2022-01-28T19:18:58.368084759Z",
        "Container": "",
        "ContainerConfig": {
            "Hostname": "",
            "Domainname": "",
            "User": "",
            "AttachStdin": false,
            "AttachStdout": false,
            "AttachStderr": false,
            "Tty": false,
            "OpenStdin": false,
            "StdinOnce": false,
            "Env": null,
            "Cmd": null,
            "Image": "",
            "Volumes": null,
            "WorkingDir": "",
            "Entrypoint": null,
            "OnBuild": null,
            "Labels": null
        },
        "DockerVersion": "",
        "Author": "",
        "Config": {
            "Hostname": "",
            "Domainname": "",
            "User": "",
            "AttachStdin": false,
            "AttachStdout": false,
            "AttachStderr": false,
            "Tty": false,
            "OpenStdin": false,
            "StdinOnce": false,
            "Env": [
                "PATH=/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin"
            ],
            "Cmd": null,
            "Image": "",
            "Volumes": null,
            "WorkingDir": "/app",
            "Entrypoint": [
                "/app/hello"
            ],
            "OnBuild": null,
            "Labels": null
        },
        "Architecture": "amd64",
        "Os": "linux",
        "Size": 344825366,
        "VirtualSize": 344825366,
        "GraphDriver": {
            "Data": {
                "LowerDir": "/var/lib/docker/overlay2/yu6qsvs6go22waex793mhkyt7/diff:/var/lib/docker/overlay2/w7uihtm3bd8abvygfwq14wwt4/diff:/var/lib/docker/overlay2/b8iv7yz0965jrxzmhcu1p9jyz/diff:/var/lib/docker/overlay2/0miaw2jlc95a7rccxqw493wxf/diff:/var/lib/docker/overlay2/db904b39bf32b6e52097bb4a481abb3dc402fc8a45746a6f1dae962bf182c45c/diff",
                "MergedDir": "/var/lib/docker/overlay2/8w0e7dmh7wq1hql4fwi6wk7a1/merged",
                "UpperDir": "/var/lib/docker/overlay2/8w0e7dmh7wq1hql4fwi6wk7a1/diff",
                "WorkDir": "/var/lib/docker/overlay2/8w0e7dmh7wq1hql4fwi6wk7a1/work"
            },
            "Name": "overlay2"
        },
        "RootFS": {
            "Type": "layers",
            "Layers": [
                "sha256:0eba131dffd015134cb310c284b776c1e44d330146cd2f0e30c4e464d0b76d24",
                "sha256:54a841e3b27c2be08748da0298153f49e547d0ca1fe1b482404c895492eb1b97",
                "sha256:10bffcdf4143f11456d81bbb3dea204724a7c6c3914ebbadd95fbcfe30149da1",
                "sha256:98e4191c940230bc0141dc31af2766511ef386ec2a90fe3bcca467b66ec14d88",
                "sha256:5ca5c72dfe4565a0d736d73bf4dd6356efce46d845426a5a9f8d7fdd7411d65f",
                "sha256:78665eba48748f7088d0a190201565ee7b946ba284c53349e1641f04b5eb82df"
            ]
        },
        "Metadata": {
            "LastTagTime": "2022-01-28T20:41:59.010559732Z"
        }
    }
] 

````

Notice a couple of things:

- The `Config:EntryPoint` only has the name of our compiled executable.
- In the `RootFS:Layers` array, there are 6 layers! Before we only had 1.

Those are the results of our base layer (Ubuntu), plus the results of our `RUN` commands, etc. 

Somewhere nestled in one of those layers is the results of the `RUN` command that updated a our list of dependencies so we could install `build-essential` using `apt-get install`. We don't need that after we install it!

Also one of those lower layers is the `gcc` compiler and all the jazz that comes with the `build-essential` package. Not only do we not need that, we don't WANT it. Oh sure, put a compiler on an image as just a big old honey-pot for a bad guy with a black hoodie and mask.

### Let's Fix That with a Multi-Stage Build

Copy all the stuff from our previous demo into a new folder (the `Dockerfile` and the `src` directory).

Here's our new `Dockerfile`:

```Dockerfile
FROM ubuntu:20.04 as build
RUN apt-get update && apt-get install -y build-essential
WORKDIR /app/
COPY src/hello.c /app/
RUN gcc -o hello -static hello.c

FROM alpine:3.15.0 as final
WORKDIR /app/
COPY --from=build /app/hello ./
ENTRYPOINT [ "/app/hello" ]
```

A couple of big changes here, so let's go through them, line by line:

- `FROM ubuntu:20.04 as build`: The only change here is the label `as build`. This will allow us to refer to this later.
- `RUN apt-get update && apt-get install -y build-essentials`: Minor change we *could* have done before. Remember `RUN` creates a new layer, so by combining two commands (chaining with &&) we get one layer for the price of two. 
- `WORKDIR /app/`: No Changes.
- `COPY src/hello.c /app/`: No Changes
- `RUN gcc -o hello -static hello.c`: Only change here is the addition of the `-static` flag telling the compiler to staticly link the compiled application instead of dynamically (the default). This makes it easier to move to our `final` image below.
- `FROM alpine:3.15.0 as final`: Another `FROM`! - Alpine linux is bare metal stuff. Tiny. The distribution is about 5mb big. We label it `final` for documentation purposes. It's a thing.
- `WORKDIR /app/`: Same, no changes
- `COPY --from=build /app/hello ./`: This is cool. We copy the compiled, staticly linked application into the `/app` directory.
- `ENTRYPOINT ["/app/hello"]`: No changes.

Build that sucker:

```shell
docker build -t greeting-ms-c:v0.1 .
``` 

And then run `docker images`:

```shell
REPOSITORY                        TAG       IMAGE ID       CREATED          SIZE
greeting-ms-c                     v0.1      f9e891288f4c   16 minutes ago   6.46MB
```

Pretty tiny! ~345MB down to 6.4MB! 

> *NOTE*: Maybe "who cares? Disk space is cheap!". But the size and layout of the layers in our images have a direct impact on how easily and quickly we can elevate our code in production, and, should it be needed, roll back to a previous version if the world is on fire.

We can go even better if we really wanted to. We don't even need `Alpine`. A statically compiled single binary image doesn't need no stinking distribution, it can run directly on the kernel. This is only useful in cases like this - you have a *single* binary file you want to run. I've never done this with C applications, but I have written a few Go applications like this. They are particularly good for init containers and stuff like that. 

Simply change your `Dockerfile` to this:

```Dockerfile
# We will start with our trusty ubuntu:20.04 but label it as 'build' since we will only use this to build our application
FROM ubuntu:20.04 as build
# Each RUN creates a new layer, no matter how many commands you 'chain' together.
# This one will run apt-get install AFTER apt-get update completes.
RUN apt-get update && apt-get install -y build-essential
WORKDIR /app/
COPY src/hello.c /app/
## Added the '-static' here to staticly link the stdio library - dependency free executable.
RUN gcc -o hello -static hello.c
## Another FROM command - this time from Alpine - it's tiny. Like 5 mb. 
FROM scratch as final
WORKDIR /app/
COPY --from=build /app/hello ./
ENTRYPOINT [ "/app/hello" ]
```

Notice the change on the second `FROM`:

```diff
-FROM alpine:3.15.0 as final
+FROM scratch as final
```

`scratch` is a 'no op' image. It's basically nothing. (it is a real thing, but it doesn't do anything. you can't use any old word here).

If you build this image:

```shell
docker build -t greeting-scratch:v0.1 .
```
And then run `docker images`:

```shell
REPOSITORY                        TAG       IMAGE ID       CREATED          SIZE
greeting-scratch                  v0.1      87aaff4019cc   7 minutes ago    872kB
```

Now *that's* tiny. 


## The Takeaways (as the folks with MBAs say)

This was an introduction to building and running docker images.

There is a lot more to cover yet, but there are some basic things here that are important:

As a developer, *you are responsible* for your `Dockerfiles` and the images.
Your goal is to make them as small as possible.

Later we will go into more detail on the following things:

- Being deliberate with our layers (stuff that is likely the change more frequently comes after stuff that changes less likely at the beginning)
    - For example, install your Nuget packages *before* your source code in your `build` image. 
- Exposing stuff on the network.
    - Most of our containers will need to expose themselves (eek!) on the network. We will see how to do this.
- Accessing files and the filesystem.
    - It will *look* like your container can read and write to the hard drive. And it sort of can. But what containers do when you run them is they throw in a temporary layer at the top of the pile that can be written to. When you remove the container, that layer is thrown away.
    - So we write our applications that need filesystem access pretending like that isn't a 'thing', and then when we run them we say: 'Hey, when this app tries to write to the '/tmp' directory (for example), actually put that stuff *here* outside of the container. 
    - Passing in environment variables.
        - Environment variables are a feature of all modern operating systems. They are aptly named. They are 'ambient' variables with values that any process can access.

One of the challenges when reading the Docker documentation (or any documentation on containers) is understanding that the documentation most often assumes you are going to run the container image directly, and not in a container orchestration framework like Kubernetes. You have to avoid duplicating features in your `Dockerfile` that might otherwise be a part of your Kubernetes deployment configuration.

This course takes a *biased* approach. Where functionality is provided by Kubernetes (for example, volumes, environment variables, limits), we will prefer that approach. Many of these things are part of 'raw' Docker as well.




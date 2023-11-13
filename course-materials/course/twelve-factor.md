# The Twelve-Factor App And Modern Application Architecture

[The Twelve Factor App](https://12factor.net/) is a set of guidance created largely by engineers that worked or work at [Heroku](https://www.heroku.com/), one of the first cloud platforms. It is a distillation of their principles in writing "cloud native" software. It is a bit aged, but works well as a generally accepted set of principles to keep in mind when building modern applications.

It is presented in a technology agnostic way (and much of the technology we use to implement this now wasn't even available when the folks at Heroku were building their platform), but it is a good guide and a place to begin our discussion.

The language isn't particular to a microservice architecture. That wasn't as much of a thing in 2007 when these principles were being introduced. The term "App" in "The Twelve Factor App" refers to a web application, or a software-as-a-service offering.

As we know (or are learning), in a microservice architecture an "App" is really an ecosystem of supporting services, each doing their part in fulfilling the requirements of some customer facing application (a UI, or a service, etc.).

## I: Codebase

> One codebase tracked in revision control, many deploys.

My take on this, other than the obvious (use source code control!), is for us an "App" is a unit of deployment. A team (or person) owns an App, and it is deployed as a single unit.

In that single repository the information needed to create all the deployments reside. For example, you may not need dozens of instances of a service, load balanced, in your DEV environment, but may in QA, Load Testing, or Production.

While many "Apps" will be services that are independently deployable, and "live" to support other Apps, sometimes an App actually has multiple units of deployment (across the environments).

The rule "Keep the code together that changes together" is a strong rule here. For example, a team may have a user interface application (perhaps an Angular application), that also talks to a backend HTTP service the team owns. There is a one-to-one relationship between the user interface and the service (though the application may use *other* services it doesn't own). This pattern is sometimes called the BFF (Backend for Frontend) and helps mirror some of the winning strategies developed in building server-side web applications (a server side web application, at its most basic, is a single application that has access to internal services (databases, etc) and *produces* a user interface in the form of HTML pages for the client. The HTML pages are not deployed separately from the code that produces them.).

Other examples of an "App" that is actually multiple services might be a service that requires an *init* service to run before it is ready.

The big thing here is:

1. Code that Changes together and has shared ownership should be kept together.
2. Do *not* have separate repositories for each environment.

## II: Dependencies

> Explicitly declare and isolate dependencies

Use dependency management tools to make your apps dependencies explicit. Tools like NPM (the Node Package Manager), or NuGet (for .NET) applications store the versioned dependency information in the source code control.

Dependencies, aside from shared libraries, may be "ambiently" available in your operating system and must not be relied upon. For example, if you are using code that assumes the presence of a particular version of OpenSSL to be installed on the machine where your application or service runs, it must be part of your application source code using a dependency manager.

The big thing here is:

1. If you find yourself referencing a DLL or another project in a solution directly in .NET you are probably *doing it wrong*. Bundle those as Nuget packages and require them with your package manager.
2. MOVE code that stays the same over time, and especially code that is shared across projects (why were you setting a reference to a DLL?) to a package that can be managed by your dependency manager.
3. Operating System level dependencies are handled by your Dockerfile or the base image you use in your Dockerfile.


## III. Config

> Store config in the environment

The implication here, referencing point I above, is that you will have code in your repository responsible for "setting up" each environment the app will run in. This needs to be explicit.

Configuration may be things like:

- Connection Strings to Databases
- Security Credentials (Secrets)
- Hostnames for services exposing an interface
- Feature toggles (or addresses of a server providing that information)
- Expected system environment variables

Note: Information that contains credentials (passwords, usernames, API keys) etc. should *never* be stored in source code control. Tools like Github Actions and Azure DevOps have mechanisms to store these values in a protected way, and to refer to them in your deployment code.

## IV: Backing Services

> Treat backing services as attached resources

It is best if in our services we treat access to other services using Interface Segregation Principle, with Dependency Injection. 

For example, in our controller for a service using .NET that needs to write a message to a Queue you might see something like this:

```csharp
namespace StoreApi.Controllers;

public class StoreController : ControllerBase
{
    private readonly IPlaceOrders _orderPlacer;

    public StoreController(IPlaceOrders orderPlacer)
    {
        _orderPlacer = orderPlacer;
    }

    [HttpPost("orders")]
    public async Task<ActionResult> PlaceOrder([FromBody] OrderRequest request)
    {
        if(ModelState.IsValid)
        {
            await _orderPlacer.WriteOrder(request);
            return NoContent();
        } else {
            return BadRequest();
        }
    }

}
```

The *implementation* of the `IPlaceOrders` interface could be any number of things, and these could change over time.

For example: 

```csharp
public class KafkaOrderProducer : IPlaceOrders
{
    public async void WriteOrder(OrderRequest order)
    {
        // code elided
    }
}
```

or

```csharp
public class RabbitMqOrderWriter : IPlaceOrders
{
    public async void WriteOrder(OrderRequest order)
    {
        // code elided
    }
}
```

In your `Program.cs` file, the composition root, you can set up your services like this:

```csharp
builder.Services.AddScoped<IPlaceOrders,KafkaOrderPlacer>();
```

Later, if you want to swap it out, you can do this:

```diff
-builder.Services.AddScoped<IPlaceOrders,KafkaOrderPlacer>();
+builder.Services.AddScoped<IPlaceOrders, RabbitMqOrderProcessor>();
```

> Note: Configuring DI/Services through feature toggles is a fairly fraught proposition. The service would need to be restarted *if* you could do it. We'll talk more about this later.

## V: Build, release, run

> Strictly separate build and run stages

This is an easy one, mostly. Don't change code in environments. All code changes originate in `dev` and propagate through the environments.

The meat here is that after the *build* stage, a *release* stage is created. Release is the build + the config it will use in "run", or production. Though you may have *n* number of releases before you actually put a particular release into production. Typically, each release is a *candidate* for production.

## VI: Processes

> Execute the app as one or more stateless processes.

Requiring clients to have an *affinity* to a specific running instance of a service is a violation. Services must be designed to be *stateless*, meaning that they can only hold data in memory for a single request/response cycle. Persistent data must be stored in a database, cache, or in a persistent volume that is *attached* to the service.

For this reason, my current approach is that for RPC communication within the cluster, HTTP is preferred in most cases because of its explicit statelessness. gRPC is (often) *stateful* - a connection is maintained between the client and the server. gRPC is useful in a multi-container Pod for container-to-container communication.

Stateless gRPC services (using the request/response model) aren't as constrained by this.

Another issue is Websockets, which also, typically, require client-server affinity ("sticky sessions"). Using a messaging system or cache can be a way to work around this. I've had success with using Redis as the backplane for SignalR, as well as the Azure SignalR Service. Using Kafka is also a cool way to do it, but in each of these situations you may have the opportunity for race conditions (a client may briefly be connected to two different instances of a service, or at least believe it is). The messaging (notifications, etc.) *must* be designed to be idempotent in such situations.


## VII: Port Binding

> Export services via port binding

In the past we might, for example, create an ASP.NET MVC Application that would be mounted, in production, to a virtual directory on IIS. 

12 Factor apps are completely "self contained". For example:

- The deliverable for a .NET Core API is our API code and the "web server" that exposes that functionality on the network (Kestrel).
- The deliverable for an Angular application is our compiled Angular application, and a web server to deliver that content to a browser (e.g. Nginx).

## VIII: Concurrency

> Scale out via the process model

In a *monolithic* application we would scale based on demand (anticipating max potential load) by building huge machines. This is "scaling up". 

Now, we "scale out" - meaning we use two techniques (or combinations of two techniques) to accommodate load.

1. Load Balancing:
    - Create multiple identical instances ("replicas") of the service and use a Load Balancer to intelligently direct traffic across the instances.
    - *Extract* portions of a service that are bottlenecks to performance into other services.
        - For example, a compute bound function in a service that does image processing may account for a large percentage of the processor and memory requirements for that service. This is a good indication that it should probably be its own service, and just that portion can be scaled (load balanced) accordingly.

## IX: Disposability

> Maximize robustness with fast startup and graceful shutdown

Our services (processes) are *ephemeral*. They startup and shutdown on a moment's notice. The control plane in our cluster may dynamically allocate additional replicas based on load. A small patch fix may be deployed and our services replaced in a rolling update. Someone might spill a beer on the server and the cluster will schedule the services to be replaced on another node.

We need to design our services so that they can spring to life and get *busy* rapidly. Essentially transparently "picking up where they left off as if nothing happened". 

Our cluster can interrogate our services for readiness (and health), and then start directing traffic to them.

In some cases, because our service may be "reincarnated" on an entirely different machine (node), we may have some preparation to do before the service can be ready to run. Init Containers can help with this.

Configuration (ConfigMaps), Secrets, persistent volumes and other services, as mentioned in factor IV above, are *attached services*. 


## X: Dev/Prod Parity

> Keep development, staging, and production as similar as possible

This is a huge topic for me. Historically, one of the things that decreases my confidence that my code will run in production is the fact that *production* is a completely different world. And entirely different universe.

I may do my development on my Windows laptop, and it may run on a server with 32 processors and 128GB of memory, configured entirely differently. And don't even get me started on using tools or libraries that require me to run code locally using a specific account (right-click run as administrator is the *devil*). Things like "Oh, I know my app will use SQL Server 2017 in production, but all I have locally is some weird version of SQL Express, that'll be good enough". 

Docker has been a God send for this kind of thing. I no longer have *any* local instance of any database (for example) on my local machine. I just pull and run a Docker image that *matches* what I use in production.

I also use various tools to move myself ever closer to Dev/Prod parity. I'm hugely skeptical of anything I can't actually run on my own local machine. This is up to an including deploying my app to a local instance of Kubernetes.

Granted, you need pretty beefy developer computers (laptops/desktops) for this kind of thing, but the price of that is miniscule compared to trying to track down issues that arise in these "well, runs on my machine" instances.

Even in testing, I see "fakes" as a potential problem (from *experience*). They can lull you into a false sense of confidence. (an example of this is using Entity Framework "In Memory" database for testing).

*Some* services (usually those available through *egress*, discussed elsewhere), are too difficult or impossible to run locally. My rule on that is if this is the case, *my* services won't rely on those things directly. 

So, for example, if your application has a dependency on data that is only available "off cluster" (perhaps in a mainframe DB2 database), services inside your application namespace have to be ignorant of that. Have a service (just one if possible) that abstracts that dependency away, and has an egress network policy to connect to it. 

These *can* be "faked" locally, and that is helpful. Tools like Postman can create mock servers, as can [MockServer](https://www.mock-server.com/#what-is-mockserver).

> Much like CI, "Dev/Prod" parity need to be *aspirational*. "Wouldn't it be nice if we could run our entire application locally for development, testing, experimenting, prototyping, etc.?" - **YES**. And it is **super** valuable, not only in terms of the huge savings of avoiding potential cu

## XI: Logs

> Treat logs as event streams

This is one that sort of shows the age of the Twelve Factor App "manifesto". But the spirit of this is what is known as *Observability* in cloud native applications.

The core of this is a live stream of data that gives insight into the running services in your application.

> An "Event Stream" is an ongoing sequence of messages that detail important things that happen at a point in time. (an 'event' is a thing that happens at a particular point in time)

So "Logs" here is expanded to include:

- **Telemetry** -  The operational metrics about running applications and services, databases, etc. Memory, processor utilization, network saturation, etc.
- **Logging** - Error messages, stack traces, informational messages, etc. useful for the care and feeding of the application.
- **Tracing** - Information that is correlated to a particular user or application across the services.

This information can be surfaced for a wide variety of uses, including:

- Application Health Monitoring 
- Analytics for business decisions
    - Super important for many things, including "Hypothesis Driven Development" [^1]
- Feature Planning


Much of the information from observability can serve as a baseline to give the control plane information it needs to "steer the ship". For example, using telemetry to determine "usual" memory and CPU requirements for an application can become part of the limits/requests for a service over time.

## XII: Admin Processes

> Run admin/management tasks as one-off processes

A good example of this is database migrations. As your app/service evolves, it is going to frequently land in an environment where the schema of the database will need to be adjusted to match the new code being put into production.

With those kind of things it is important they are part of our codebase, and they are run against a release. In other words *anything* you do as a developer that adjusts the *ambient* environment in which the code you are working on run has to be reproducible and automated.


### "Meta" Admin Processes

Another example of Admin processes, especially in Kubernetes, are the cluster operations that you and your team may not have the permissions to perform.

Examples include:

- Creating the namespace for your application.
- Creating RBAC and network policies for your namespace and applications
- Defining Egress/Ingress
    - Ingress in particular is usually based on an authority, so your namespace may be given "permission" to define any ingress routes in `https://games.super-corp.com/*`.
- Certificates for TLS, etc.

The creation of all these things *should* be defined and scripted or declared in your code base, but they may need to be run with different authority that the rest of your deployments that run in the namespace.

For example, you may have a deployment for a certificate needed by your ingress. In your dev/qa environments this could be a "fake" developer certificate using something like `mkcert`. However, in production, that deployment may point to a secret set by the cluster administrators.

Of course, there is no guarantee that ample nodes will be available for your new app in the cluster. For large deployments, a DEV or test environment that would allow ample simulated load testing, along with instrumentation, may be required to create ample resource requirements in your deployments so the physical servers can be acquired to accommodate the load.


[^1]: [IEEE, Hypothesis-Driven Adaptation of Business Models on Product Line Engineering](https://ieeexplore.ieee.org/abstract/document/9140225)
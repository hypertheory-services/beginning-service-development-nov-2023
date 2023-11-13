# Adding (Basic) Health, Readiness, and Startup Checks to our API

In our OnCallApi Solution, we will add some checks.

In the `Program.cs` we will add two classes that provide some information about the state of our service:

```csharp
public class BasicLivenessCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy());
    }
}

public class BasicReadinessCheck : IHealthCheck
{
    private readonly string _contentRootPath;

    public BasicReadinessCheck(string contentRootPath)
    {
        _contentRootPath = contentRootPath;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var filePath =  Path.Combine(_contentRootPath, "data", "schedule.json");
        if (File.Exists(filePath))
        {
            return Task.FromResult(HealthCheckResult.Healthy()); 
        } else
        {
            return Task.FromResult(HealthCheckResult.Unhealthy());
        }
        
    }
}
```

The first class, `BasicLivenessCheck`, will just return `true` when it is checked. This is a *dumb* probe, and not too bad in many cases for just checking if the application is able to receive a request.

We will use this class for both our `liveness` and `readiness` checks.

The second class, `BasicReadinessCheck` is demonstrating how you could check the status of attached resources, etc. to make sure you are ready to start receiving requests. Here we are just making sure that the file is present. You could also check to make sure you could connect to a database, message queue, etc. 

This class will provide our `startup` check.

## Add The Services

At the top of the `Program.cs`, after the `builder` is created, add the following:

```c#
var readinessCheck = new BasicReadinessCheck(builder.Environment.ContentRootPath);

// Add services to the container.
builder.Services.AddHealthChecks()
    .AddCheck<BasicLivenessCheck>("liveness")
    .AddCheck<BasicLivenessCheck>("readiness")
    .AddCheck("startup", readinessCheck);
```

Since the BasicReadiness check needs a dependency on it's constructor, we will give it an instance. The other calls to `AddCheck` will use the generic and allow .NET Core to create them for us. Then names passed in (`liveness`, `readiness`, and `startup`) will be used below and are just somewhat arbitrary names.



## Expose them on the App

In the `Program.cs`, after the call to `var app = builder.Build()`, we will add the health checks as *middleware*, exposing them from our API. There is a lot of extra configuration you can do here, including telling it to not cache, expose on a different port, use (or not use) authentication, etc. This is a basic example.

```c#
app.UseHealthChecks("/healthz/live", new HealthCheckOptions
{
    Predicate = check => check.Name == "liveness"
});
app.UseHealthChecks("/healthz/ready", new HealthCheckOptions
{
    Predicate = check => check.Name == "readiness"
});
app.UseHealthChecks("/healthz/startup", new HealthCheckOptions { 
    Predicate = check => check.Name == "startup"
});
```

Here we are exposing three routes that can be accessed by our cluster.

Run your app and check them out.

### Rebuild your container.

Since we update the code, we will have to rebuild our container. I don't think we need to update the version yet (that stuff isn't that important until you deploy these to the next environment the first time).

Make sure you have set the `docker` context in your terminal to use Minikube.

```powershell
& minikube -p minikube docker-env --shell powershell | Invoke-Expression
```

And rebuild the container:

```
docker build -t oncallapi:v0.1 .
```

## Update our Deployments to Use Our Probes

Change your `deployment.yaml` file to provide information Kubernetes will use to check the health, readiness, and startup ability of our containers:

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
        livenessProbe:
          failureThreshold: 3
          httpGet:
            path: /healthz/live
            port: 80
            scheme: HTTP
          periodSeconds: 3 # How often to run the probe, in seconds
          successThreshold: 1
          timeoutSeconds: 1
        readinessProbe:
          failureThreshold: 5
          httpGet:
            path: /healtz/ready
            port: 80
            scheme: HTTP
          periodSeconds: 10
          successThreshold: 1
          timeoutSeconds: 1
        startupProbe:
          failureThreshold: 5
          httpGet:
            path: /healtz/startup
            port: 80
            scheme: HTTP
          periodSeconds: 10
          successThreshold: 1
          timeoutSeconds: 1
      volumes:
        - name: schedule-config
          configMap:
            name: schedule-config-map

```
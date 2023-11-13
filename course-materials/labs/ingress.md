# Ingress

Enable Minikube Addons:

```
minikube addons enable ingress
minikube addons enable ingress-dns
```

Install MkCert

```
choco install mkcert
```

In a directory, make the certs:

```
mkcert hypertheory-class.com "*.hypertheory-class.com" 
```

Trust the Certs:

```
mkcert -install
```

Create the secret for the certs:

```
kubectl create secret tls tls-cert --key hypertheory-class.com+1-key.pem --cert hypertheory-class.com+1.pem
```

## Adding Ingress DNS

```
 Add-DnsClientNrptRule -Namespace ".hypertheory-class.com"  -NameServers "$(minikube ip)"
 ```

 > NOTE: Only do this next part to remove it.

 ```
 Get-DnsClientNrptRule | Where-Object {$_.Namespace -eq '.hypertheory-class.com'} | Remove-DnsClientNrptRule -Force; 
 ```

 
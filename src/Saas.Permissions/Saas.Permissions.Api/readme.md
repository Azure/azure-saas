Permissions API supports two auth schemes:
- JWT/AAD Auth
- Certificate Auth


Permissions Controller only supports JWT/AAD Auth
CustomClaims Controller only support certificate auth


To auth with certificate auth, generate a self signed certificate (not reccomended for prod use). Place the thumbprint in the `SelfSignedCertThumbprint` app setting. 
Certificate needs to be sent in a .cer format (b64 string) in the `X-ARR-ClientCert` Header. Auth will only succeed if the thumbprints match. Need to add docs on how to add futher validation if needed

https://docs.microsoft.com/en-us/azure/app-service/app-service-web-configure-tls-mutual-auth

https://docs.microsoft.com/en-us/aspnet/core/security/authentication/certauth

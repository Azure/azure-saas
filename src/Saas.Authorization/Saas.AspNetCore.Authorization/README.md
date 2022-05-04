# About
This library is useful if you have a multi tenant ASP.NET application with users potentially having 
different roles in different tenants.
## Goals
- Easy to use for developers
- Integrate with ASP.NET authorization libraries
- Use REST conventions apply authorization rules
- Leverage OAuth/OIDC but allow alternate approaches
- Allow users to have different roles depending on the context (subscription, organization, etc)

## Approach
- Add custom roles in the form of \{contxt}.Role to users ID_Token
- Provide extensions for the application to be able to authorize based on different request properties (context)
  

For example given the following registeration for a subscriptionwith ID ```subscription_2```:
```C#
builder.Services.AddRouteBasedRoleHandler("subscriptionId");
```
And this action method:
```C#
[Authorize(Roles = "SystemAdmin, SubscriptionAdmin")]
[Route("subscriptions/{subscriptionId}/Admins")]
public IActionResult GetUsersAdmin(string subscriptionId)
{
    return View();
}
```
Will allow only users with ```subscription_2.SubscriptionAdmin``` __or__ ```subscription_2.SystemAdmin``` __or__ ```SubscriptionAdmin``` __or__ ```SystemAdmin``` 
to access the view.


# How to use
## ClaimToRoleTransformer
*ClaimToRoleTransofrmer* is used to convert custom claims that are added to users ID_Token into roles that are 
compatible with ASP.NET authorization.  This way you can use roles that your identity provider adds to the token 
for authorization.  For example if you are using AADB2C you can enrich your tokens from an external source using 
an [API connector](https://docs.microsoft.com/azure/active-directory-b2c/add-api-connector-token-enrichment).  This way 
they can be validated and processed using the same tools.

Will generate a new ClaimsPrincipal that has an additional identity with the added roles.

__Note:__ You don't need this if your claims are already in an array format.

### To enable:
Add the settings to your appsettings.json
```json
  "ClaimToRoleTransformer": {
    "SourceClaimType": "extension_CustomClaim", //Name of the claim custom roles are in
    "RoleClaimtype": "MyCustomRoles",           //Type of the claim to use in the new Identity (works along side of built in)
    "AuthenticationType" : "MyCustomRoleAut"    //If you need to change the Authentication type for new identity
  }
```
And add this to your application startup (ther are other overrides but this is recommended)
```C#
builder.Services.AddClaimToRoleTransformer(builder.Configuration, "ClaimToRoleTransformer");
```

## Route based authorization
This component allows you to authorize users based on roles that have a context attached to them which can be different for each user.
For example a user can be in role ``subscription_1.SubscriptionAdmin``` but in your code you would only specify required role
  
### To enable:
Add the following to your application startup, which will let you use the ```subscriptionId``` component of the route as your context.
```C#
builder.Services.AddRouteBasedRoleHandler("subscriptionId");
```

At this point you can use either policies or _Role_ property of the _Authorize_ attribute as you would normally use them. 
For methods that have a _subscriptionId_ in them such as  ```/subscriptions/{subscriptionId}/todoitems``` the role is __also__ evaluated 
as if it had the value of _subscriptionId_ as a prefix.


Now you can write policies that allow you to write context based roles.

For example given the following policy for a subscription with ID ```subscription_1```:
```C#
builder.Services.AddAuthorization(options => {
    options.AddPolicy("SubscriptionAdminOnly", policyBuilder =>
    {
        policyBuilder.Requirements.Add(new RolesAuthorizationRequirement(new string[] { "SubscriptionAdmin" }));
    });
});
```  
And this action method:
```C#
[Authorize(Policy = "SubscriptionAdminOnly")]
[Route("subscriptions/{subscriptionId}/users")]
public IActionResult GetUsersSubAdminPolicy(string subscriptionId)
{
    return View();
}
```
This call _http://contoso.com/api/subscriptions/subscription_1/users_ will allow only users with ```subscription_1.SubscriptionAdmin``` role to access this view.

Similarly you can write the previous example using the _Authorize_ attribute's _Roles_ property:

For example given the following registeration for a subscriptionwith ID ```subscription_2```:
```C#
builder.Services.AddRouteBasedRoleHandler("subscriptionId");
```
And this action method:
```C#
[Authorize(Roles = "SystemAdmin, SubscriptionAdmin")]
[Route("subscriptions/{subscriptionId}/Admins")]
public IActionResult GetUsersAdmin(string subscriptionId)
{
    return View();
}
```
Will allow only users with ```subscription_2.SubscriptionAdmin``` __or__ ```subscription_2.SystemAdmin``` __or__ ```SubscriptionAdmin``` __or__ ```SystemAdmin``` 
to access the view.

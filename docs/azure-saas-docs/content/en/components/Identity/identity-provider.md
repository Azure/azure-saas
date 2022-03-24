---
type: docs
title: "Identity Provider"
weight: 50
---

- The identity provider default implementation is [Azure B2C](https://docs.microsoft.com/en-us/azure/active-directory-b2c/overview).
- If your scenario uses [Azure AAD](https://azure.microsoft.com/en-us/services/active-directory/), you can swap out the **Identity Provider** in the diagram below.
- B2C is offering permissions as a service.

```mermaid
graph
	user("fa:fa-user Contoso Business Admin")
	adminweb("Saas.Admin.Web")
	identityapi("fa:fa-key <b>Identity Provider</b>")
	catalogapi("Saas.Catalog.Api")
	catalogsql[(Saas.Catalog.Sql)]

	user-- Bearer Token -->adminweb
	adminweb-->user

	adminweb-- Token -->catalogapi
	catalogapi-->adminweb

	catalogapi-->identityapi

	catalogapi-- EF CRUD -->catalogsql
```
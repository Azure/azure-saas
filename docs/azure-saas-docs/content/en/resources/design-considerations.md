---
type: docs
title: "Design Considerations"
weight: 40
---


Multitenancy model, how we landed on it. Scenarios that this would and wouldn't support. How you might change the multitenancy model. 

This solution uses various `appsettings.json` and `appsettings.development.json` to manage deployment settings and other configuration settings. In a true production deployment, you should configure [Azure Key Vault](https://azure.microsoft.com/en-us/services/key-vault/) to provide a secure and centralized key storage location. This allows applications to not have direct access to confidential keys.
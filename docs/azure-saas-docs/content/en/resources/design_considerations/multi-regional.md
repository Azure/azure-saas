---
type: docs
title: "Multi-Regional Deployment"
linkTitle: "Multi-Regional Deployment" 
weight: 11
---
There are many reasons you may choose to deploy your SaaS application in multiple regions at once. This may include [reliability](https://docs.microsoft.com/en-us/azure/architecture/framework/resiliency/overview), scalability for high throughput scenarios, or to meet data residency requirements for customers in multiple geographies.

Deploying any application in multiple regions with multiple sets of infrastructure introduces a lot of complexity and cost. It is recommended to consider this only when the requirements of your application deem it necessary.

## Considerations

### Data Storage

The biggest consideration when designing a multi regional application is where your data will be stored. The ASDK project uses SQL Server as its backend data store, which can be deployed in a multi regional format, but will require some thought behind the design first.

#### SQL - Read Replicas

If the reason you're deploying your application to multiple regions is strictly for reliability and performance, you could consider using SQL Read replicas in multiple regions. This is most commonly done for business continuity and disaster recovery purposes, but it provides a scaling benefit as well. Read more about it on the [Active geo-replication](https://docs.microsoft.com/en-us/azure/azure-sql/database/active-geo-replication-overview?view=azuresql) documentation page.

#### Data Sharding

Data sharding is another common practice to solve scalability requirements as well as data residency requirements. 
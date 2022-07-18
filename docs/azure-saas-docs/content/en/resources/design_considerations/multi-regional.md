---
type: docs
title: "Multi-Regional Deployment"
linkTitle: "Multi-Regional Deployment" 
weight: 11
---
# Overview
There are many reasons you may choose to deploy your SaaS application in multiple regions at once. This may include [reliability](https://docs.microsoft.com/en-us/azure/architecture/framework/resiliency/overview), scalability for high throughput scenarios, or to meet data residency requirements for customers in multiple geographies.

Deploying any application in multiple regions with multiple sets of infrastructure introduces a lot of complexity and cost. It is recommended to consider this only when the requirements of your application deem it necessary.

## Considerations

### Data Storage

The biggest consideration when designing a multi regional application is where your data will be stored. The ASDK project uses SQL Server as its backend data store, which can be deployed in a multi regional format, but will require some thought behind the design first.

#### SQL - Read Replicas

If the reason you're deploying your application to multiple regions is strictly for reliability and performance, you could consider using SQL Read replicas in multiple regions. This is most commonly done for business continuity and disaster recovery purposes, but it provides a scaling benefit as well. Read more about it on the [Active geo-replication](https://docs.microsoft.com/en-us/azure/azure-sql/database/active-geo-replication-overview?view=azuresql) documentation page.

#### NoSQL

Another option for enabling high reliability and high throughput for your data storage across multiple regions is switching to a NoSQL database. This is also a highly complex topic with a lot of important decisions and considerations. Check out the [Relational vs. NoSQL data](https://docs.microsoft.com/en-us/dotnet/architecture/cloud-native/relational-vs-nosql-data) comparison here.
#### Data Sharding

Data sharding is another common practice to solve scalability requirements as well as data residency requirements. Data sharding is a technique used to distribute large amounts of data across many databases. There are many different patterns used to distribute this data, but here are some examples:

- Separate customer databases (ie some or all customers get their own database)
- Database per region or geography

An important consideration for this is to ensure that your distribution strategy maximizes cost efficiency while minimizing "hot spots" in the load. This is a more complicated strategy that requires some planning and changes at the application layer as well. Read more about this approach on the [multitenancy documentaton](https://docs.microsoft.com/en-us/azure/architecture/guide/multitenant/considerations/tenancy-models#vertically-partitioned-deployments) as well as the [Azure SQL documentation](https://docs.microsoft.com/en-us/azure/azure-sql/database/elastic-scale-introduction?view=azuresql).
# Azure SaaS Dev Kit User Guide

The user guide is built using markdown files generated using [Hugo](https://gohugo.io/) and [Docsy](https://www.docsy.dev/) Template.  To get started developing the User Guide site, locally:

- [Install Hugo](https://gohugo.io/getting-started/) - Note: You'll need to pick the "extended" version if you're on Windows.
- Run `npm install postcss-cli`
- Change to the /docs/azure-saas-docs folder
- Start the Hugo Server 

```
    hugo server
```

Docsy builds out navigation using the directory structure built in the content/en directory.  So to add new content for the site, you just create a new markdown page in the corresponding folder. At the top of the content page, add the following

```
---
type: docs
title: "[Page Title]"
linkTitle: "[Link Title to show on the navigation bar]"
weight: [Number indicating the order of the page in the navigation bar]
description: "[Description of the page]"
---
```
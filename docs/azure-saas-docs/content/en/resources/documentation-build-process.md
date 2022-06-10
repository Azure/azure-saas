---
type: docs
title: "Documentation Build Process"
weight: 10
---

Documentation for the site is generated using [Hugo](https://gohugo.io/) and [Docksy](https://www.docsy.dev/). Hugo is a static site generator with Docksy providing a default theme and auto-generated navigation to make reviewing the documentation easier.  These make use of structured Markdown (.md) files organized in nested directories to create a simple site. This site is built and deployed by a GitHub workflow action executing [docs-build.yml](https://github.com/Azure/azure-saas/blob/main/.github/workflows/docs-build.yml). The site is deployed via the [pages-build-deployment](https://github.com/Azure/azure-saas/actions/workflows/pages/pages-build-deployment) workflow.

| Variable     | Section | Description                                                                    | Default |
| ------------ | ------- | ------------------------------------------------------------------------------ | ------- |
| github_token | Deploy  | Secret token necessary to give the bot access to publish the document pages    |         |
| publish_dir  | Deploy  | Path to published directory that will be deployed                              |         |

## Further Reading

[Publishing a Hugo Site](https://docs.microsoft.com/en-us/azure/static-web-apps/publish-hugo) - Manual setup of a Hugo static pages on Azure
[GitHub Page Deploy](https://docs.github.com/en/pages/getting-started-with-github-pages) - Documentation of GitHub functionality to automate page deployment
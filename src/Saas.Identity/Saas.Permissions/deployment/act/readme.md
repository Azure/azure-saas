# Saving Time Running Local GitHub Actions

GitHub actions are terrific for CI/CD automated deployment. It the *inner loop* for getting GitHub actions right can be a tedious affair - i.e., having to commit, push and run when testing and troubleshoot.

Luckily, there a solution for this called [act](https://github.com/nektos/act). Act lets you run the a GitHub running locally in a container that mimics what is running in GitHub. You still have to commit your latest code to GitHub, as act will pull it from there when it runs. However, you don't have to commit and push every time you make a change to the GitHub action workflow. This last part can save a lot of time and avoid all this *testing*, *wip* etc. commit and pushes to you main branch. It also allows you to have a slightly different `workflow.yml` file that pulls from for instance your dev branch rather than your main branch.  



... bla. bla.
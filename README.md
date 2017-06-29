# Press Release
This is a website for creating release notes by collating relevant information from GitHub. It is also a learning exercise.

## Building Press Release
The build chain uses the `dotnet` CLI, `gulp`, and `yarn`, with `browserify` and `flow`.

1. Install `yarn` and `dotnet`
1. Run `dotnet restore` to restore packages
    - NOTE: Parts of the following build step use `yarn run` to run tools like `gulp` and `flow` from a project's `node_modules` folder. To ensure you are running the intended packages, please make sure there is no `node_modules` folder prior to restoring packages.
1. Run `dotnet build` to build the solution

## Development
Press Release has been developed using Visual Studio Code, ASP.NET Core, and React.

### Visual Studio Code
#### Flow Types
If you are using Visual Studio Code, install the Flow extension and run `yarn global add flow-bin`. The extension can be used against a local project's installation of `flow-bin`, but that was not done by default in order to avoid any nasty security issue that some malevolent individuals may exploit.

#### Watch
Regular builds will always process the cliet-side files. However, if the dotnet site is being served (in or outside of the debugger), the client-side files can regularly be updated such that refreshing the browser will show the changes. To enable watching, just run `yarn do:watch` in the `PressRelease` sub-folder.

## Running Press Release locally
ASP.NET Core sites run on Linux, OS/X, or Windows using the `dotnet` command line tools.

1. Configure the GitHub client information
1. Use the `dotnet user-secrets` command to add your `github:clientid` and `github:clientsecret` secrets
1. - Either
        1. Use the instructions [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments#setting-the-environment) to set the hosting environment to `Development`
        1. In the command line, navigate to the `PressRelease` sub-folder (the same folder as the `PressRelease.csproj`)
        1. Run `dotnet run`
        1. Navigate your browser to `http://localhost:5000`
    - or
        1. Press `F5` or `Ctrl+F5` inside Visual Studio Code


NOTE: In order to run, you may need to either:

- Modify the code to disable Application Insights (`TelemetryConfiguration.Active.DisableTelemetry = true`)

-or-
- Add a user-secret for `ApplicationInsights:InstrumentationKey`, setting it to your Application Insights instrumentation key.

## Authors
This project is authored by:
- [Jeff Yates](https://github.com/somewhatabstract)
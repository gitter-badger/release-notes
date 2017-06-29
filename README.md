# Release Notes
This is a website for creating release notes by collating relevant information from GitHub. It is also a learning exercise.

## Building Release Notes
The build chain uses the `dotnet` CLI, `gulp`, and `yarn`, with `browserify` and `flow`.

1. Install `yarn` and `dotnet`
1. Run `dotnet restore` to restore packages
    - NOTE: Parts of the following build step use `yarn run` to run tools like `gulp` and `flow` from a project's `node_modules` folder. To ensure you are running the intended packages, please make sure there is no `node_modules` folder prior to restoring packages.
1. Run `dotnet build` to build the solution

## Development
Release Notes has been developed using Visual Studio Code, ASP.NET Core, and React.

### Visual Studio Code
#### Flow Types
If you are using Visual Studio Code, install the Flow extension and run `yarn global add flow-bin`. The extension can be used against a local project's installation of `flow-bin`, but that was not done by default in order to avoid any nasty security issue that some malevolent individuals may exploit.

#### Watch and Live Reload
Updating everything with a build is tedious when developing. To make things nicer, there are two ways to watch the app.

1. Full watch of both server- and client-side parts
    - Generally, use this option. It watches everything and will auto-refresh the browser if the server DLL or client-side files change.
    1. From solution folder, `cd PressRelease`
    1. `yarn do:watch`
    1. Browse to `localhost:3000`
        - You can also browse to `localhost:3001` to use BrowserSync features
1. Client-side only
    - Use this option if you only intend to play around with the client-side code against an unchanging backend
    1. From solution folder, `cd PressRelease`
    1. `yarn do:watchclientonly`
    1. Browse to `localhost:3000`
        - You can also browse to `localhost:3001` to use BrowserSync features
1. Server and client in separate terminals
    - This is the same as the first option, except that the output of client-side and server-side watches can be run in independent terminals
    1. From solution folder, `cd PressRelease`
    1. In one terminal, run `yarn do:watchserver`
    1. In another terminal, run `yarn do:watchclient`
    1. Browse to `localhost:3000`
        - You can also browse to `localhost:3001` to use BrowserSync features


## Running Release Notes locally
ASP.NET Core sites run on Linux, OS/X, or Windows using the `dotnet` command line tools.

1. Configure the GitHub client information
1. Use the `dotnet user-secrets` command to add your `github:clientid` and `github:clientsecret` secrets
1. - Do
        1. `cd PressRelease`
        1. `yarn do:runasdev`
        1. Navigate your browser to `http://localhost:5000`
    - or
        1. Open solution folder in Visual Studio Code
        1. Press `F5` or `Ctrl+F5`
    - or
        1. Use the instructions [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments#setting-the-environment) to set the hosting environment to `Development`
        1. In the command line, navigate to the `PressRelease` sub-folder (the same folder as the `PressRelease.csproj`)
        1. Run `dotnet run`
        1. Navigate your browser to `http://localhost:5000`


NOTE: In order to run, you may need to either:

- Modify the code to disable Application Insights (`TelemetryConfiguration.Active.DisableTelemetry = true`)

-or-
- Add a user-secret for `ApplicationInsights:InstrumentationKey`, setting it to your Application Insights instrumentation key.

## Authors
This project is authored by:
- [Jeff Yates](https://github.com/somewhatabstract)
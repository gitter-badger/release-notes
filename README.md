# Press Release

[![Join the chat at https://gitter.im/release-notes/Lobby](https://badges.gitter.im/release-notes/Lobby.svg)](https://gitter.im/release-notes/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
This is a website for creating release notes by collating relevant information from GitHub.


## Development
Press Release has been developed using Visual Studio Code and ASP.NET Core.

The development is somewhat of a learning exercise. Portions of the front end may well end up using React or some other client-side framework.

## Building Press Release
The build chain uses the `dotnet` CLI, `gulp`, and `yarn`.

1. Install `yarn`, `gulp` and `dotnet`
1. Run `dotnet restore` to restore packages
1. Run `dotnet build` to build the solution

## Running Press Release locally
ASP.NET Core sites run on Linux, OS/X, or Windows using the `dotnet` command line tools.

1. Configure the GitHub client information
1. Use the `dotnet user-secrets` command to add your `github:clientid` and `github:clientsecret` secrets
1. Use the instructions [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments#setting-the-environment) to set the hosting environment to `Development`
1. In the command line, navigate to the `PressRelease` sub-folder (the same folder as the `PressRelease.csproj`)
1. Run `dotnet run`
1. Navigate your browser to `http://localhost:5000`
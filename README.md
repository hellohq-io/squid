# squid #
**squid - dependency management as a service**

## Overview

`squid` allows you to manage NuGet dependencies of multiple .NET Core projects. It lists the dependencies of the repositories, their latest versions and makes it easier to update those. This is especially useful when you need to manage a lot of projects.

.NET Core project repositories added to `squid` are checked for their dependencies and compared to the public NuGet feed as well as private feeds.

`squid` then allows you to update one or several dependencies of a project to the latest version. `squid` creates a PR, waits for the build to succeed and then automatically merges the PR.

Currently only supports Bitbucket repositories.

## Deployment

Docker tag: `hellohq/squid`

## Configuration

The following configuration options / environment variables are available.

Environment Variable | Description | Default
------------ | ------------- | -------------
`CRYPTO_KEY` | The key used to encrypt passwords in the database. | Required.
`MONGODB_CONNECTION_STRING` | The full MongoDB connection string for the data store. | `mongodb://127.0.0.1:27017`
`MONGODB_DATABASE_NAME` | The name of the database to use. | `squid`
`DEPENDENCY_JOB_INTERVAL` | The name of the branch for dependency updates. | 5
`PULLREQUEST_JOB_INTERVAL` | The name of the branch for dependency updates. | 3
`NUGET_JOB_INTERVAL` | The name of the branch for dependency updates. | 5
`PRIVATEFEED_JOB_INTERVAL` | The name of the branch for dependency updates. | 5

## Setup

Start `squid` like this:
```
docker run
    -e MONGODB_CONNECTION_STRING="mongodb://localhost:27017"
    -e CRYPTO_KEY=y7xsPENyw7j3RCcXvqWJEYZzjZUfUUN8
    -p 5000:5000
    hellohq/squid:latest
```

## Build

```
dotnet build
```

## Build Release Version

```
dotnet publish -c release --version-suffix 1 -o out squid.csproj
```

## ToDo

- [ ] Support github repositories 
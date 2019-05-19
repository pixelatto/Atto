# Atto

Atto is a basic architecture and tools pack for Unity projects. Unity nature usually pushes unexperienced programmers to very coupled code with no structure whatsoever. Atto, while not being the ultimate solution, helps to improve that without being too sophisticated. It is aimed at intermediate users who want to improve their codebase maintainability without running into additional boilerplate.

Atto it's build around these principles:
- **Easy to boot:** You just have to import the package and type `Atto.[Something]` to get started. There's a caveat: Atto sacrifices namespacing in order to get up and running right away.
- **Simple to use:** There are many common services already installed and automatically included.
- **Easy to customize:** It's quite simple to add new services or configure the default ones using the [settings](#settings) file.

## Table of contents

- Basic concepts
  - [Container](#container)
  - [Core](#Core)
  - [Services](#services)
  - [Providers](#providers)
- Installation
  - [Package setup](#package-setup)
  - [Binding](#binding)
  - [Settings](#settings)
- Usage
  - Common services
    - [Logger](#logger-service)
    - [Serializer](#serializer-service)
    - [Channels](#channels-service)
    - [Storage](#storage-service)
    - [Database](#database-service)
    - [Scenes](#scenes-service)
    - [Input](#input-service)
- Additional tools
  - TBD

## Container

This is the center of the Atto architecture, a service container providing various kinds of common fucntionality through your app. Its major advantage is that it enables global access to common services through your app while also providing loose coupling to them (they are all isolated via interfaces).


## Core

This is your main entry point to access the container services. It features some shortcuts to make the access to services less verbose.

Some usage examples:
```
Core.Logger.Log("This is a debug text");

var hero = Core.Serializer.Deserialize("{name: Lonk, job: Adventurer}");
```

You can customize the name of the Core class (e.g: use `C.Logger.Log("This is a debug text")` using the framework [settings](#settings).


## Services

Services are defined via interfaces. The basic setup includes many common services for most use cases. You can include the additional special services from the corresponding folder or even make your own.

They are named starting with an I and ending with the word "Service" (e.g: `ISaveGameService`).


## Providers

Providers are service implementations. 

They are named similarly to the service they implement, with some definition of the specifics of the provide and ending with the word "Provider" (e.g: `LocalSaveGameProvider`).


# Installation

Atto is installed as a Unity package. You can find additional info on setting up custom packages [here](https://gist.github.com/LotteMakesStuff/6e02e0ea303030517a071a1c81eb016e).

## Package setup

Create a new Unity project. In this new project, open the `/Packages/manifest.json` file and include the following entry:
```
{
  "dependencies": {
    "com.pixelatto.atto": "https://github.com/pixelatto/atto.git"
  }
}
```
Save the manifest, go back to Unity, wait for it to refresh (or force with ctrl+R) and... you're ready to go!

**Important note**: Atto is not namespaced, since it's designed to be a starting point for projects and fast/simple to use. It shouldn't be imported in existing codebases.

## Binding

*This step is optional for basic usage.*

By default, the basic services are all binded to the container and available for immediate use. You can Bind additional services using the attribute [BindService] into any class that implements exactly one interface to create and bind a new service. If the interface is named IMyInterfaceService it'll then be available as `Atto.MyInterface` as a static shortcut.

## Settings

After running the compiled package for the first time, a `AttoSettings.json` file will be generated on the 'Assets/Plugins/Atto' folder (hit ctrl+R if you don't see it). This is a JSON file that contains various settings you can tweak to modify the behaviour of the framework.

| Key                      | Values                                 | Description                                                      |
| ----------------         | ----------------                       | ----------------                                                 |
| autoBindCommonServices   | "true", "false"                        | Sets the default boostrapping on startup for all common services.|
| storagePath              | "dataPath", "persistentDataPath", URI  | Sets the base path for the storage service                       |
| dataChannels             | An array of data channel definitions   | Defines data channels for the data channels service              |
| coreName                 | An string                              | Redefines the name of the Core class                             |

The default `AttoSettings.json` file looks like this:
```
{
    "autoBindCommonServices": true,
    "storagePath": "dataPath",
    "dataChannels": [
        {
            "channelName": "Database",
            "channelId": 1,
            "uri": "/Data.sav"
        },
        {
            "channelName": "Options",
            "channelId": 2,
            "uri": "/Options.sav"
        },
        {
            "channelName": "Save",
            "channelId": 3,
            "uri": "/Save.sav"
        },
        {
            "channelName": "Rankings",
            "channelId": 4,
            "uri": "/Rankings.sav"
        }
    ]
}
```


# Usage

## Common services

### Logger service
  This service wraps around logging features, similar to Debug.Log but decoupled from Unity dependencies. The default provider is the `UnityConsoleLogProvider`, which behaves similarly to Unity's `Debug` class. Use `Core.Logger.Log("My message");` instead of `Debug.Log("My message")` if you want to have decoupled logs.

### Serializer service
  This service wraps around serialization features. This allows you to change your serialization engine to suit your project needs. Use `Core.Serialization.Serialize(SomeObject);` or `Core.Serialization.Deserialize("{ Serialized: data }")` to serialize/deserialize strings to/from data objects.
  
### Channels service
  This service wraps around resource locations (URIs, URLs, Keys...) into *Data Channels* to avoid having strings all around your code. If you need to refer to some location on the hard drive or in the cloud, use this service to define a data channel. You'll have to add new entries to the `DataChannelTypes` enum.
  
### Storage service
  This service uses *Data Channels* to store data somewhere. Use `Core.Storage.ReadFromStorage(ChannelType)` to read data and `Core.Storage.WriteToStorage(ChannelType)` to write data.

### Database service
  This service wraps a datbase around a storage service to enable saving entries in a dictionary scheme (i.e: Key->Value). Use `Core.Database.WriteEntry(key, value)` to write an entry or `var value = Core.Database.ReadEntry(key)` to retrieve it. The service is made async by using [Promises](https://github.com/Real-Serious-Games/C-Sharp-Promise), so it can be easily replaced by some remote service (e.g: a Firebase database).

### Scenes service
  This service wraps scene management with scene loading parameters. Specially useful if you want to load scenes with different behaviours depending on data (e.g: play mode vs replay mode). Use `Core.Scenes.LoadScene(Scene, SceneParams)`.

### Input service
  This service wraps input device management. Use this instead of Unity's Input class to avoid coupling input logic to Unity's default input system so you can change to a more powerful framework later (e.g: InControl, Rewired...)
  
## Special services
  TBD
  
# Future additions

Feel free to request new services or submit your own ones via pull request.

I hope you find it useful!

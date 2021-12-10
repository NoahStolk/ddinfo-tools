# DevilDaggersInfo

DevilDaggersInfo is a website created specifically for the game Devil Daggers. It is hosted on [devildaggers.info](https://devildaggers.info/).

Special thanks to the community for all the support throughout the years, and to the developer for making the game, as well as continuing to update the game to make working with its internals a lot easier.

## Structure Of Projects

The project is written in C# and currently runs on .NET 6.0. There are multiple types of projects in the repository:

### DevilDaggersInfo.Cmd

These are simple command line applications that can be used to communicate with certain core functionalities, such as compiling multiple assets into a binary file that the game can read.

### DevilDaggersInfo.Core

These are class libraries that can be referenced by any other project. They are separated appropriately and only deal with a specific kind of base functionality that should be usable in any other project.

### DevilDaggersInfo.SourceGen

These are source generators that generate source code for other projects at compile time. They generate source code based on other source code, or based on data files.

### DevilDaggersInfo.Test

These are unit test projects that test the functionalities of other projects. They usually target a core functionality such as parsing a spawnset or mod file.

### DevilDaggersInfo.Web

These are the projects that make up the website. The full web application consists of three main projects, `Client`, `Server`, and `Shared`.

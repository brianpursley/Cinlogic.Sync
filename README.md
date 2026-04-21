# Cinlogic.Sync

Cinlogic.Sync is a C# library that provides an easy way to ensure an action or function is executed only once in a multi-threaded environment.

It is inspired by Go's [`sync.Once`](https://pkg.go.dev/sync#Once) type.

## Getting Started

### Installation

This library is published on NuGet. To install it, you can use the following command in the package manager console:

```shell
Install-Package Cinlogic.Sync
```

Or via the .NET CLI:
```shell
dotnet add package Cinlogic.Sync
```

## Usage

To use the `Once` class in your project:

```csharp
var once = new Once();
var counter = 0;

for (int i = 0; i < 1000; i++)
{
    // Despite this loop running 1000 times, the action will only be executed once.
    once.Do(() => counter++);
}

Console.WriteLine(counter); // Outputs: 1
```

# Features
* Ensures that a function or an action is executed only once, even in a multithreaded environment.
* Allows for exception handling in the Do method.
* Provides async support with the DoAsync method.
* Supports cancellation tokens for async operations.

# Examples
Please see the tests for more examples of how to use the Once class.

# License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

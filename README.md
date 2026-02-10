# PackageInstaller
Description
You suddenly have a curious aspiration to create a package installer that can handle
dependencies. You want to be able to give the installer a list of packages with dependencies,
and have it install the packages in order such that an install won’t fail due to a missing
dependency.
This exercise is to write the code that will determine the order of install.

# PackageDependencyInstaller (C#)

A C# utility that determines the installation order for packages based on their dependencies.  
It also detects cycles in dependencies and prevents invalid installation orders.

This project demonstrates **dependency resolution, cycle detection, and topological ordering** in C#.

---

## 🛠 Features

- Determines installation order for packages respecting dependencies
- Detects cycles in the dependency graph and throws meaningful exceptions
- Handles packages with no dependencies
- Provides a clean, comma-separated output of installable packages
- Built with **clean code principles** and proper validation

---

## 💻 Example Usage

```csharp
string[] dependencies = new string[]
{
    "A: B",
    "B: C",
    "C: ",
    "D: B"
};

var installer = new PackageDependencyInstaller.PackageInstaller();
string installOrder = installer.PrintDependencies(dependencies);

Console.WriteLine("Installation order:");
Console.WriteLine(installOrder);


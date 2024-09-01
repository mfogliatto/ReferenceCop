# ReferenceCop
ReferenceCop is a tool that allows .NET teams to enforce dependency management rules at build time and prevent certain references from being added to a project.

## Usage

To enable ReferenceCop in your project, follow the steps below:

1. Add ReferenceCop as a `PackageReference` to your project.

In case you are using [Central Package Management](https://learn.microsoft.com/en-us/nuget/consume-packages/Central-Package-Management), you can add ReferenceCop as a `GlobalPackageReference` in your Directory.Packages.Props file.

```
<ItemGroup>
	<GlobalPackageReference Include="ReferenceCop" Version="{LatestVersion}" />
</ItemGroup>
```

2. Add a `ReferenceCop.config.json` file to your project:

```
<ItemGroup>
	<AdditionalFiles Include="./ReferenceCop.config.json" />
</ItemGroup>

```

3. Build the project. You will see a set of warnings or errors based on the violations that were detected. Here are the diagnostic descriptors returned:

## Configuration

The ReferenceCop.config.json file defines a set of rules to be enforced. The config file should be added to the project as an `AdditionalFile`.

### Rule Properties
- `Name` - A unique name for the rule.
- `Description` - A description of the rule.
- `Pattern` - The pattern to search for in the project's references. It can be either an exact reference AssemblyName,
or a prefix pattern that ends with '*'.
- `Severity` - The severity of the rule violation. Can be `error` or `warning`.

### Example

Here is an example of a `ReferenceCop.config.json` file:

```json
{
  "Rules": [
    {
      "Name": "DoNotUse-X",
      "Description": "Use of X is forbidden. Please use Y instead",
      "Pattern":  "X",
      "Severity": "Error"
    },
    {
      "Name": "AvoidWhenPossible-X.*",
      "Description": "It is discouraged to use X and its packages. Consider using Y instead",
      "Pattern": "X.*",
      "Severity": "Warning"
    }
  ]
}
```

### Diagnostic Descriptors

RC0001| "Illegal references" | Error
RC0002| "Discouraged references" | Warning


### Scaling Configuration

Alternatively, and in case you'd like to apply the config to several projects in a directory, you can do the same in a `Directory.Build.props` file by [Customizing the build by folder](https://learn.microsoft.com/en-us/visualstudio/msbuild/customize-by-directory?view=vs-2022).

This file will contain the rules you want to enforce. The contents of this configuration are explained in the section below.


# License

ReferenceCop is licensed under the MIT license. See LICENSE for more information.
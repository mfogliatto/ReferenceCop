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

2. Add a `ReferenceCop.config` file to your project and define the repository root for ReferenceCop to use:

```
<ItemGroup>
	<AdditionalFiles Include="./ReferenceCop.config" />
</ItemGroup>

<PropertyGroup>
    <ReferenceCopRepositoryRoot>$(MSBuildStartupDirectory)</ReferenceCopRepositoryRoot>
</PropertyGroup>

```

The `ReferenceCopRepositoryRoot` property is used to define the root of the repository where the project is located. This is used to resolve the paths of the project references.
The recommended value for `ReferenceCopRepositoryRoot` is `$(MSBuildStartupDirectory)` as this will typically be the root of the solution where the project is located.
If this does not work for your project, you can set the value to a different path.

3. Build the project. You will see a set of warnings or errors based on the violations that were detected. Here are the diagnostic descriptors returned:

## Configuration

The ReferenceCop.config file defines a set of rules to be enforced. The config file should be added to the project as an `AdditionalFile`.

### Rule Properties

All rules have the following properties:

- `Name` - A unique name for the rule.
- `Description` - A description of the rule.
- `Severity` - The severity of the rule violation. Can be `error` or `warning`.

Besides the common properties, each rule can have the following properties depending on the type of rule:

#### `AssemblyName`

- `Pattern` - The pattern to search for in the name of a project or package reference. It can be either an exact reference AssemblyName,
or a prefix pattern that ends with '*'.

#### `Assembly Path` (Not implemented yet)

- `Pattern` - The pattern to search for in the path of a project references. The value should be relative to the path where the config file is located,
and it can be either an exact path or a prefix pattern that ends with '*'.

#### `Assembly Tag` (Not implemented yet)

- `Tag` - The tag to search for in the project's manifest that qualifies the project in some way. For example, a tag can indicate
the layer to which the project belongs: `UI`, `Business`, `Data`, etc.

### Example

Here is an example of a `ReferenceCop.config` file:

```xml
<ReferenceCopConfig>
    <Rules>
        <AssemblyName>
            <Name>XMustNotBeUsed</Name>
            <Description>Use of X is forbidden. Ensure that Y is used instead</Description>       
            <Severity>Error</Severity>
            <Pattern>X</Pattern>
        </AssemblyName>
        <AssemblyName>
			<Name>TryToAvoidLibraryX</Name>
			<Description>Consider using Y instead of X</Description>
			<Severity>Warning</Severity>
            <Pattern>X</Pattern>
        </AssemblyName>
        <AssemblyTag>
            <Name>AbstractionsShouldNotReferenceImplementations</Name>
            <Description>Ensure Abstractions do not reference Implementations</Description>
            <Severity>Error</Severity>
            <FromAssemblyTag>Abstractions</FromAssemblyTag>
            <ToAssemblyTag>Implementations</ToAssemblyTag>
        </AssemblyTag>
        <AssemblyPath>
            <Name>AppShouldNotReferenceData</Name>
            <Description>Ensure UI projects do not reference Data projects</Description>
            <Severity>Error</Severity>
            <FromPath>src\UI</FromPath>
            <ToPath>src\data</ToPath>
        </AssemblyPath>
    </Rules>
</ReferenceCopConfig>
```

### Diagnostic Descriptors

RC0001| "Illegal references" | Error
RC0002| "Discouraged references" | Warning

### Scaling Configuration

Alternatively, and in case you'd like to apply the config to several projects in a directory, you can do the same in a `Directory.Build.props` file by [Customizing the build by folder](https://learn.microsoft.com/en-us/visualstudio/msbuild/customize-by-directory?view=vs-2022).

This file will contain the rules you want to enforce. The contents of this configuration are explained in the section below.

# License

ReferenceCop is licensed under the MIT license. See LICENSE for more information.
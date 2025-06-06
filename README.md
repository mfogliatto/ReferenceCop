# ReferenceCop
ReferenceCop is a suite of tools (including a Roslyn analyzer and an MSBuild task) that work together to help .NET teams 
enforce agreed-upon dependency management rules at build time. Prevents discouraged or prohibited references from being 
added to projects, maintaining clean architecture as defined by your team.

![Build Status](https://github.com/mfogliatto/ReferenceCop/actions/workflows/official-build.yaml/badge.svg)
![NuGet Version](https://img.shields.io/nuget/v/ReferenceCop.svg)

## Usage

To enable ReferenceCop in your project, follow the steps below:

1. Add ReferenceCop as a `PackageReference` to your project.

In case you are using [Central Package Management](https://learn.microsoft.com/en-us/nuget/consume-packages/Central-Package-Management), you can add ReferenceCop as a `GlobalPackageReference` in your Directory.Packages.Props file.

```
<ItemGroup>
    <GlobalPackageReference Include="ReferenceCop" Version="{LatestVersion}" />
</ItemGroup>
```

2. Add a `ReferenceCop.config` file to your project and define the repository root for ReferenceCop to use. Please make sure
the add the respective `ReferenceCopConfig` Label node:

```
<ItemGroup>
    <AdditionalFiles Include="./ReferenceCop.config">
        <Label>ReferenceCopConfig</Label>
    </AdditionalFiles>
</ItemGroup>
```

3. Define the `ReferenceCopRepositoryRoot` property. This property defines the root of the repository where the projects are located
and is used to resolve the paths of project references.
The recommended way to set `ReferenceCopRepositoryRoot` is to define it in a Directory.Build.props file placed at the root of your repository:
 
```
<PropertyGroup>
    <ReferenceCopRepositoryRoot>$(MSBuildThisFileDirectory)</ReferenceCopRepositoryRoot>
</PropertyGroup>
```

4. Build the project. You will see a set of warnings or errors based on the violations that were detected. Here are the diagnostic descriptors returned:

## Configuration

The ReferenceCop.config file defines a set of rules to be enforced. The config file should be added to the project as an `AdditionalFile`.
This file will contain the rules you want to enforce. The contents of this configuration are explained in the section below.

### Rule Properties

All rules have the following properties:

- `Name` - A unique name for the rule.
- `Description` - A description of the rule.
- `Severity` - The severity of the rule violation. Can be `error` or `warning`.

Besides the common properties, each rule can have the following properties depending on the type of rule:

#### `AssemblyName`

- `Pattern` - The pattern to search for in the name of a project or package reference. It can be either an exact reference AssemblyName,
or a prefix pattern that ends with '*'. This can be either a project reference or a package reference.

#### `ProjectPath`

- `Pattern` - The pattern to search for in the path of a project references. The value should be relative to the path where the config file is located,
and it can be either an exact path or a prefix pattern that ends with '*'.

#### `ProjectTag`

- `Tag` - The tag to search for in the project's `csproj` file that qualifies the project in some way. ReferenceCop will look for the `ProjectTag` node for this detection. 
- For example, a tag can indicate the layer that the project belongs to: `UI`, `Business`, `Data`, etc.

### Example

Here is an example of a `ReferenceCop.config` file:

```xml
<ReferenceCopConfig xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="https://raw.githubusercontent.com/mfogliatto/ReferenceCop/main/ReferenceCopConfig.xsd">
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
        <ProjectTag>
            <Name>AbstractionsShouldNotReferenceImplementations</Name>
            <Description>Ensure Abstractions do not reference Implementations</Description>
            <Severity>Error</Severity>
            <FromProjectTag>Abstractions</FromProjectTag>
            <ToProjectTag>Implementations</ToProjectTag>
        </ProjectTag>
        <ProjectPath>
            <Name>AppShouldNotReferenceData</Name>
            <Description>Ensure UI projects do not reference Data projects</Description>
            <Severity>Error</Severity>
            <FromPath>src\UI</FromPath>
            <ToPath>src\data</ToPath>
        </ProjectPath>
    </Rules>
</ReferenceCopConfig>
```

### Diagnostic Descriptors

| Code  | Description              | Severity |
|-------|--------------------------|----------|
| RC0001| Illegal references       | Error    |
| RC0002| Discouraged references   | Warning  |

### Scaling Configuration

In case you'd like to enable ReferenceCop and apply the same configuration to multiple projects in a directory, you can do that in a `Directory.Build.props` file by [Customizing the build by folder](https://learn.microsoft.com/en-us/visualstudio/msbuild/customize-by-directory?view=vs-2022).

## License

ReferenceCop is licensed under the MIT license. See LICENSE for more information.

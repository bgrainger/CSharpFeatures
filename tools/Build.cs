#!/usr/bin/env dotnet run

// dotnet run .\tools\Build.cs -- build
// dotnet .\tools\Build.cs build

#: package Faithlife.Build@5.*

return BuildRunner.Execute(args, build =>
{
	build.AddDotNetTargets();
});

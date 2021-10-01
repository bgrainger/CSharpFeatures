
// using System.Text comes from GlobalUsings.cs
var stringBuilder = new StringBuilder();

// using System.Collections.Generic comes from Microsoft.NET.Sdk and <ImplicitUsings>enable</ImplicitUsings> in Project
// stored in obj\Debug\net6.0\CSharp10.GlobalUsings.g.cs
// list of namespaces at https://docs.microsoft.com/en-us/dotnet/core/compatibility/sdk/6.0/implicit-namespaces-rc1#new-behavior
var list = new List<string>();

// using comes from <Using Include="System.Collections.ObjectModel"/> in Project
ReadOnlyCollection<string> readOnly = list.AsReadOnly();

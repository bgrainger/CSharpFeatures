// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

// dotnet run -c Release --project CSharp15
var runtimeAsync = new CSharp15.RuntimeAsync();
await runtimeAsync.StackTraces();

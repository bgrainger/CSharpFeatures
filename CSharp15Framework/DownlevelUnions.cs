using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSharp15Framework
{
	// the other C# 15 features are compiler-only, so they work on .NET Framework, too
	// unions and closed hierarchies just need a few types the compiler looks for (see bottom of file)

	internal union DocumentSource(FileInfo, string);

	internal closed record class CalculatorOperation;
	internal sealed record class Add(double Left, double Right) : CalculatorOperation;
	internal sealed record class Multiply(double Left, double Right) : CalculatorOperation;

	internal static class SequenceExtensions
	{
		extension<T>(IEnumerable<T> source)
		{
			public T this[int index] => source.ElementAt(index);
		}
	}

	internal class DownlevelUnions
	{
		public void Unions()
		{
			DocumentSource source = new FileInfo("notes.txt");
			Console.WriteLine(source switch
			{
				FileInfo file => file.Name,
				string text => text,
			}); // notes.txt
		}

		public void ClosedHierarchies()
		{
			CalculatorOperation operation = new Multiply(6, 7);
			Console.WriteLine(operation switch
			{
				Add(var left, var right) => left + right,
				Multiply(var left, var right) => left * right,
			}); // 42
		}

		public void CollectionExpressionArguments()
		{
			HashSet<string> set = [with(StringComparer.OrdinalIgnoreCase), "a", "A"];
			Console.WriteLine(set.Count); // 1
		}

		public void ExtensionIndexers()
		{
			IEnumerable<int> numbers = new[] { 1, 2, 3 };
			Console.WriteLine(numbers[1]); // 2
		}
	}
}

namespace System.Runtime.CompilerServices
{
	// records
	internal static class IsExternalInit { }

	// unions
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
	internal sealed class UnionAttribute : Attribute { }

	internal interface IUnion
	{
		object? Value { get; }
	}

	// closed hierarchies
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	internal sealed class IsClosedTypeAttribute : Attribute { }

	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
	internal sealed class CompilerFeatureRequiredAttribute(string featureName) : Attribute
	{
		public string FeatureName { get; } = featureName;
		public bool IsOptional { get; init; }
	}
}

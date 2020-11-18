#nullable enable
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CSharp9Framework
{
	public class NullableReferenceTypes
	{
		public static T? FirstOrDefault<T>(IEnumerable<T> source)
		{
			using var enumerator = source.GetEnumerator();
			return enumerator.MoveNext() ? enumerator.Current : default;
		}

		public void Test()
		{
			string? s = FirstOrDefault(new string[0]);
			int i = FirstOrDefault(new int[0]);
			int? j = FirstOrDefault(new int?[0]);
		}
	}

	public class Base
	{
		[return: MaybeNull]
		public virtual T Method<T>() => default;
	}

	public class Derived : Base
	{
		public override T? Method<T>() where T : default => default;
	}
}


namespace System.Diagnostics.CodeAnalysis
{
	/// <summary>
	///     Specifies that an output may be <see langword="null"/> even if the
	///     corresponding type disallows it.
	/// </summary>
	[AttributeUsage(
		AttributeTargets.Field | AttributeTargets.Parameter |
		AttributeTargets.Property | AttributeTargets.ReturnValue,
		Inherited = false
	)]
	internal sealed class MaybeNullAttribute : Attribute
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="MaybeNullAttribute"/> class.
		/// </summary>
		public MaybeNullAttribute() { }
	}
}

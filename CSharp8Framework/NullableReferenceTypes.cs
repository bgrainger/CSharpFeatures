#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CSharp8Framework
{
	public class NullableReferenceTypes
	{
		// warning CS8603: Possible null reference return.
		public string GetString() => null;

		public string? GetNullableString() => null;

		public void FlowAnalysis()
		{
			var str = GetNullableString();

			// warning CS8602: Dereference of a possibly null reference.
			Console.WriteLine(str.Length);

			if (str is null)
				str = "value";

			// no warning
			Console.WriteLine(str.Length);
		}

		public int ArgumentValidation(string s)
		{
			if (s is null)
				throw new ArgumentNullException(nameof(s));

			return s.Length;
		}

		public void ComposingTypes()
		{
			List<string> nonNullableListOfNonNullableString = new List<string> { "test" };
			List<string>? nullableListOfNonNullableString = null;
			List<string?> nonNullableListOfNullableString = new List<string?> { null };
			List<string?>? nullableListOfNullableString = DateTime.Now.Month == 1 ? null : new List<string?> { null };

			string[]? nullableArray = null;
			string?[] arrayOfNullableString = new string?[] { null };
			string?[]? nullableArrayOfNullableString = nullableListOfNullableString?.ToArray();
		}

		[return: MaybeNull]
		public static T FirstOrDefault<T>(IEnumerable<T> source)
		{
			using var enumerator = source.GetEnumerator();
			return enumerator.MoveNext() ? enumerator.Current : default;
		}

		public static T? FirstOrDefault2<T>(IEnumerable<T?> source) where T : struct
		{
			using var enumerator = source.GetEnumerator();
			return enumerator.MoveNext() ? enumerator.Current : default;
		}

		public static T? FirstOrDefault2<T>(IEnumerable<T?> source) where T : class
		{
			using var enumerator = source.GetEnumerator();
			return enumerator.MoveNext() ? enumerator.Current : default;
		}

		public static bool Any<T>(IEnumerable<T> source, Func<T, bool> test)
			where T : notnull
		{
			foreach (var t in source)
				if (test(t))
					return true;
			return false;
		}
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

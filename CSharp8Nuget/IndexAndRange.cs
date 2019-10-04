using System;
using System.Runtime.CompilerServices;

namespace CSharp8Nuget
{
	public class IndexAndRange
	{
		public string[] Words = new[] { "the", "quick", "brown", "fox" };

		public void Index()
		{
			var the = Words[0];
			var fox = Words[^1];
			the = Words[^4];

			// indexes are ^length (first element), ... ^1 (last element), ^0 (one past end)
		}

		public void ArrayRange()
		{
			// ranges are half-open
			var range = Words[0..4];

			// can use ^0 (one past the end) as shorthand
			range = Words[0..^0];

			// only has one element: "quick"
			// range is two indexes, not index+length
			range = Words[1..2];

			// this does actually create a new array, not a lightweight "slice"
			var newArray = Words[1..^1]; // newArray is string[]

			// use .AsSpan() first to be more efficient
			var slice = Words.AsSpan()[1..^1]; // slice is Span<string>

			// DO NOT DO THIS; this creates a temporary array then implicitly converts it to Span<string>
			Span<string> slice2 = Words[1..^1];
		}

		public void StringRange()
		{
			var str = "the quick brown fox";

			var range = str[4..9]; // "quick"

			// beginning or end indexes can be omitted
			range = str[..3]; // "the"
			range = str[16..]; // "fox"
			range = str[..]; // "the quick brown fox";

			Console.WriteLine(object.ReferenceEquals(range, str)); // True

			// never write "collection.Length - 1" again
			var lastCharacter = str[str.Length - 1];
			lastCharacter = str[^1];

			// remove first character
			str = str[1..];

			// remove last character
			str = str[..^1];

			// divide string in thirds
			var length = str.Length / 3;
			var first = str[0..length];
			var second = str[length..^length];
			var third = str[^length..];
		}

		public void Variables()
		{
			Index i = ^0;
			Console.WriteLine(i); // ^0

			// ^0 is syntactic sugar for Index constructor
			i = new Index(0, fromEnd: true);

			Range r = 3..^7;
			Console.WriteLine(r); // 3..^7

			// .. is syntactic sugar for Range constructor
			r = new Range(new Index(3), new Index(7, fromEnd: true));
		}

		public void CustomTypes()
		{
			var collection1 = new Collection1(1, 2, 3);

			// compiler sythesizes call to .Length and Index.GetOffset, then uses indexer
			var x = collection1[^1]; // x = 3

			var collection2 = new Collection2(1, 2, 3);

			// compiler uses provided indexer
			x = collection2[^1]; // x = 3

			var collection3 = new Collection3(1, 2, 3, 4, 5);

			// compiler synthesizes call to .Length then .Slice, using start and length
			var r = collection3[1..^1]; // 2, 3, 4

			var collection4 = new Collection4(1, 2, 3, 4, 5);

			r = collection4[1..^1]; // 2, 3, 4
		}
	}

	public class Collection1
	{
		public Collection1(params int[] data) => m_data = data;

		public int Length => m_data.Length;

		public int this[int index] => m_data[index];

		int[] m_data;
	}

	public class Collection2
	{
		public Collection2(params int[] data) => m_data = data;

		public int Length => m_data.Length;

		public int this[Index index] => m_data[index];

		int[] m_data;
	}

	public class Collection3
	{
		public Collection3(params int[] data) => m_data = data;

		public int Length => m_data.Length;

		public int[] Slice(int offset, int length)
		{
			var result = new int[length];
			Array.Copy(m_data, offset, result, 0, length);
			return result;
		}

		int[] m_data;
	}

	public class Collection4
	{
		public Collection4(params int[] data) => m_data = data;

		public int Length => m_data.Length;

		public int this[Index index] => m_data[index];
		public int[] this[Range range] => m_data[range];

		int[] m_data;
	}
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.CompilerServices
{
	internal static class RuntimeHelpers
	{
		/// <summary>
		/// Slices the specified array using the specified range.
		/// </summary>
		public static T[] GetSubArray<T>(T[] array, Range range)
		{
			if (array == null)
			{
				throw new ArgumentNullException();
			}

			(int offset, int length) = range.GetOffsetAndLength(array.Length);

			if (default(T)! != null || typeof(T[]) == array.GetType()) // TODO-NULLABLE: default(T) == null warning (https://github.com/dotnet/roslyn/issues/34757)
			{
				// We know the type of the array to be exactly T[].

				if (length == 0)
				{
					return Array.Empty<T>();
				}

				var dest = new T[length];
				Array.Copy(array, offset, dest, 0, length);
				return dest;
			}
			else
			{
				// The array is actually a U[] where U:T.
				T[] dest = (T[])Array.CreateInstance(array.GetType().GetElementType()!, length);
				Array.Copy(array, offset, dest, 0, length);
				return dest;
			}
		}
	}
}

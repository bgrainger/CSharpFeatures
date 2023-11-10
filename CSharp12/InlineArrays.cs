using System.Runtime.CompilerServices;

namespace CSharp12;

internal class InlineArrays
{
	// This struct has two members: an int (stored inline) and a reference to a char[] (stored on the heap).
	public struct StructWithArray
	{
		public int Age;
		public char[] Name;
	}

	// Since C# 1.0, you could use "fixed" in unsafe code to store an array inline.
	public unsafe struct UnsafeStructWithArray
	{
		public int Age;

		// The "fixed" type can only be a primitive type: bool, byte, char, int, etc.
		public fixed char Name[50];
	}

	// C# 12 introduces the "[InlineArray]" attribute, which creates fixed-size arrays in safe code.
	[InlineArray(50)]
	public struct Name50
	{
		// Struct must contain a single field (but it can be any non-pointer type).
		private char m_firstCharacter;
	}

	public struct SafeStructWithInlineArray
	{
		public int Age;
		public Name50 Name;
	}

	public void UseInlineArray()
	{
		// can index into the inline array
		var name = new Name50();
		name[0] = 'E';
		name[1] = 'd';

		// error CS9174: Cannot initialize type 'InlineArrays.Name50' with a collection expression because the type is not constructible.
		// Was in C# feature proposal; maybe coming in a future version?
		// Name50 me = ['m', 'e'];

		// index access is compile-time checked
		// error CS9166: Index is outside the bounds of the inline array
		// Console.WriteLine(name[50]);

		// can 'foreach' over the inline array
		foreach (var ch in name)
			Console.Write(ch);

		// implicitly convert to a Span<> to get its length
		Span<char> span = name;
		for (var index = 0; index < span.Length; index++)
			span[index] = char.ToUpper(span[index]);

		// runtime check: throws IndexOutOfRangeException
		Console.WriteLine(span[50]);

		SafeStructWithInlineArray outerStruct = default;
		outerStruct.Name[0] = 'E';
		outerStruct.Name[1] = 'd';

		// can use Index or Range with the InlineArray
		Span<char> substring = outerStruct.Name[3..8];

		// can take a ref to an element of the InlineArray
		ref char first = ref name[0];

		// [InlineArray(2)]
		// internal struct <>y__InlineArray2<T>
		Span<char> compilerWillUseAnInlineArray = ['E', 'd'];
	}
}

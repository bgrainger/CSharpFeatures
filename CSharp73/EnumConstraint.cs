using System;

namespace CSharp73
{
	class EnumConstraint
	{
		// real example: https://github.com/Faithlife/FaithlifeUtility/blob/master/src/Faithlife.Utility/EnumUtility.cs
		public static T[] GetValuesCSharp7<T>()
			// error CS0702: Constraint cannot be special class 'Enum'
			where T : struct
		{
			return (T[]) Enum.GetValues(typeof(T));
		}

		public static T[] GetValues<T>()
			where T : Enum // must use CLR type name, not C# keyword
		{
			return (T[]) Enum.GetValues(typeof(T));
		}
	}
}

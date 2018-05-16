using System;

namespace CSharp73
{
	class DelegateConstraint
	{
		// real example: http://faithlife.codes/blog/2008/07/casting_delegates/
		public static T CastCSharp7<T>(Delegate d)
			// error CS0702: Constraint cannot be special class 'Delegate'
			where T : class
		{
			// cast produces error CS0030: Cannot convert type 'System.Delegate' to 'T'
			return Delegate.CreateDelegate(typeof(T), d.Target, d.Method) as T;
		}

		public static T Cast<T>(Delegate d)
			where T : Delegate // must use CLR type name, not C# keyword
		{
			return (T) Delegate.CreateDelegate(typeof(T), d.Target, d.Method);
		}
	}
}

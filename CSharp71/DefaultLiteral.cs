using System;
using System.Collections.Generic;

namespace CSharp71
{
	public static class DefaultLiteral
	{
		public static void Locals()
		{
			// initialize struct, class, or built-in
			IReadOnlyCollection<string> collection = null;
			int i = 0;
			ValueTuple<string, int> tuple = default(ValueTuple<string, int>);

			// can initialize constants
			const int j = default;
		}

		public static void ConditionalExpression(string value)
		{
			var tuple = value == null ? default((string, int)) : (value, value.Length);
		}

		public static ValueTuple<string, int> GetValue()
		{
			return default(ValueTuple<string, int>);
		}

		public static void OptionalParameters(ValueTuple<string, int> tuple = default(ValueTuple<string, int>))
		{
		}

		public static void ArrayInitializers()
		{
			var array = new[]
			{
				default(ValueTuple<string, int>),
				("name", 42),
			};
		}

		public static void MethodCall()
		{
			ConditionalExpression(default(string));
			OptionalParameters(default(ValueTuple<string, int>));
		}

		public static void Comparison(string value)
		{
			if (default == value) { }
			if (value == default) { }
		}

		public static void As()
		{
			// only allowed for reference types
			// warning CS0458: The result of the expression is always 'null' of type 'IReadOnlyCollection<string>'
			var collection = default as IReadOnlyCollection<string>;
		}

		public static void SwitchStatement(string value)
		{
			switch (value)
			{
			case " ":
				Console.WriteLine("a single space");
				break;

			case "":
				Console.WriteLine("is empty");
				break;

			// case default:
				// C# 7.1: warning CS8313: Did you mean to use the default switch label (`default:`) rather than `case default:`? If you really mean to use the default literal, consider `case (default):` or another literal (`case 0:` or `case null:`) as appropriate.
			// case (default):
				// VS 15.6: error CS8313: A default literal 'default' is not valid as a case constant. Use another literal (e.g. '0' or 'null') as appropriate. If you intended to write the default label, use 'default:' without 'case'.
			case null:
				Console.WriteLine("is null");
				break;

			default:
				Console.WriteLine("something else");
				break;
			}
		}

		public static void Illegal()
		{
			// error CS8315: Operator '==' is ambiguous on operands 'default' and 'default'
			// if (default == default);
			// NOTE: "if (null == null)" is allowed

			// error CS0023: Operator 'is' cannot be applied to operand of type 'default'
			// if (default is string);

			// if (value is default) { }
			// was legal in C# 7.1
			// VS 15.6: error CS8363: A default literal 'default' is not valid as a pattern.Use another literal(e.g. '0' or 'null') as appropriate.To match everything, use a discard pattern 'var _'.
			// https://github.com/dotnet/roslyn/issues/23499

			// error CS0815: Cannot assign default to an implicitly-typed variable
			// var i = default;

			// error CS0155: The type caught or thrown must be derived from System.Exception
			// throw default;
			// NOTE: "throw default(Exception);" and "throw null;" compile and throw NullReferenceException at runtime; compiler team closed this loophole
		}
	}
}

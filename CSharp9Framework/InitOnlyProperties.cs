using System;

namespace CSharp9Framework
{
	class InitOnlyProperties
	{
		class Poco
		{
			public string Name { get; set; }
			public int Age { get; set; }
		}

		public void Mutable()
		{
			var bob = new Poco
			{
				Name = "Bob",
				Age = 25,
			};

			// mutable
			bob.Name = "John";
		}

		class PocoInit
		{
			public string Name { get; init; }
			public int Age { get; init; }
		}

		public void NotMutable()
		{
			var bob = new PocoInit
			{
				Name = "Bob",
				Age = 25,
			};

			// immutable
			// error CS8852: Init-only property or indexer 'X' can only be assigned in an object initializer, or on 'this' or 'base' in an instance constructor or an 'init' accessor.
			// bob.Name = "John";
		}

		class SetReadonlyProperties
		{
			readonly string _name; // the "init;" syntax generates a readonly field

			public string Name
			{
				get => _name;
				init => _name = value ?? throw new ArgumentNullException(nameof(value));
			}
		}

		/*
		 * An instance property containing an init accessor is considered settable in the following circumstances, except when in a local function or lambda:
		 * During an object initializer
		 * During a with expression initializer
		 * Inside an instance constructor of the containing or derived type, on this or base
		 * Inside the init accessor of any property, on this or base
		 * Inside attribute usages with named parameters
		 */
		class Derived : SetReadonlyProperties
		{
			readonly int _age;

			public int Age
			{
				get => _age;
				init
				{
					_age = value;
					Name = value.ToString();
				}
			}
		}

		public void UseDerived()
		{
			var derived = new Derived { Age = 10 };
		}
	}
}

namespace System.Runtime.CompilerServices
{
	public class IsExternalInit { }
}

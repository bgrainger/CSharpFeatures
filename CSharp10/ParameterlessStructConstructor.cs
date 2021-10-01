namespace CSharp10;

struct ParameterlessStructConstructor
{
	// structs can now have constructors
	public ParameterlessStructConstructor()
	{
		Name = "Struct with no name";
	}

	public string Name { get; }
}

struct ParameterlessStructConstructor2
{
	// structs can now have property/field initializers, which synthesize a default constructor
	public string Name { get; } = "A default name";
}

class ParameterlessStructConstructorExample
{
	public void Example()
	{
		var s1 = new ParameterlessStructConstructor();
		Console.WriteLine(s1.Name); // "Struct with no name"

		ParameterlessStructConstructor s2 = default;
		Console.WriteLine(s2.Name); // (null)
		// may decompile as 'new ParameterlessStructConstructor()', which is now incorrect

		var s3 = MakeGeneric<ParameterlessStructConstructor>();
		Console.WriteLine(s3.Name); // "Struct with no name"

		var s4 = MakeGenericArray<ParameterlessStructConstructor>(2)[0];
		Console.WriteLine(s4.Name); // (null)
	}

	public static T MakeGeneric<T>()
		where T : new()
	{
		return new T();
	}

	public static T[] MakeGenericArray<T>(int size)
		where T : new()
	{
		return new T[size];
	}
}

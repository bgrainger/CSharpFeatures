namespace CSharp11;

readonly struct AutoDefaultStruct
{
	public string Name { get; }
	public int Age { get; }

	public AutoDefaultStruct()
	{
		Name = "No name";

		// previously: error CS0843: Auto-implemented property 'AutoDefaultStruct.Age' must be fully assigned before control is returned to the caller.
	}

	public AutoDefaultStruct(string name) => Name = name;
}

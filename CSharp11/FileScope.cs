namespace CSharp11;

// doesn't conflict with type with the same name in another file
file class ExtendedNameofScope
{
}

// can apply to any type: class, struct, enum, interface, delegate, record
file struct AutoDefaultStruct
{
}

file interface IFileInterface
{
	void DoThing();
}

file interface IFileInterface2
{
	void DoOtherThing();
}

// file interfaces can be implemented by public types
public class PublicClass : IFileInterface, IFileInterface2
{
	public void DoThing() => throw null;

	// explicit implementation can only be invoked in this file
	void IFileInterface2.DoOtherThing() => throw null;
}

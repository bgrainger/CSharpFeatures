namespace CSharp13;

internal class AllowsRefStruct
{
	public void GenericMethod<T>(T t)
	{

	}

	public void CallGenericMethod()
	{
		// OK
		GenericMethod<int>(default);

		// error CS9244: The type 'Span<T>' may not be a ref struct or a type parameter allowing ref structs in order to use it as parameter 'T' 
		// GenericMethod<Span<T>>(default);
	}

	public void GenericMethodAllowsRefStruct<T>(T t)
		where T : allows ref struct
	{
	}

	public void CallGenericMethodWithSpan()
	{
		// OK
		GenericMethodAllowsRefStruct<Span<int>>(default);
	}
}

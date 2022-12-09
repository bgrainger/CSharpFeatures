using Microsoft.AspNetCore.Mvc;

namespace CSharp11;

internal class GenericAttributes
{
	[ProducesResponseType(typeof(SuccessDto), 200)]
	[ProducesResponseType(typeof(ErrorDto), 400)]
	public IActionResult SomeMethod() => default!;

	[ProducesResponseType2<SuccessDto>(200)]
	[ProducesResponseType2<ErrorDto>(400)]
	public IActionResult OtherMethod() => default!;
}

internal class GenericType<T>
{
	// generic attribute needs to be fully constructed; this is not valid:
	// [ProducesResponseType2<T>(200)]
	public IActionResult SomeMethod() => default!;

	// can't use types that aren't directly represented in metadata; use the underlying type instead
	// [ProducesResponseType2<string?>(200)] -- use string
	// [ProducesResponseType2<dynamic>(200)] -- use object
	// [ProducesResponseType2<(int X, int Y)>(200)] -- use ValueTuple<int, int>
	public IActionResult OtherMethod() => default!;
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ProducesResponseType2Attribute<T> : Attribute
//	where T : SuccessDto
{
	public ProducesResponseType2Attribute(int statusCode)
	{
	}
}

public record SuccessDto();
public record ErrorDto();

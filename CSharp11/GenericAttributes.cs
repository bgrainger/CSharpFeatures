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


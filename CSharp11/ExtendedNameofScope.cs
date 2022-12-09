using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace CSharp11;

public class ExtendedNameofScope
{
	[return: NotNullIfNotNull(nameof(input))]
	public int? TryParse(string? input) => default;

	public void Verify(bool condition, [CallerArgumentExpression(nameof(condition))] string conditionExpression = null!) => throw new InvalidOperationException();
}

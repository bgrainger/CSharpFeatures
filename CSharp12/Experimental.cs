using System.Diagnostics.CodeAnalysis;

namespace CSharp12;

internal class Experimental
{
	[Experimental("FL1001", UrlFormat = "https://faithlife.dev/warnings/{0}")]
	public void ExperimentalMethod(string value) => Console.WriteLine(value);

	public void CallMethod()
	{
		// error FL1001: 'CSharp12.Experimental.ExperimentalMethod(string)' is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed. (https://faithlife.dev/warnings/FL1001)
#pragma warning disable FL1001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
		ExperimentalMethod("test");

		// documentation says that [Obsolete] warning will take precedence, but doesn't seem to be the case
#pragma warning disable FL1002
		ObsoleteExperimentalMethod("test 2");
	}

	[Obsolete, Experimental("FL1002", UrlFormat = "https://faithlife.dev/warnings/{0}")]
	public void ObsoleteExperimentalMethod(string value) => Console.WriteLine(value);
}

// can mark an entire assembly as experimental
// [assembly: Experimental("FL1234")]

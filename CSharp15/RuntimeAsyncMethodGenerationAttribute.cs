namespace System.Runtime.CompilerServices;

// experimental per-method escape hatch for runtime async (see RuntimeAsync.cs)
// it isn't a public API in the core libraries; the compiler recognizes it by its full name, so it must be in this namespace
// (declared in any other namespace, it compiles without warning but is silently ignored)
[AttributeUsage(AttributeTargets.Method)]
internal sealed class RuntimeAsyncMethodGenerationAttribute(bool runtimeAsync) : Attribute
{
	public bool RuntimeAsync => runtimeAsync;
}

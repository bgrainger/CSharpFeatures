using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace CSharp11;

public class ScopedRef
{
	public void Scoped(int size)
	{
		Span<byte> initialized = stackalloc byte[100];
		// can't do this:
		// return initialized;

		Span<byte> uninitialized;

		// error CS8352: Cannot use variable 'bytes' in this context because it may expose referenced variables outside of their declaration scope
		// uninitialized = stackalloc byte[100];

		byte[]? buffer = null;
		Span<byte> span = size < 100 ? stackalloc byte[size] : (buffer = ArrayPool<byte>.Shared.Rent(size));

		scoped Span<byte> bytes;
		if (size < 100)
			bytes = stackalloc byte[size];
		else
			bytes = new byte[size];
	}

	public Span<int> CreateSpan(ref int value) => new Span<int>(ref value);
	public Span<int> CreateScopedSpan(scoped ref int value) => default;

	public Span<int> CallCreateSpan(int parameter)
	{
		// error CS8166: Cannot return a parameter by reference 'parameter' because it is not a ref parameter
		// error CS8347: Cannot use a result of 'ScopedRef.CreateSpan(ref int)' in this context because it may expose variables referenced by parameter 'value' outside of their declaration scope
		// return CreateSpan(ref parameter);
		
		return CreateScopedSpan(ref parameter);
	}
}

public struct Unscoped
{
	private int m_value;

	public Unscoped(int value) => m_value = value;

	[UnscopedRef]
	// error CS8170: Struct members cannot return 'this' or other instance members by reference
	public ref int Value => ref m_value;
}

public ref struct RefField
{
	private ref int m_ref;

	public RefField(ref int value) => m_ref = ref value;

	public ref int Value => ref m_ref;

	private ref readonly int m_refReadonly;
	private readonly ref int m_readonlyRef;
	private readonly ref readonly int m_readonlyRefReadonly;

	public void Update(ref int value)
	{
		// m_refReadonly = value;
		m_refReadonly = ref m_ref;

		m_readonlyRef = value;
		// m_readonlyRef = ref m_ref;

		// m_readonlyRefReadonly = value;
		// m_readonlyRefReadonly = ref m_ref;
	}
}

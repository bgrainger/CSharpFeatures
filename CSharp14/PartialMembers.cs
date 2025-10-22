using System.Runtime.InteropServices;

namespace CSharp14;

// Part 1: Defining declaration
internal partial class PartialConstructorExample
{
	// Defining declaration - no body
	public partial PartialConstructorExample(string name);

	public string Name { get; private set; }
	public DateTime CreatedAt { get; private set; }
}

// Part 2: Implementing declaration
internal partial class PartialConstructorExample
{
	// Implementing declaration - has body
	public partial PartialConstructorExample(string name)
		// implementing declaration can use : this() or : base()
		: this(DateTime.Now)
	{
		Name = name;
	}

	public PartialConstructorExample(DateTime createdAt)
	{
		CreatedAt = createdAt;
	}
}

// Part 1: Defining declaration
internal partial class PartialEventExample
{
	// Defining declaration - field-like event
	partial event EventHandler? StatusChanged;

	private partial void RaiseStatusChanged();

	public void ChangeStatus()
	{
		Console.WriteLine("Status changing...");
		RaiseStatusChanged();
	}

	public void Subscribe()
	{
		StatusChanged += (s, e) => { Console.WriteLine("Status changed"); };
	}
}

// Part 2: Implementing declaration
internal partial class PartialEventExample
{
	// Implementing declaration - must have add and remove accessors
	partial event EventHandler? StatusChanged
	{
		add => m_statusChangedHandler += value;
		remove => m_statusChangedHandler -= value;
	}

	private EventHandler? m_statusChangedHandler;

	private partial void RaiseStatusChanged()
	{
		m_statusChangedHandler?.Invoke(this, EventArgs.Empty);
	}
}

// Motivating Example: Interop code
internal partial class InteropFileHandle
{
	[ImportedMethod("CreateFileW")] // (hypothetical; would trigger source generation)
	public partial InteropFileHandle(string fileName, uint desiredAccess, uint shareMode);
}

// Hypothetical source-generated implementation
internal partial class InteropFileHandle
{
	private IntPtr m_handle;

	public partial InteropFileHandle(string fileName, uint desiredAccess, uint shareMode)
	{
		m_handle = CreateFileW(fileName, desiredAccess, shareMode);
	}

	// simplified example; CreateFileW has seven parameters
	[LibraryImport("kernel32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
	private static partial IntPtr CreateFileW(string lpFileName, uint dwDesiredAccess, uint dwShareMode);
}

// Motivating Example: Weak event handlers
internal partial class WeakEventSource
{
	[WeakEvent] // hypothetical; would trigger source generation
	public partial event EventHandler? MyEvent;
}

// Hypothetical source-generated implementation
internal partial class WeakEventSource
{
	private EventHandler? m_myEventHandlers;
	private void RaiseMyEvent(EventArgs eventArgs) => m_myEventHandlers?.Invoke(this, eventArgs);

	public partial event EventHandler? MyEvent
	{
		add
		{
			foreach (Delegate del in value!.GetInvocationList())
			{
				var weakReference = new WeakReference(del.Target);
				var method = del.Method;

				EventHandler handler = null!;
				handler = (s, e) =>
				{
					if (weakReference.Target is { } target)
					{
						method.Invoke(target, [s, e]);
					}
					else
					{
						// remove the handler if the target has been collected
						m_myEventHandlers -= handler;
					}
				};
				m_myEventHandlers += handler;
			}
			m_myEventHandlers += value;
		}
		remove => m_myEventHandlers -= value; // Simplified for illustration
	}
}

[AttributeUsage(AttributeTargets.Constructor)]
internal sealed class ImportedMethodAttribute(string methodName) : Attribute
{
}

[AttributeUsage(AttributeTargets.Event)]
internal sealed class WeakEventAttribute : Attribute
{
}

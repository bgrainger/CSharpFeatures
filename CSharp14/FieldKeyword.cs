using System.ComponentModel;

namespace CSharp14;

internal class FieldKeyword
{
	// Before C# 14: needed explicit backing field
	private string m_message = "";
	public string OldStyleMessage
	{
		get => m_message;
		set => m_message = value ?? throw new ArgumentNullException(nameof(value));
	}

	// C# 14: use 'field' keyword for compiler-synthesized backing field
	public string Message
	{
		get;
		set => field = value ?? throw new ArgumentNullException(nameof(value));
	} = "";

	// Can use in get accessor too
	public string UppercaseMessage
	{
		get => field.ToUpper();
		set => field = value ?? "";
	}

	// Validation on set
	public int Age
	{
		get;
		set => field = value is >= 0 and <= 150
			? value
			: throw new ArgumentOutOfRangeException(nameof(value));
	}

	// Common pattern: change notification
	public string NotifyingProperty
	{
		get;
		set
		{
			if (field != value)
			{
				field = value;
				PropertyChanged?.Invoke(this, new(nameof(NotifyingProperty)));
			}
		}
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	// If you have a field named 'field', use @field or this.field to disambiguate
	private string field = "I'm a field named 'field'";
	public string ConfusingProperty
	{
		get => @field; // refers to the explicit field named 'field'
		set => this.field = value; // refers to the explicit field named 'field'
	}
}

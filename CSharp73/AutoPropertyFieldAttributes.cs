using System;
using System.Runtime.Serialization;

namespace CSharp73
{
	[Serializable]
	class AutoPropertyFieldAttributes
	{
		[NonSerialized]
		private string _csharp7Field;

		public string CSharp7Field
		{
			get => _csharp7Field;
			set => _csharp7Field = value;
		}

		// can apply an attribute to the backing field in C# 7.0
		[field: NonSerialized]
		public event EventHandler MyEvent;

		// now the same is possible for auto-generated properties in C# 7.3
		[field: NonSerialized]
		public string Secret { get; set;  }

		[field: OptionalField]
		public string Optional { get; set; }

		[field: ThreadStatic]
		public static int Count { get; set; }
	}
}

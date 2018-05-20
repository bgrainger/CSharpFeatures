using System;

namespace CSharp73
{
	[Serializable]
	class AutoPropertyFieldAttributes
	{
		[NonSerialized]
		[ThreadStatic]
		private string _secret;

		public string Secret
		{
			get => _secret;
			set => _secret = value;
		}

		// can apply an attribute to the backing field in C# 7.0
		[field: NonSerialized]
		public event EventHandler MyEvent;
	}
}

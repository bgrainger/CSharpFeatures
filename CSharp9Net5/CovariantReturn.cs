namespace CSharp9Net5
{
	public class CovariantReturn
	{
		public abstract class RichTextElement
		{
			public abstract RichTextElement Clone();
		}

		public class RichTextContent : RichTextElement
		{
			public override RichTextContent Clone() => new();
		}

		public class RichTextInline : RichTextContent
		{
			public override RichTextInline Clone() => new();
		}

		public class RichTextRun : RichTextInline
		{
			public override RichTextRun Clone() => new();
		}
	}
}

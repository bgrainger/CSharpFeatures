namespace CSharp13;

internal class PartialMembers
{
	public partial class MyPartialClass
	{
		public partial string Name { get; set; }
		public partial int Count { get; set; }
		public partial string this[int index] { get; set; }
	}

	public partial class MyPartialClass
	{
		// can't do: public partial string Name { get; set; }
		public partial string Name
		{
			get => m_name;
			set => m_name = value;
		}
		private string m_name;

		public partial int Count
		{
			get => m_list.Count;
			set
			{
				while (m_list.Count < value)
					m_list.Add("");
			}
		}
		private List<string> m_list = new();

		public partial string this[int index]
		{
			get => m_list[index];
			set => m_list[index] = value;
		}
	}

	public void Example()
	{
		var mpc = new MyPartialClass();

		mpc.Name = "Example";
		mpc.Count = 2;
		mpc[0] = mpc.Name;
	}
}

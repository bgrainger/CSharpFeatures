namespace CSharp72
{
	public class PrivateProtectedBase
	{
		protected internal void ProtectedInternal()
		{
			// "protected internal" is "protected OR internal"
			// uses 'famorassem' access specifier in IL
		}

		private protected void PrivateProtected()
		{
			// "protected internal" is "protected AND internal"
			// uses 'famandassem' access specifier in IL
		}

		// all dervied types must be in this assembly
		// private protected PrivateProtectedBase() { }
	}

	public class AnotherClass
	{
		public AnotherClass()
		{
			// uses "internal" part of "protected internal"
			new PrivateProtectedBase().ProtectedInternal();

			// can't invoke "private protected": we have "internal" but not "protected" access
			// new PrivateProtectedBase().PrivateProtected();
		}
	}

	public class DerivedInSameAssembly : PrivateProtectedBase
	{
		public void DerivedMethod()
		{
			// succeeds because "protected" (derived class) and "internal" (same assembly)
			base.PrivateProtected();
		}
	}
}

namespace PretendThisIsAnotherAssembly
{
	public class DerivedClasss : CSharp72.PrivateProtectedBase
	{
		public void DerivedMethod()
		{
			// uses "protected" part of "protected internal"
			base.ProtectedInternal();

			// can't invoke "private protected": we have "protected" but not "internal" access
			// base.PrivateProtected();
		}
	}
}

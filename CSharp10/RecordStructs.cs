namespace CSharp10;

// "record struct" can be used to declare a record value type
record struct Point2D(int X, int Y);

// can now write "record class" instead of "record" for clarity if desired
record class RecordClass(string Name, int Age);

class RecordStructExamples
{
	public void Mutable()
	{
		// record class is immutable
		var person = new RecordClass("Jane", 33);
		// person.Age = 34; // error CS8852: Init-only property

		// record struct is mutable!
		// "The asymmetric (im)mutability behavior between record structs and record classes will likely be met with surprise and even distaste with some readers."
		var p = new Point2D(1, 1);
		p.X = 2;
	}

	public void LikeValueTuple()
	{
		// mutability is like existing ValueTuple, and provides a way to upgrade code to named record structs
		var old = OldMethod();
		old.X = 2;

		var now = NewMethod();
		now.X = 2;
	}

	private (int X, int Y) OldMethod() => (1, 1);

	private Point2D NewMethod() => new(1, 1);
}

// "readonly record struct", analogous to "readonly struct"
readonly record struct Point3D(int X, int Y, int Z);

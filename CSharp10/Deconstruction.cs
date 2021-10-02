namespace CSharp10;

internal class Deconstruction
{
	public void Example()
	{
		var point = (X: 1, Y: 2);

		// declare and initialize variables
		(var x1, var y1) = point;
		var (x2, y2) = point;

		// assign to existing variables
		(x1, y1) = point;

		// New: declare AND assign
		(x2, var y3) = point;
	}
}

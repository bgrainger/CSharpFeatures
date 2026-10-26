using System.Diagnostics;

namespace CSharp15;

// C# 15: 'closed' means direct subclasses can only be declared in this assembly
internal closed record class CalculatorOperation;
internal sealed record class Add(double Left, double Right) : CalculatorOperation;
internal sealed record class Subtract(double Left, double Right) : CalculatorOperation;
internal sealed record class Multiply(double Left, double Right) : CalculatorOperation;
internal record class Exponentiation(double Base, double Exponent) : CalculatorOperation;

// derivation isn't transitive: 'Exponentiation' isn't sealed or closed, so another assembly could derive from it
internal sealed record class Square(double Value) : Exponentiation(Value, 2);

internal class ClosedHierarchies
{
	public double Calculate(CalculatorOperation operation) => operation switch
	{
		Add(var left, var right) => left + right,
		Subtract(var left, var right) => left - right,
		Multiply(var left, var right) => left * right,
		Exponentiation(var @base, var exponent) => Math.Pow(@base, exponent),
		// no warning: every direct descendant of 'CalculatorOperation' is handled
	};

	public void UseCalculator()
	{
		Console.WriteLine(Calculate(new Add(2, 3))); // 5
		Console.WriteLine(Calculate(new Exponentiation(2, 10))); // 1024

		// handled by the 'Exponentiation' arm
		Console.WriteLine(Calculate(new Square(9))); // 81
	}

	public double BeforeCSharp15(CalculatorOperation operation) => operation switch
	{
		Add(var left, var right) => left + right,
		Subtract(var left, var right) => left - right,
		Multiply(var left, var right) => left * right,
		Exponentiation(var @base, var exponent) => Math.Pow(@base, exponent),

		// abstract base class: someone else could derive from it, so this was required
		// otherwise: warning CS8509: The switch expression does not handle all possible values of its input type (it is not exhaustive).
		_ => throw new UnreachableException(),
	};

	public double AddNewOperation(CalculatorOperation operation)
	{
		// add 'internal sealed record class Divide(double Left, double Right) : CalculatorOperation;' and every switch without a default arm warns:
		// warning CS8509: The switch expression does not handle all possible values of its input type (it is not exhaustive). For example, the pattern 'CSharp15.Divide' is not covered.
		return Calculate(operation);
	}

	public string NullableOperation(CalculatorOperation? operation) => operation switch
	{
		Add => "+",
		Subtract => "-",
		Multiply => "*",
		Exponentiation => "^",
		null => "", // otherwise: warning CS8655: The switch expression does not handle some null inputs (it is not exhaustive). For example, the pattern 'null' is not covered.
	};

	public string ConstrainedTypeParameter<TOperation>(TOperation operation)
		where TOperation : CalculatorOperation =>
		operation switch
		{
			// type parameter constrained to a closed type is also exhaustive
			Add => "+",
			Subtract => "-",
			Multiply => "*",
			Exponentiation => "^",
		};

	public void ImplicitlyAbstract()
	{
		// a closed class is implicitly abstract
		// error CS0144: Cannot create an instance of the abstract type or interface 'CalculatorOperation'
		// var operation = new CalculatorOperation();

		// can't be combined with 'sealed', 'static', or 'abstract'
		// error CS9381: 'CalculatorOperation': a closed type cannot be sealed or static
	}

	// Note: 'Tree<T>' is an open type of a closed class, this is called the Open/Closed Principle
	internal closed record class Tree<T>; 
	internal sealed record class Leaf<T>(T Value) : Tree<T>;
	internal sealed record class Branch<T>(Tree<T> Left, Tree<T> Right) : Tree<T>;

	public int Sum(Tree<int> tree) => tree switch
	{
		Leaf<int>(var value) => value,
		Branch<int>(var left, var right) => Sum(left) + Sum(right),
	};

	public void ContextualKeyword()
	{
		// 'closed' is a contextual keyword; it's still a valid identifier
		var closed = DateTime.Now.Hour >= 17;
		Console.WriteLine(closed ? "Closed" : "Open");
	}

	// Real-world example: LogosDesktop's FacetNode (LDLS4\Controls\Facets) has exactly two sealed subclasses, FacetGroup and FacetTerm

	// Before: DocumentFacetSource.GetFacetGroupDataForTerms
	public IEnumerable<(string GroupId, FacetTerm Term)> GetTerms1(FacetNode node, FacetGroup parentGroup)
	{
		if (node is FacetGroup group)
			return group.Children.SelectMany(x => GetTerms1(x, group));
		if (node is FacetTerm term)
			return [(parentGroup.Id, term)];
		throw new InvalidOperationException("Unknown facet type");

		// FacetState.CreateAndStoreViewModel has the same if/else chain, ending in:
		// throw new NotImplementedException("Unexpected facet node type.");
	}

	// After: with 'public closed class FacetNode', a switch expression is exhaustive
	public IEnumerable<(string GroupId, FacetTerm Term)> GetTerms2(FacetNode node, FacetGroup parentGroup) =>
		node switch
		{
			FacetGroup group => group.Children.SelectMany(x => GetTerms2(x, group)),
			FacetTerm term => [(parentGroup.Id, term)],
		};

	// System.Text.Json can infer polymorphism for closed hierarchies: see Net11Json.cs
}

// simplified from LogosDesktop, where it's 'public abstract class FacetNode'
internal closed class FacetNode(string id, string title)
{
	public string Id { get; } = id;
	public string Title { get; } = title;
}

internal sealed class FacetGroup(string id, string title, IReadOnlyList<FacetNode> children) : FacetNode(id, title)
{
	public IReadOnlyList<FacetNode> Children { get; } = children;
}

internal sealed class FacetTerm(string id, string title, int? count) : FacetNode(id, title)
{
	public int? Count { get; } = count;
}

namespace CSharp14;

internal class NullConditionalAssignment
{
	public void BasicNullConditionalAssignment()
	{
		Customer? customer = GetCustomer();

		// Before C# 14: needed null check
		if (customer is not null)
		{
			customer.Order = GetCurrentOrder();
		}

		// C# 14: simplified with ?. operator
		customer?.Order = GetCurrentOrder();

		// Or:
		GetCustomer()?.Order = GetCurrentOrder();
	}

	public void NullConditionalWithIndexer()
	{
		Dictionary<string, int>? dict = GetDictionary();

		// Before C# 14
		if (dict is not null)
			dict["key"] = 42;

		// C# 14: use ?[] operator
		GetDictionary()?["key"] = 42;
	}

	public void CompoundAssignmentOperators()
	{
		Customer? customer = GetCustomer();

		// Works with compound assignment operators
		customer?.Score += 10;
		customer?.Score -= 5;
		customer?.Score *= 2;
		customer?.Score /= 3;

		// Also works with collection indexers
		GetDictionary()?["count"] += 1;
	}

	public void IncrementDecrementNotAllowed()
	{
		Customer? customer = GetCustomer();

		// These are NOT allowed:
		// customer?.Score++;
		// customer?.Score--;
		// ++customer?.Score;
		// --customer?.Score;
	}

	public void RightSideNotEvaluatedWhenNull()
	{
		Customer? customer = null;

		// GetCurrentOrder() is NOT called when customer is null
		customer?.Order = GetCurrentOrder();

		// Same with compound assignment
		customer?.Score += ExpensiveCalculation();
	}

	public void NestedNullConditional()
	{
		Company? company = GetCompany();

		// Can chain null-conditional operators
		company?.PrimaryCustomer?.Order = GetCurrentOrder();

		// Works with indexers too
		company?.Customers?[0].Order = GetCurrentOrder();
	}

	private static Customer? GetCustomer() => new Customer();
	private static Dictionary<string, int>? GetDictionary() => new();
	private static Order GetCurrentOrder() { Console.WriteLine("GetCurrentOrder called"); return new Order(); }
	private static int ExpensiveCalculation() { Console.WriteLine("ExpensiveCalculation called"); return 100; }
	private static Company? GetCompany() => new Company();
}

internal class Customer
{
	public Order? Order { get; set; }
	public int Score { get; set; }
}

internal class Order
{
}

internal class Company
{
	public Customer? PrimaryCustomer { get; set; }
	public List<Customer>? Customers { get; set; }
}

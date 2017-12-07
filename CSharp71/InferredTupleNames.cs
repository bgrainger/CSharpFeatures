using System;
using System.Globalization;
using System.Linq;

namespace CSharp71
{
    public static class InferredTupleNames
    {
	    public static void CreateFromLocals()
	    {
		    var firstName = "Bradley";
		    var lastName = "Grainger";
		    var hireDate = new DateTime(2000, 6, 27);

			// C# 7.0: unnamed tuple elements are called Item1, Item2, Item3, etc.
		    var employee1 = (firstName, lastName, hireDate);
		    Console.WriteLine($"{employee1.Item1} {employee1.Item2} was hired on {employee1.Item3}");

			// C# 7.0: can/must explicitly name elements
			var employee2 = (firstName: firstName, lastName: lastName, hireDate: hireDate);
		    Console.WriteLine($"{employee2.Item1} {employee2.Item2} was hired on {employee2.Item3}");
		    Console.WriteLine($"{employee2.firstName} {employee2.lastName} was hired on {employee2.hireDate}");

			// C# 7.1: names are inferred
		    var employee3 = (firstName, lastName, hireDate);
		    Console.WriteLine($"{employee3.Item1} {employee3.Item2} was hired on {employee3.Item3}");
		    Console.WriteLine($"{employee3.firstName} {employee3.lastName} was hired on {employee3.hireDate}");
		}

	    public static void DuplicateNames()
	    {
			var firstName = "Bradley";
		    var lastName = "Grainger";
		    var hireDate = new DateTime(2000, 6, 27);

			// if name inference would create duplicates, it's not performed
		    var employee4 = (firstName, firstName, hireDate);
			// employee4 has properties Item1, Item2, Item3
	    }

	    public static void CreateFromLinq()
	    {
		    CultureInfo.GetCultures(CultureTypes.AllCultures)
			    .Select(x => (x.EnglishName, x.IetfLanguageTag))
			    .OrderBy(x => x.EnglishName)
			    .ToList();
	    }

	    public static void BreakingChange()
	    {
		    var X = 1;
		    Action Dump = () => Console.WriteLine("lambda");
		    var tuple = (X, Dump);

			// C# 7.0: called extension method
			// C# 7.0 in VS 2017.3: error CS8306: Tuple element name 'Dump' is inferred. Please use language version 7.1 or greater to access an element by its inferred name.
			// C# 7.1: calls tuple.Item2() (via inferred name "Dump")
			tuple.Dump();
	    }

		public static void Dump<T1, T2>(this ValueTuple<T1, T2> tuple)
	    {
		    Console.WriteLine("ValueTuple: ({0}, {1})", tuple.Item1, tuple.Item2);
	    }
	}
}

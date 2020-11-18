using InlineMapping;

namespace CSharp9Framework
{
	public class DbPerson
	{
		public string Name { get; set; }
		public int Age { get; set; }
		public string Location { get; set; }
	}

	public class Person
	{
		public string Name { get; set; }
		public int Age { get; set; }
		public string Location { get; set; }
	}

	public record PersonDto
	{
		public string Name { get; init; }
		public int Age { get; init; }
		public string Location { get; init; }
	}

	// Using https://www.nuget.org/packages/InlineMapping/ - https://github.com/JasonBock/InlineMapping
	// [MapTo(typeof(Person))]
	// [MapTo(typeof(PersonDto))]

	public class SourceGenerators
	{
		public void Use()
		{
			var dbPerson = new DbPerson { Name = "Robin", Age = 30, Location = "Anywhere, USA" };
			// var person = dbPerson.MapToPerson();
			// var personDto = person.MapToPersonDto();
		}
	}
}

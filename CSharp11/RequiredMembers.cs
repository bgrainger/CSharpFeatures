using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;

namespace CSharp11;

class InitMembers
{
	public int Id { get; init; }

	// warning CS8618: Non-nullable property 'Name' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
	public string Name { get; init; }

	public void Usage()
	{
		var good = new InitMembers { Id = 1, Name = "Fred" };
		var bad = new InitMembers { Id = 2 };
	}
}

class RequiredMembers
{
	public int Id { get; init; }
	public required string Name { get; init; }

	public void Usage()
	{
		var good = new RequiredMembers { Id = 1, Name = "Fred" };

		// error CS9035: Required member 'RequiredMembers.Name' must be set in the object initializer or attribute constructor.
		// var bad = new RequiredMembers { Id = 1 };
	}
}

internal class MyWebApplication
{
	public void DemonstrateConfiguration()
	{
		var builder = WebApplication.CreateBuilder();

		builder.Configuration.AddInMemoryCollection(new[] { new KeyValuePair<string, string?>("test", "value") });

		builder.Services.AddOptions<AppSettings1>().BindConfiguration("").ValidateDataAnnotations();
		builder.Services.AddOptions<AppSettings2>().BindConfiguration("").ValidateDataAnnotations();
		builder.Services.AddOptions<AppSettings3>().BindConfiguration("").ValidateDataAnnotations();

		var app = builder.Build();

		var settings1 = app.Services.GetRequiredService<IOptions<AppSettings1>>().Value;
		var settings2 = app.Services.GetRequiredService<IOptions<AppSettings2>>().Value;
		var settings3 = app.Services.GetRequiredService<IOptions<AppSettings3>>().Value;
	}
}

class AppSettings1
{
	public string Test { get; set; } = default!;
	public string Test2 { get; set; } = default!;
}

class AppSettings2
{
	[AllowNull, Required]
	public string Test { get; set; }

	[AllowNull]
	public string Test2 { get; set; }
}

class AppSettings3
{
	[Required]
	public required string Test { get; set; }

	// [RequiredMember]
	// see https://github.com/dotnet/aspnetcore/issues/40099
	public required string Test2 { get; set; }
}


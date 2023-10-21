// current
using MyString = System.String;

// new in C# 12
using MyString2 = char[];
using UriRequest = (System.Uri Uri, string Method, string UserAgent, string Accept);

namespace CSharp12;

internal class AliasAnyType
{
	public MyString GetString() => "abc";

	public MyString2 GetString2() => "abc".ToCharArray();

	public Task GetAsync(UriRequest request) => default;
}

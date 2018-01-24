# JsonDigest
Generates a digest of a JSON string with configurable normalization and hash algorithm.

Usage example:

```csharp
var obj = @"
{
    ""foo"": 4.10000, 
    ""bar"": ""thing"", 
    ""stuff"": [ ""erm"", {""b"": 1, ""a"": 2}, 7 ],
    ""other"": { 
        ""surname"": ""Fowler"", 
        ""forename"": ""Derek"" 
    } 
}";

var digester = new JsonDigest();
var digest = digester.Generate(obj); // => def26d4afaead860a3b4ebe03fba54e6
```
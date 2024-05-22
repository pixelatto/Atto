using System.Text.RegularExpressions;

public static class StringExtensions
{
    public static string AddSpacesToCamelCase(string input)
    {
        return Regex.Replace(input, "([a-z])([A-Z])", "$1 $2");
    }
} 
public static class StringExtensions
{
    public static bool Empty(this string @string)
    {
        return string.IsNullOrEmpty(@string);
    }
    
    public static bool NotEmpty(this string @string) => !Empty(@string);

    public static bool White(this string @string)
    {
        return string.IsNullOrWhiteSpace(@string);
    }
}
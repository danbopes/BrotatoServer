namespace SearchEngine.Utilities;
internal static class StringExtensions
{
    public static string CleanPunctuation(this string name)
    {
        return name
            .Replace(",", "")
            .Replace("'", "");
    }

}

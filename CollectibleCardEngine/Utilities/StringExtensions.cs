using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectibleCardEngine.Utilities;
internal static class StringExtensions
{
    public static string CleanPunctuation(this string name)
    {
        return name
            .Replace(",", "")
            .Replace("'", "");
    }

}

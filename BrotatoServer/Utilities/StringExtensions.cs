using CodeKicker.BBCode;
using Microsoft.AspNetCore.Components;

namespace BrotatoServer.Utilities;

public static class StringExtensions
{
    public static string UcFirst(this string str) => str switch
    {
        null => throw new ArgumentNullException(nameof(str)),
        "" => throw new ArgumentException($"{nameof(str)} cannot be empty", nameof(str)),
        _ => str[0].ToString().ToUpper() + str[1..]
    };

    public static string GetAssetPath(this string str)
    {
        return str.Replace("res://", "assets/");
    }

    /*private static readonly BbParser _bbParser = new BbParser(new[] {
        new Tag("color", "<span style=\"color: {value}\">", "</span>", withAttribute: true, secure: false),
    }, new Dictionary<string, string>(), new Dictionary<string, string>()
    {
        { "\n", "<br />" }
    });*/
    private static readonly BBCodeParser _bbParser = new BBCodeParser(ErrorMode.ErrorFree, null, new[]
    {
         new BBTag("color", "<span style=\"color: ${color}\">", "</span>", new BBAttribute("color", "")),
         new BBTag("img", "<img style=\"${style}\" src=\"${content}\" />", "", false, BBTagClosingStyle.RequiresClosingTag,
             (content) => content.GetAssetPath(),
             new BBAttribute("style", "", (ctx) =>
             {
                 var split = ctx.AttributeValue.Split('x');

                 return $"width: {split[0]}px; height: {split[1]}px;";
             }))
    });

    private static readonly BBCodeParser _bbStripParser = new BBCodeParser(ErrorMode.ErrorFree, null, new[]
    {
         new BBTag("color", "", "", new BBAttribute("color", "")),
         new BBTag("img", "", "", true, BBTagClosingStyle.RequiresClosingTag, (content) => {
             switch (content)
             {
                 case "res://items/stats/ranged_damage.png":
                     return "Ranged";
                 case "res://items/stats/melee_damage.png":
                     return "Melee";
                 default:
                    return "";
             }
         }, new BBAttribute("dimensions", ""))
    });

    public static string StripBBCodeText(this string str)
    {
        return _bbStripParser.ToHtml(str);
    }

    public static MarkupString ParseBBCodeText(this string str)
    {
        //[img=20x20]res://items/stats/luck.png[/img]
        //[color=#00ff00]+30%[/color]
        //var tag = new Tag()
        return new MarkupString(_bbParser.ToHtml(str));
    }
}

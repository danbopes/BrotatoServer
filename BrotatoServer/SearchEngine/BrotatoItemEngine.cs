using System.Text;
using BrotatoServer.Data.Game;
using BrotatoServer.Utilities;
using SearchEngine;

namespace BrotatoServer.SearchEngine;

public class BrotatoItemEngine : WebEngine<BrotatoItem>
{
    public BrotatoItemEngine(ILogger<WebEngine<BrotatoItem>> log) : base(log)
    {
    }

    public override string Name => "BrotatoItems";

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    protected override async IAsyncEnumerable<BrotatoItem> InitializeObjectsAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        foreach (var (key, item) in BrotatoAssets.Items)
        {
            var wikiUrl = "https://brotato.wiki.spellsandguns.com/" + item.Name.Replace(' ', '_');

            var textOutput = new StringBuilder();
            textOutput.Append(item.Name);

            if (!string.IsNullOrEmpty(item.EffectsText))
            {
                textOutput.Append(" | ");
                textOutput.Append(item.EffectsText.Replace("\n", " | ").StripBBCodeText());
            }

            textOutput.Append(" | ");
            textOutput.Append(wikiUrl);
            
            yield return new BrotatoItem
            {
                Culture = "en-US",
                Id = key,
                Name = item.Name,
                TextOutput = textOutput.ToString()
            };
        }
    }
}

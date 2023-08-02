using BrotatoServer.Data.Game;
using BrotatoServer.Utilities;
using CardSearcher.CardSearchers.CardEngines;

namespace BrotatoServer.SearchEngine;

public class BrotatoItemEngine : WebEngine<BrotatoItem>
{
    public BrotatoItemEngine(ILogger<WebEngine<BrotatoItem>> log) : base(log)
    {
    }

    public override string Name => "BrotatoItems";

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    protected override async IAsyncEnumerable<BrotatoItem> InitializeCardsAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        foreach (var (key, item) in Assets.Items)
        {
            var wikiUrl = "https://brotato.wiki.spellsandguns.com/" + item.Name.Replace(' ', '_');
            yield return new BrotatoItem
            {
                Culture = "en-US",
                Id = key,
                Name = item.Name,
                TextOutput = item.EffectsText.Replace("\n", " | ").StripBBCodeText() + $" | {wikiUrl}"
            };
        }
    }
}

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

    protected override async IAsyncEnumerable<BrotatoItem> InitializeCardsAsync()
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

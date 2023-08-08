using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Util;

namespace SearchEngine;

internal class CaseInsensitiveKeywordAnalyzer : Analyzer
{
    private readonly LuceneVersion _matchVersion;

    public CaseInsensitiveKeywordAnalyzer(LuceneVersion matchVersion)
    {
        _matchVersion = matchVersion;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "TokenStreamComponents will dispose of components")]
    protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
    {
        var source = new KeywordTokenizer(reader);
        var filter = new LowerCaseFilter(_matchVersion, source);

        return new TokenStreamComponents(source, filter);
    }
}

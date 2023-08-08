using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.Miscellaneous;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.Extensions.Logging;
using SearchEngine.Utilities;

namespace SearchEngine;

/// <summary>
/// Provides an easy way for cards to be populated into an indexed lucene database
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract partial class WebEngine<T> : ICardEngine, IDisposable where T : ISearchableObject
{
    public abstract string Name { get; }
    public bool InitialSearchEnabled { get; protected set; }
    
    private readonly ILogger<WebEngine<T>> _log;
    private readonly FSDirectory _index;
    private readonly IndexWriter _writer;
    private readonly SearcherManager _searcher;
    private readonly SemaphoreSlim _indexLocker = new(1);

    private HashSet<string> _cardsIds = new();

    private string _indexDir => $"Indexes/{Name}";
    private List<Analyzer> _analyzers = new();

    private const LuceneVersion LUCENE_VERSION = LuceneVersion.LUCENE_48;

    protected WebEngine(ILogger<WebEngine<T>> log)
    {
        _log = log;

#if !DEBUG
        if (System.IO.Directory.Exists(_indexDir))
            System.IO.Directory.Delete(_indexDir, true);
#endif

        var info = System.IO.Directory.CreateDirectory(_indexDir);
        _index = new SimpleFSDirectory(_indexDir);
        
        var standardAnalyzer = new StandardAnalyzer(LUCENE_VERSION);
        var ciKeywordAnalyzer = new CaseInsensitiveKeywordAnalyzer(LUCENE_VERSION);

        var analyzer = new PerFieldAnalyzerWrapper(standardAnalyzer, new Dictionary<string, Analyzer>()
                {
                    {"CIName", ciKeywordAnalyzer},
                    {"CINameNoPunctuation", ciKeywordAnalyzer},
                    {"CIInitials", ciKeywordAnalyzer},
                    {"SimpleName", ciKeywordAnalyzer}
                });

        _analyzers.Add(standardAnalyzer);
        _analyzers.Add(ciKeywordAnalyzer);
        _analyzers.Add(analyzer);

        _writer = new IndexWriter(_index, new IndexWriterConfig(LUCENE_VERSION, analyzer));
        _searcher = new SearcherManager(_writer, true, null);
    }

    public async Task InitializeAsync()
    {
#if DEBUG
        if (System.IO.Directory.EnumerateFiles(_indexDir).Take(2).Count() > 1)
            return;
#endif
        try
        {
            await UpdateIndexAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Exception building indexes");
        }
    }

    protected abstract IAsyncEnumerable<T> InitializeObjectsAsync();
    
    protected async Task UpdateIndexAsync()
    {
        await _indexLocker.WaitAsync().ConfigureAwait(false);

        try
        {
            await foreach (var obj in InitializeObjectsAsync())
            {
                if (!_cardsIds.Add(obj.Id))
                    continue;

                var doc = new Document
                {
                    new StoredField("Id", obj.Id),
                    new StringField("Culture", obj.Culture, Field.Store.YES),
                    new Field("CIName", obj.Name, new FieldType
                    {
                        IsStored = true,
                        IsIndexed = true,
                        IsTokenized = true,
                        StoreTermVectors = true,
                        StoreTermVectorPositions = true,
                        StoreTermVectorOffsets = true,
                    }),
                    new Field("CINameNoPunctuation", obj.Name.CleanPunctuation(), new FieldType
                    {
                        IsIndexed = true,
                        IsTokenized = true
                    }) {Boost = 0.5f},
                    new Field("SimpleName", obj.Name.CleanPunctuation(), new FieldType
                        {
                            IsIndexed = true,
                            IsTokenized = true,
                            StoreTermVectors = true,
                            StoreTermVectorPositions = true,
                            StoreTermVectorOffsets = true,
                        }),
                    new StoredField("Card", JsonSerializer.SerializeToUtf8Bytes(obj))
                };

                if (InitialSearchEnabled)
                {
                    var nameClean = CleanRegex().Replace(obj.Name, "");

                    var initials = string.Join("", nameClean.Split(' ')
                            .Where(word => !string.IsNullOrWhiteSpace(word))
                            .Select(word => word[0]))
                        .ToLowerInvariant();
                    
                    doc.Add(new StringField("CIInitials", initials, Field.Store.NO));
                }
                _writer.AddDocument(doc);
            }
            _writer.Commit();
            _searcher.MaybeRefresh();
        }
        finally
        {
            _indexLocker.Release();
        }
    }

    private static IEnumerable<ISearchableObject> GetResults(IndexSearcher searcher, IEnumerable<ScoreDoc> scoreDocs, string fetchCulture)
    {
        if (scoreDocs == null) throw new ArgumentNullException(nameof(scoreDocs));
        if (fetchCulture == null) throw new ArgumentNullException(nameof(fetchCulture));

        foreach (var doc in scoreDocs)
        {
            var currentDoc = searcher.Doc(doc.Doc);

            // Already english?
            if (currentDoc.GetField("Culture").GetStringValue() == fetchCulture)
            {
                var bytes = searcher.Doc(doc.Doc).GetField("Card").GetBinaryValue();
                yield return JsonSerializer.Deserialize<T>(bytes.Bytes)!;
            }
            else
            {
                // Lets fetch the requested version
                var search = new BooleanQuery {
                    {new TermQuery(new Term("Culture", fetchCulture)), Occur.MUST},
                    {new TermQuery(new Term("Id", currentDoc.GetField("Id").GetStringValue())), Occur.MUST}
                };

                var hits = searcher.Search(search, 1);
                var docId = hits.TotalHits > 0 ? hits.ScoreDocs[0].Doc : doc.Doc;
                var bytes = searcher.Doc(docId).GetField("Card").GetBinaryValue();
                yield return JsonSerializer.Deserialize<T>(bytes.Bytes)!;
            }
        }
    }

    public int CardCount
    {
        get
        {
            var searcher = _searcher.Acquire();

            try
            {
                return searcher.IndexReader.NumDocs;
            }
            finally
            {
                _searcher.Release(searcher);
            }
        }
    }

    public async Task<ISearchResults<ISearchableObject>> FindAsync(string cardName)
    {
        ArgumentNullException.ThrowIfNull(cardName);

        var searcher = _searcher.Acquire();

        try
        {
            var culture = Thread.CurrentThread.CurrentCulture;

            var cultureName = culture.Name;

            if (searcher.IndexReader.NumDocs == 0)
            {
                _log.LogWarning("Searcher has no cards to search.");
                return new SearchResults<ISearchableObject>(cardName, Array.Empty<ISearchableObject>(), 0);
            }

            cardName = cardName.Trim();

            var fetchCulture = cultureName;

            if (cardName.IndexOf("en ", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                cardName = cardName[3..];
                fetchCulture = "en-US";
            }

            _log.LogDebug("Search: {CardName}", cardName);
            FixSplitCardName(ref cardName);

            var hits1 = await WildCardSearch(searcher, cardName, cultureName);

            if (hits1.TotalHits > 0)
            {
                return new SearchResults<ISearchableObject>(cardName, ResultType.StartsWith,
                    GetResults(searcher, hits1.ScoreDocs, fetchCulture),
                    hits1.TotalHits
                );
            }

            
            if (InitialSearchEnabled && cardName.Length < 5 && !cardName.Contains(" "))
            {
                var initialHits = InitialSearch(searcher, cardName, cultureName);

                if (initialHits.TotalHits > 0)
                {
                    return new SearchResults<ISearchableObject>(cardName, ResultType.Initials,
                            GetResults(searcher, initialHits.ScoreDocs, fetchCulture),
                            initialHits.TotalHits
                        );
                }
            }

            var hits2 = ContainSearch(searcher, cardName, cultureName);
            if (hits2.TotalHits > 0)
            {
                return new SearchResults<ISearchableObject>(cardName, ResultType.Contains,
                    GetResults(searcher, hits2.ScoreDocs, fetchCulture),
                    hits2.TotalHits
                );
            }

            var hits3 = FuzzySearch(searcher, cardName, cultureName)
                .ScoreDocs
                .Where(scoreDoc => scoreDoc.Score > 3)
                .OrderByDescending(scoreDoc => scoreDoc.Score)
                .ToArray();

            if (hits3.Length > 1)
            {
                //Do we have a high hit, greater then 3 of the second? If so, just return the first.
                if (hits3.First().Score > hits3.Skip(1).First().Score + 3)
                {
                    hits3 = new[] { hits3.First() };
                }
            }

            if (hits3.Any())
            {
                return new SearchResults<ISearchableObject>(cardName, ResultType.Fuzzy,
                    GetResults(searcher, hits3, fetchCulture),
                    hits3.Length
                );
            }

            var hits4 = FuzzySearchAll(searcher, cardName, cultureName);
            if (hits4.ScoreDocs.Any(scoreDoc => scoreDoc.Score > 3))
            {
                return new SearchResults<ISearchableObject>(cardName, ResultType.Fuzzy,
                    GetResults(searcher, hits4.ScoreDocs
                        .Where(scoreDoc => scoreDoc.Score > 3), fetchCulture),
                    hits4.TotalHits
                );
            }

            return new SearchResults<ISearchableObject>(cardName, Array.Empty<ISearchableObject>(), 0);
        }
        finally
        {
            _searcher.Release(searcher);
        }
    }


    private async Task<TopDocs> WildCardSearch(IndexSearcher searcher, string cardName, string cultureName)
    {
        using var analyzer = new CaseInsensitiveKeywordAnalyzer(LUCENE_VERSION);
        using var analyzer2 = new CaseInsensitiveKeywordAnalyzer(LUCENE_VERSION);
        var qp = new QueryParser(LUCENE_VERSION, "CIName", analyzer)
        {
            AllowLeadingWildcard = true
        };
        var qp2 = new QueryParser(LUCENE_VERSION, "CINameNoPunctuation", analyzer2)
        {
            AllowLeadingWildcard = true
        };


        /*var term = new Term("Name", cardName + "*");
        var term2 = new Term("NameNoCommas", cardName.Replace(",", "") + "*");

        var query = new BooleanQuery()
        {
            {new WildcardQuery(term), Occur.MUST},
            {new WildcardQuery(term2), Occur.MUST}
        };*/

        var cleanName = cardName.CleanPunctuation();

        var escaped = QueryParserBase.Escape(cardName).Replace(" ", @"\ ");
        var escapedClean = QueryParserBase.Escape(cleanName).Replace(" ", @"\ ");

        var cultureQuery = new BooleanQuery
        {
            { new TermQuery(new Term("Culture", cultureName)), Occur.SHOULD },
            { new TermQuery(new Term("Culture", "en-US")), Occur.SHOULD }
        };

        // var cardQuery = new BooleanQuery
        // {
        //     { qp.Parse(escaped + "*"), Occur.SHOULD },
        //     { qp2.Parse(escapedClean + "*"), Occur.SHOULD }
        // };

        var query = new BooleanQuery
        {
            {cultureQuery, Occur.MUST},
            {qp.Parse(escaped + "*"), Occur.MUST}
        };


        var hits1 = searcher.Search(query, 50);

        if (hits1.ScoreDocs.Length > 0)
        {
            _log.LogDebug("WildCardSearch1: {Result}", FormatSearchResults(searcher, hits1));

            return hits1;
        }

        var query2 = new BooleanQuery
        {
            {cultureQuery, Occur.MUST},
            {qp2.Parse(escapedClean + "*"), Occur.MUST}
        };

        var hits2 = searcher.Search(query2, 50);

        _log.LogDebug("WildCardSearch2: {Result}", FormatSearchResults(searcher, hits2));
        return hits2;
    }

    private TopDocs InitialSearch(IndexSearcher searcher, string cardInitals, string cultureName)
    {
        Debug.Assert(InitialSearchEnabled);
        using var analyzer = new CaseInsensitiveKeywordAnalyzer(LUCENE_VERSION);
        var qp = new QueryParser(LUCENE_VERSION, "CIInitials", analyzer);

        var cultureQuery = new BooleanQuery
        {
           {
               new TermQuery(new Term("Culture", cultureName)),
               Occur.SHOULD
           },
           { new TermQuery(new Term("Culture", "en-US")), Occur.SHOULD }
        };

        var query = new BooleanQuery
        {
            {cultureQuery, Occur.MUST},
            {qp.Parse(QueryParser.Escape(cardInitals)), Occur.MUST}
        };

        var hits = searcher.Search(query, 50);

        _log.LogDebug("InitialSearch: {Result}", FormatSearchResults(searcher, hits));

        return hits;
    }

    private TopDocs ContainSearch(IndexSearcher searcher, string cardName, string cultureName)
    {
        using var simpleAnalyzer = new CaseInsensitiveKeywordAnalyzer(LUCENE_VERSION);
        var qp = new QueryParser(LUCENE_VERSION, "SimpleName", simpleAnalyzer)
        {
            AllowLeadingWildcard = true
        };

        var test = qp.Parse(QueryParserBase.Escape(cardName));

        if (string.IsNullOrEmpty(test.ToString()))
            return new TopDocs(0, Array.Empty<ScoreDoc>(), 0f);

        var cultureQuery = new BooleanQuery
        {
            { new TermQuery(new Term("Culture", cultureName)), Occur.SHOULD },
            { new TermQuery(new Term("Culture", "en-US")), Occur.SHOULD }
        };

        cardName = cardName
            .Replace("OR", "\\OR")
            .Replace("AND", "\\AND")
            .Replace("NOT", "\\NOT");

        var query = new BooleanQuery
        {
            {cultureQuery, Occur.MUST},
            {qp.Parse(string.Join(" ", QueryParser.Escape(cardName).Split(' ').Select(c => "+*" + c + "*"))), Occur.MUST}
        };

        var hits = searcher.Search(query, 50);
        _log.LogDebug("ContainSearch: {Result}", FormatSearchResults(searcher, hits));

        return hits;
    }

    private TopDocs FuzzySearch(IndexSearcher searcher, string cardName, string cultureName)
    {
        using var analyzer = new CaseInsensitiveKeywordAnalyzer(LUCENE_VERSION);
        var qp = new QueryParser(LUCENE_VERSION, "SimpleName", analyzer);

        var cultureQuery = new BooleanQuery
                                   {
                                       {
                                           new TermQuery(new Term("Culture", cultureName)),
                                           Occur.SHOULD
                                       },
                                       { new TermQuery(new Term("Culture", "en-US")), Occur.SHOULD }
                                   };

        var query = new BooleanQuery
        {
            {cultureQuery, Occur.MUST},
            {qp.Parse(QueryParser.Escape(cardName).Replace(" ", @"\ ") + "~0.65"), Occur.MUST}
        };

        //var query = new FuzzyQuery(new Term("Name", QueryParser.Escape(cardName).ToLower()), 0.65f);

        var hits = searcher.Search(query, 50);
        _log.LogDebug("FuzzySearch: {Results}", FormatSearchResults(searcher, hits));

        return hits;
    }

    private TopDocs FuzzySearchAll(IndexSearcher searcher, string cardName, string cultureName)
    {
        using var analyzer = new SimpleAnalyzer(LUCENE_VERSION);
        var qp = new QueryParser(LUCENE_VERSION, "SimpleName", analyzer);

        var cultureQuery = new BooleanQuery
        {
            { new TermQuery(new Term("Culture", cultureName)), Occur.SHOULD },
            { new TermQuery(new Term("Culture", "en-US")), Occur.SHOULD }
        };

        // cardName = cardName
        //     .Replace("OR", "\\OR")
        //     .Replace("AND", "\\AND")
        //     .Replace("NOT", "\\NOT");

        var bq = new QueryBuilder(analyzer).CreateBooleanQuery("SimpleName",
            string.Join(" ",
                cardName.Split(' ').Where(c => !string.IsNullOrWhiteSpace(c))
                    .Select(c => new FuzzyQuery(new Term("SimpleName", c), 2))));

        var query = new BooleanQuery
        {
            {cultureQuery, Occur.MUST},
            {bq, Occur.MUST}
        };

        var hits = searcher.Search(query, 50);
        _log.LogDebug("FuzzySearchAll: {0}", FormatSearchResults(searcher, hits));

        return hits;
    }

    private string FormatSearchResults(IndexSearcher searcher, TopDocs searchDocs)
    {
        return string.Join(", ", searchDocs.ScoreDocs
            .Select(
                scoreDoc =>
                    $"{searcher.Doc(scoreDoc.Doc).GetField("CIName").GetStringValue()} ({scoreDoc.Score})"
            ).ToList()
        );
    }

    private static void FixSplitCardName(ref string cardName)
    {
        if (cardName.Contains("/"))
        {
            cardName = cardName.Replace("//", "/").Replace(" ", "").Replace("/", " // ");
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _writer.Dispose();
            _index.Dispose();

            foreach (var analyzer in _analyzers)
            {
                analyzer.Dispose();
            }

            _searcher.Dispose();
        }
    }

    public Task<ISearchableObject?> GetRandomAsync()
    {
        var searcher = _searcher.Acquire();

        try
        {
            var tries = 100;

            while (--tries > 0)
            {
                var rand = Random.Shared.Next(searcher.IndexReader.MaxDoc);

                var bytes = searcher.Doc(rand).GetField("Card").GetBinaryValue() ??
                    throw new Exception("Card was null");

                var card = JsonSerializer.Deserialize<T>(bytes.Bytes)!;

                if (card.Culture == "en-US")
                    return Task.FromResult<ISearchableObject?>(card);
            }
        }
        finally
        {
            _searcher.Release(searcher);
        }

        return Task.FromResult<ISearchableObject?>(null);
    }

    protected virtual void OnCardAdded(T card)
    {
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    [GeneratedRegex("[^a-zA-Z ]+")]
    private static partial Regex CleanRegex();
}

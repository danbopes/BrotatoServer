namespace SearchEngine;

public enum ResultType
{
    Fuzzy = 0,
    Contains = 1,
    StartsWith = 2,
    Initials = 3,
}

public interface ISearchResults<out T> where T : ISearchableObject
{
    string SearchTerm { get; }
    ResultType ResultType { get; }
    int TotalHits { get; }
    IEnumerable<T> ExactResults { get; }
    IEnumerable<T> Results { get; }
}

public class SearchResults<T> : ISearchResults<T> where T : ISearchableObject
{
    public string SearchTerm { get; }
    public ResultType ResultType { get; }
    public int TotalHits { get; set; }
    public IEnumerable<T> ExactResults
    {
        get
        {
            return HasMultipleObjects() ? Results.Where(c => c.Name.Equals(SearchTerm, StringComparison.InvariantCultureIgnoreCase)) : Results;
        }
    }
    public IEnumerable<T> Results { get; }

    public SearchResults(string searchTerm) : this(searchTerm, new List<T>(), 0) { }

    public SearchResults(string searchTerm, IEnumerable<T> results, int totalHits = 0) : this(searchTerm, ResultType.Contains, results, totalHits) { }

    public SearchResults(string searchTerm, ResultType type, IEnumerable<T> results, int totalHits = 0)
    {
        results = results.ToList();
        ResultType = type;
        TotalHits = totalHits > 0 ? totalHits : results.Count();
        SearchTerm = searchTerm;
        Results = results;
    }

    private bool HasMultipleObjects()
    {
        return Results.Any() && Results.Any(result => result.Name != Results.First().Name);
    }
}
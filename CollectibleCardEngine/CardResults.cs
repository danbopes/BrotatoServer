namespace CollectibleCardEngine;

public enum ResultType
{
    Fuzzy = 0,
    Contains = 1,
    StartsWith = 2,
    Initials = 3,
}

public interface ICardResults<out T> where T : ICollectibleCard
{
    string SearchTerm { get; }
    ResultType ResultType { get; }
    int TotalHits { get; }
    IEnumerable<T> ExactResults { get; }
    IEnumerable<T> Results { get; }
}

public class CardResults<T> : ICardResults<T> where T : ICollectibleCard
{
    public string SearchTerm { get; }
    public ResultType ResultType { get; }
    public int TotalHits { get; set; }
    public IEnumerable<T> ExactResults
    {
        get
        {
            return HasMultipleCards() ? Results.Where(c => c.Name.Equals(SearchTerm, StringComparison.InvariantCultureIgnoreCase)) : Results;
        }
    }
    public IEnumerable<T> Results { get; }

    public CardResults(string searchTerm) : this(searchTerm, new List<T>(), 0) { }

    public CardResults(string searchTerm, IEnumerable<T> results, int totalHits = 0) : this(searchTerm, ResultType.Contains, results, totalHits) { }

    public CardResults(string searchTerm, ResultType type, IEnumerable<T> results, int totalHits = 0)
    {
        results = results.ToList();
        ResultType = type;
        TotalHits = totalHits > 0 ? totalHits : results.Count();
        SearchTerm = searchTerm;
        Results = results;
    }

    private bool HasMultipleCards()
    {
        return Results.Any() && Results.Any(result => result.Name != Results.First().Name);
    }

    /*public static implicit operator CardResults<CollectibleCard>(CardResults<T> result)
    {
        return result;
    }*/
}
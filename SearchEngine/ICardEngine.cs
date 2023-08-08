namespace SearchEngine;

public interface ICardEngine
{
    int CardCount { get; }
    public string Name { get; }
    Task InitializeAsync();
    Task<ISearchResults<ISearchableObject>> FindAsync(string name);
    Task<ISearchableObject?> GetRandomAsync();
}

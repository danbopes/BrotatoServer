namespace SearchEngine;

public interface ISearchEngine
{
    int ItemCount { get; }
    public string Name { get; }
    Task InitializeAsync();
    ISearchResults<ISearchableObject> Find(string objName);
    Task<ISearchableObject?> GetRandomAsync();
}

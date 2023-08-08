namespace SearchEngine;

public interface ISearchableObject
{
    string Culture { get; }
    string Id { get; }
    string Name { get; }
    string TextOutput { get; }
}
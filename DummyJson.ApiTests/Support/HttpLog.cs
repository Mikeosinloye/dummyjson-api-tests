namespace DummyJson.ApiTests.Support;

public sealed class HttpLog
{
    private readonly List<string> _entries = [];

    public IReadOnlyList<string> Entries => _entries;

    public void Add(string entry) => _entries.Add(entry);
}

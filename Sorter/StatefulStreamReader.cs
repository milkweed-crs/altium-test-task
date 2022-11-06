using System.Text;

namespace Sorter;

internal sealed class StatefulStreamReader : IDisposable
{
    private const int DefaultBufferSize = 16384;
    private readonly StreamReader _streamReader;

    internal string? CurrentLine { get; private set; }

    private StatefulStreamReader(
        string path,
        Encoding encoding)
    {
        _streamReader = new StreamReader(path, encoding, false, DefaultBufferSize);
    }

    internal static StatefulStreamReader CreateAndInit(
        string path,
        Encoding encoding)
    {
        var instance = new StatefulStreamReader(path, encoding);
        instance.ReadLine();
        return instance;
    }

    internal void ReadLine()
    {
        CurrentLine = _streamReader.ReadLine();
    }

    public void Dispose()
    {
        _streamReader.Dispose();
    }
}
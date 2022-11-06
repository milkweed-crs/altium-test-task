using System.Text;
using Generator;
using Sorter;

namespace Tests;

public sealed class ExternalMergeSorterTests : IDisposable
{
    private readonly string _notSortedFilePath = Path.GetRandomFileName();
    private readonly string _sortedFilePath = Path.GetRandomFileName();

    public ExternalMergeSorterTests()
    {
        FileGenerator.Generate(_notSortedFilePath, (decimal) 0.1, 100, 5, Encoding.UTF8);
    }

    public void Dispose()
    {
        File.Delete(_notSortedFilePath);
        File.Delete(_sortedFilePath);
    }

    [Fact]
    public void SortsFile()
    {
        //Arrange
        var comparer = new TextThenNumberComparer();
        var sorter = new ExternalMergeSorter((float) 0.2, Encoding.UTF8);
        //Act
        sorter.Sort(_notSortedFilePath, _sortedFilePath);
        //Assert
        using var reader = new StreamReader(_sortedFilePath);
        var prevLine = reader.ReadLine();
        var curLine = reader.ReadLine();
        while (curLine != null)
        {
            Assert.True(
                comparer.Compare(prevLine, curLine) <= 0,
                "in sorted file previous line should be < than next line");
            prevLine = curLine;
            curLine = reader.ReadLine();
        }
    }

    [Fact]
    public void PreservesLineCountInSortedFile()
    {
        //Arrange
        var sorter = new ExternalMergeSorter((float) 0.2, Encoding.UTF8);
        //Act
        sorter.Sort(_notSortedFilePath, _sortedFilePath);
        //Assert
        using var unsortedReader = new StreamReader(_sortedFilePath);
        var unsortedLinesCount = 0;
        while (unsortedReader.ReadLine() != null)
            unsortedLinesCount++;

        using var sortedReader = new StreamReader(_sortedFilePath);
        var sortedLinesCount = 0;
        while (sortedReader.ReadLine() != null)
            sortedLinesCount++;

        Assert.Equal(sortedLinesCount, unsortedLinesCount);
    }
}
using System.Text;
using Generator;

namespace Tests;

public sealed class FileGeneratorTests : IDisposable
{
    private readonly string _outputFilePath = Path.GetRandomFileName();
    private const decimal FileSizeInGb = (decimal) 0.1;
    private const int MaxLineLength = 100;

    public FileGeneratorTests()
    {
        FileGenerator.Generate(_outputFilePath, FileSizeInGb, MaxLineLength, 5, Encoding.UTF8);
    }

    public void Dispose()
    {
        File.Delete(_outputFilePath);
    }

    [Fact]
    public void GeneratesFileOfRequestedSize()
    {
        //Arrange
        var allowedError = MaxLineLength;
        //Assert
        Assert.True(FileSizeInGb * 1024 * 1024 * 1024 - File.ReadAllBytes(_outputFilePath).Length < allowedError);
    }
}
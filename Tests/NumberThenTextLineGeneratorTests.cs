using Generator;

namespace Tests;

public class NumberThenTextLineGeneratorTests
{
    [Fact]
    public void GeneratesLineOfNumberThenTextSeparatedByDotAndSpace()
    {
        //Arrange
        var generator = new NumberThenTextLineGenerator(100);
        //Act
        var result = generator.Generate();
        //Assert
        var parts = result.Split(". ");
        Assert.True(parts[0].All(char.IsDigit));
        Assert.True(parts[1].TrimEnd(Environment.NewLine.ToCharArray()).All(x => char.IsLetter(x) || x == ' '));
    }

    [Fact]
    public void GeneratesLineWithLengthLessThanMaxLength()
    {
        //Arrange
        var maxLength = 100;
        var generator = new NumberThenTextLineGenerator(maxLength);
        //Act
        var results = Enumerable.Range(0, 100).Select(_ => generator.Generate()).ToArray();
        //Assert
        Assert.True(results.All(x => x.Length < maxLength));
    }

    [Fact]
    public void GeneratesLineWithSameText_WhenGeneratingWithDuplicateText()
    {
        //Arrange
        var generator = new NumberThenTextLineGenerator(100);
        var line = generator.Generate();
        //Act
        var duplicateTextLine = generator.GenerateWithDuplicateText();
        //Assert
        Assert.Equal(line[(line.IndexOf(' ') + 1)..], duplicateTextLine[(duplicateTextLine.IndexOf(' ') + 1)..]);
    }

    [Fact]
    public void GeneratesRandomLineAsAFallback_WhenAskedToGenerateDuplicate_AndNoPreviousLinePresent()
    {
        //Arrange
        var generator = new NumberThenTextLineGenerator(100);
        //Act
        var line = generator.GenerateWithDuplicateText();
        //Assert
        Assert.NotEmpty(line);
    }

    [Fact]
    public void ThrowsArgumentException_WhenTryingToInitializeWithMaxLengthLessThanMinPossibleLength()
    {
        //Arrange
        const int minNumberLength = 1;
        const int minTextLength = 1;
        var separatorLength = ". ".Length;
        var newLineLength = Environment.NewLine.Length;
        var minLength = minNumberLength + minTextLength + separatorLength + newLineLength;
        //Assert
        Assert.Throws<ArgumentException>(() => new NumberThenTextLineGenerator(minLength - 1));
    }
}
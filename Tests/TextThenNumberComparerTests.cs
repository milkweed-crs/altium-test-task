using Sorter;

namespace Tests;

public class TextThenNumberComparerTests
{
    private static readonly TextThenNumberComparer Comparer = new();

    [Theory]
    [InlineData("123", "Test")]
    public void ReturnsZero_WhenLinesAreEqual(
        string number,
        string text)
    {
        //Arrange
        var x = FormatLine(number, text);
        var y = FormatLine(number, text);
        //Act
        var result = Comparer.Compare(x, y);
        //Assert
        Assert.Equal(0, result);
    }

    [Theory]
    [InlineData("101", "abc", "100", "abd")]
    [InlineData("101", "abc", "100", "bbc")]
    [InlineData("101", "abc", "100", "acc")]
    public void DoesNotCompareNumbers_WhenStringsAreNotEqual(
        string numberX,
        string stringX,
        string numberY,
        string stringY)
    {
        //Arrange
        var x = FormatLine(numberX, stringX);
        var y = FormatLine(numberY, stringY);
        //Act
        var result = Comparer.Compare(x, y);
        //Assert
        Assert.True(result < 0, "line x should be < than line y");
    }


    [Theory]
    [InlineData("1", "abc", "5", "abc")]
    [InlineData("5", "abc", "10", "abc")]
    public void ComparesNumbers_WhenStringsAreEqual(
        string numberX,
        string stringX,
        string numberY,
        string stringY)
    {
        //Arrange
        var x = FormatLine(numberX, stringX);
        var y = FormatLine(numberY, stringY);
        //Act
        var result = Comparer.Compare(x, y);
        //Assert
        Assert.True(result < 0, "line x should be < than line y");
    }

    private static string FormatLine(
        string number,
        string text)
    {
        return $"{number}. {text}";
    }
}
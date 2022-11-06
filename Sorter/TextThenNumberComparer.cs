using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace Sorter;

internal sealed class TextThenNumberComparer : IComparer<string>
{
    public int Compare(
        string x,
        string y)
    {
        return Compare(x, y, StringComparison.Ordinal);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Compare(
        ReadOnlySpan<char> x,
        ReadOnlySpan<char> y,
        StringComparison comparisonType)
    {
        var xSpaceIndex = x.IndexOf(' ');
        var ySpaceIndex = y.IndexOf(' ');

        var xText = x[(xSpaceIndex + 1)..];
        var yText = y[(ySpaceIndex + 1)..];

        var textComparisonResult = xText.CompareTo(yText, comparisonType);
        if (textComparisonResult != 0)
            return textComparisonResult;

        var xNumber = x[..xSpaceIndex];
        var yNumber = y[..ySpaceIndex];

        var numberOfDigitsComparisonResult = xNumber.Length - yNumber.Length;
        if (numberOfDigitsComparisonResult != 0)
            return numberOfDigitsComparisonResult;

        return xNumber.CompareTo(yNumber, comparisonType);
    }
}
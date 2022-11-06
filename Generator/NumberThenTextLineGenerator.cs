namespace Generator;

internal sealed class NumberThenTextLineGenerator
{
    private readonly int _maxLineLength;
    private static readonly string NewLine = Environment.NewLine;
    private static readonly int MinLineLength = MinTextLength + MinNumberLength + Delimiter.Length + NewLine.Length;
    private static readonly int FixedLineLength = Delimiter.Length + NewLine.Length;

    private const string Delimiter = ". ";
    private const string AllowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ";
    private const string AllowedNumbers = "0123456789";
    private const int MinNumberLength = 1;
    private const int MinTextLength = 1;

    private string _lastGeneratedText = string.Empty;

    internal NumberThenTextLineGenerator(int maxLineLength)
    {
        if (maxLineLength < MinLineLength)
            throw new ArgumentException(
                $"Can't initialize generator. Minimum length of line can't be less than {MinLineLength}");

        _maxLineLength = maxLineLength;
    }

    internal string Generate()
    {
        var lineLength = Random.Shared.Next(MinLineLength, _maxLineLength);

        var textLength = Random.Shared.Next(MinTextLength, lineLength - FixedLineLength - MinNumberLength);

        var number = GenerateNumber(lineLength - textLength - FixedLineLength);

        _lastGeneratedText = GenerateText(textLength);

        return $"{number}{Delimiter}{_lastGeneratedText}{NewLine}";
    }

    internal string GenerateWithDuplicateText()
    {
        if (_lastGeneratedText == string.Empty)
            return Generate();

        var lineLength = Random.Shared.Next(
            _lastGeneratedText.Length + FixedLineLength + MinNumberLength,
            _maxLineLength);

        var number = GenerateNumber(lineLength - _lastGeneratedText.Length - FixedLineLength);

        return $"{number}{Delimiter}{_lastGeneratedText}{NewLine}";
    }

    private static string GenerateNumber(int length)
    {
        var chars = new char[length];

        chars[0] = AllowedNumbers[Random.Shared.Next(1, AllowedNumbers.Length)];

        for (var i = 1; i < length; i++)
        {
            chars[i] = AllowedNumbers[Random.Shared.Next(0, AllowedNumbers.Length)];
        }

        return new string(chars);
    }

    private static string GenerateText(int length)
    {
        var chars = new char[length];

        chars[0] = AllowedCharacters[Random.Shared.Next(26)];

        for (var i = 1; i < length; i++)
        {
            var willNotPlaceDuplicateOrTrailingSpace = chars[i - 1] != ' ' && i + 1 < length;

            chars[i] = willNotPlaceDuplicateOrTrailingSpace
                ? AllowedCharacters[Random.Shared.Next(27, AllowedCharacters.Length)]
                : AllowedCharacters[Random.Shared.Next(27, AllowedCharacters.Length - 1)];
        }

        return new string(chars);
    }
}
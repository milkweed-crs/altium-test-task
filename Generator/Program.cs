using System.Diagnostics;
using System.Globalization;
using System.Text;
using Generator;

if (args.Length < 4)
{
    ShowErrorWithHint("Insufficient number of arguments");
    return;
}

var outputFilePath = args[0];
var directory = Path.GetDirectoryName(outputFilePath);
if (directory is null || !Directory.Exists(Path.GetDirectoryName(outputFilePath)))
{
    ShowErrorWithHint("Provided directory does not exist");
    return;
}

if (!decimal.TryParse(args[1], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var sizeInGb) ||
    !int.TryParse(args[2], out var maxLineLength) ||
    !int.TryParse(args[3], out var duplicateChancePercent))
{
    ShowErrorWithHint("Can't parse provided arguments");
    return;
}

Console.WriteLine("Generating...");

var sw = Stopwatch.StartNew();

FileGenerator.Generate(outputFilePath, sizeInGb, maxLineLength, duplicateChancePercent, Encoding.UTF8);

Console.WriteLine($"Generated {outputFilePath} in {sw.Elapsed.TotalSeconds} seconds");

void ShowErrorWithHint(string error)
{
    Console.WriteLine(error);
    Console.WriteLine("Usage with arguments: pathToOutputFile sizeInGb maxLineLength duplicateChancePercent");
    Console.WriteLine(@"Example: D:\generated-file.txt 1 100 5");
}
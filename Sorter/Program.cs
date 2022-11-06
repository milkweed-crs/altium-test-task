using System.Diagnostics;
using System.Globalization;
using System.Text;
using Sorter;

if (args.Length < 3)
{
    ShowErrorWithHint("Insufficient number of arguments");
    return;
}

var inputFilePath = args[0];
if (!File.Exists(inputFilePath))
{
    ShowErrorWithHint("Provided input file does not exist");
    return;
}

var outputFilePath = args[1];
if (!IsValidDirectory(outputFilePath))
{
    ShowErrorWithHint("Provided output directory does not exist");
    return;
}

if (!float.TryParse(args[2], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var bufferSizeInGb))
{
    ShowErrorWithHint("Can't parse buffer size");
    return;
}

var sorter = new ExternalMergeSorter(bufferSizeInGb, Encoding.UTF8);

Console.WriteLine("Sorting...");

var sw = Stopwatch.StartNew();
sorter.Sort(inputFilePath, outputFilePath);

Console.WriteLine($"Sorted {outputFilePath} in {sw.Elapsed.TotalSeconds} seconds");

bool IsValidDirectory(string filePath)
{
    var directory = Path.GetDirectoryName(filePath);
    return directory is not null && Directory.Exists(Path.GetDirectoryName(filePath));
}

void ShowErrorWithHint(string error)
{
    Console.WriteLine(error);
    Console.WriteLine("Usage with arguments: pathToInputFile pathToOutputFile bufferSizeInGb");
    Console.WriteLine(@"Example: D:\unsorted-file.txt D:\sorted-file.txt 0.5");
}
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("Tests")]

namespace Generator;

internal static class FileGenerator
{
    private const decimal BufferSizeGb = (decimal) 0.5;

    internal static void Generate(
        string outputFilePath,
        decimal sizeInGb,
        int maxLineLength,
        int duplicateChancePercent,
        Encoding encoding)
    {
        if (sizeInGb < BufferSizeGb)
        {
            GenerateFile(outputFilePath, sizeInGb, maxLineLength, duplicateChancePercent, encoding);
            return;
        }

        var directory = Path.GetDirectoryName(outputFilePath);

        var chunks = ChunkByAllowedBufferSize(directory, sizeInGb);

        Parallel.ForEach(
            chunks,
            chunk =>
            {
                GenerateFile(
                    chunk.FilePath,
                    chunk.Size,
                    maxLineLength,
                    duplicateChancePercent,
                    encoding);
            });

        foreach (var chunk in chunks)
        {
            using (var input = File.OpenRead(chunk.FilePath))
            using (Stream output = new FileStream(
                       outputFilePath,
                       FileMode.Append,
                       FileAccess.Write,
                       FileShare.None))
            {
                input.CopyTo(output);
            }

            File.Delete(chunk.FilePath);
        }
    }

    private static List<(string FilePath, decimal Size)> ChunkByAllowedBufferSize(
        string directory,
        decimal sizeInGb)
    {
        var chunksNameSize = new List<(string FilePath, decimal Size)>();

        var leftGb = sizeInGb;
        var fileNumber = 0;
        while (leftGb > 0)
        {
            if (leftGb < BufferSizeGb)
            {
                chunksNameSize.Add((Path.Combine(directory, $"{fileNumber}.txt"), leftGb));
                break;
            }

            leftGb -= BufferSizeGb;
            chunksNameSize.Add((Path.Combine(directory, $"{fileNumber}.txt"), BufferSizeGb));
            fileNumber++;
        }

        return chunksNameSize;
    }

    private static void GenerateFile(
        string path,
        decimal sizeInGb,
        int maxLineLength,
        int duplicateChancePercent,
        Encoding encoding)
    {
        var sizeInBytes = sizeInGb * 1024 * 1024 * 1024;

        var generator = new NumberThenTextLineGenerator(maxLineLength);

        using var writer = new StreamWriter(path, false, encoding);
        decimal currentSize = 0;

        AddLine(generator.Generate());
        //ensure we have at least one duplicate
        AddLine(generator.GenerateWithDuplicateText());

        while (currentSize < sizeInBytes)
        {
            var isDuplicate = Random.Shared.Next(100) < duplicateChancePercent;

            AddLine(
                isDuplicate
                    ? generator.GenerateWithDuplicateText()
                    : generator.Generate());
        }

        void AddLine(string line)
        {
            writer.Write(line);
            currentSize += encoding.GetByteCount(line);
        }
    }
}
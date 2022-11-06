using System.Text;

namespace Sorter;

internal sealed class ExternalMergeSorter
{
    private readonly Encoding _encoding;
    private static readonly int NewLineSeparatorLength = Environment.NewLine.Length;
    private static readonly TextThenNumberComparer Comparer = new();

    private readonly float _bufferSizeBytes;

    internal ExternalMergeSorter(
        float bufferSizeInGb,
        Encoding encoding)
    {
        _encoding = encoding;
        _bufferSizeBytes = bufferSizeInGb * 1024 * 1024 * 1024;
    }

    internal void Sort(
        string inputPath,
        string outputPath)
    {
        var sortedFiles = SplitToSortedFiles(inputPath, outputPath);

        MergeSortedFiles(sortedFiles, outputPath);
    }

    private List<string> SplitToSortedFiles(
        string inputPath,
        string outputPath)
    {
        using var reader = new StreamReader(inputPath, _encoding, false, 65536);

        var sortedFiles = new List<string>();

        var unsortedLinesBuffer = new List<string>();
        var sortedLinesBuffer = new List<string>();
        float currentBufferSizeBytes = 0;

        var line = reader.ReadLine();
        while (line != null)
        {
            unsortedLinesBuffer.Add(line);
            currentBufferSizeBytes += line.Length + NewLineSeparatorLength;

            line = reader.ReadLine();
            if (currentBufferSizeBytes < _bufferSizeBytes)
                continue;

            SortBufferAndWriteToFile(unsortedLinesBuffer, sortedLinesBuffer, outputPath, sortedFiles);
            currentBufferSizeBytes = 0;
        }

        SortBufferAndWriteToFile(unsortedLinesBuffer, sortedLinesBuffer, outputPath, sortedFiles);

        return sortedFiles;
    }

    private static void SortBufferAndWriteToFile(
        List<string> unsortedLinesBuffer,
        List<string> sortedLinesBuffer,
        string outputPath,
        List<string> sortedFiles)
    {
        if (unsortedLinesBuffer.Count == 0)
            return;

        Sort(unsortedLinesBuffer, sortedLinesBuffer);
        unsortedLinesBuffer.Clear();

        WriteToFile(sortedLinesBuffer, sortedFiles, outputPath);
        sortedLinesBuffer.Clear();
    }

    private static void Sort(
        List<string> lines,
        List<string> sortedLinesBuffer)
    {
        sortedLinesBuffer.AddRange(
            lines
                .AsParallel()
                .OrderBy(x => x, Comparer));
    }

    private static void WriteToFile(
        List<string> sortedLinesBuffer,
        List<string> sortedFiles,
        string outputPath)
    {
        var fileName = $"{outputPath}.{sortedFiles.Count}.sorted";
        File.WriteAllLines(fileName, sortedLinesBuffer);

        sortedFiles.Add(fileName);
    }

    private void MergeSortedFiles(
        List<string> files,
        string outputPath)
    {
        if (files.Count == 1)
        {
            File.Move(files[0], outputPath);
            return;
        }

        var readers = files
            .Select(file => StatefulStreamReader.CreateAndInit(file, _encoding))
            .ToList();

        readers.Sort((x, y) => Comparer.Compare(x.CurrentLine!, y.CurrentLine!));

        using var writer = new StreamWriter(outputPath, false, _encoding);
        var sb = new StringBuilder(100 * 1024 * 1024);
        while (readers.Count > 0)
        {
            var reader = readers[0];
            readers.RemoveAt(0);

            sb.AppendLine(reader.CurrentLine);
            if (sb.Length >= _bufferSizeBytes)
            {
                writer.Write(sb);
                sb.Clear();
            }

            reader.ReadLine();

            if (reader.CurrentLine != null)
                InsertInOrder(readers, reader);
            else
                reader.Dispose();
        }

        //in case something is left in buffer
        writer.Write(sb);

        foreach (var file in files)
            File.Delete(file);
    }

    private static void InsertInOrder(
        List<StatefulStreamReader> readers,
        StatefulStreamReader reader)
    {
        var left = 0;
        var right = readers.Count - 1;
        while (left <= right)
        {
            var mid = left + (right - left) / 2;
            var order = Comparer.Compare(readers[mid].CurrentLine!, reader.CurrentLine!);

            switch (order)
            {
                case 0:
                    readers.Insert(mid, reader);
                    return;
                case < 0:
                    left = mid + 1;
                    break;
                default:
                    right = mid - 1;
                    break;
            }
        }

        readers.Insert(left, reader);
    }
}
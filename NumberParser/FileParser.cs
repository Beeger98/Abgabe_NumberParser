using NumberParser.Infratructure;
using System.Text;

public class FileParser(ParserConfiguration Config)
{
    /// <summary>
    /// Parse the file at the given location and return the parsed lines in a chunk of configureed height
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    public IEnumerable<string> ParseFile(string location)
    {

        var lines = ReadAndNormalize(location);
        if (lines.Length % Config.LineHeight != 0)
            throw new ArgumentException($"The number of lines with content in the file must be a multiple of {Config.LineHeight}");

        var imageLines = lines.Chunk(Config.LineHeight).Select(chunk =>
        {
            var maxLineLength = chunk.Max(line => line.Length);
            return chunk.Select(line => line.PadRight(maxLineLength, ' ')).ToArray();
        });

        foreach (var imageLine in imageLines)
        {
            var sb = new StringBuilder();

            var imagesBoxes = FindImageBoxes(imageLine);
            foreach (var imageBox in imagesBoxes)
            {
                var boxContent =
                    imageLine
                        .Select(line => line.Substring(imageBox.StartPos, imageBox.Width).ToCharArray())
                        .ToArray();

                var detectedChar =
                    Config.DetectionAlgorithm(boxContent)
                    ?? Config.UnknownDetectedCharacter;

                if (detectedChar.HasValue)
                    sb.Append(detectedChar.Value);
            }

            yield return sb.ToString();
        }
    }



    /// <summary>
    /// Get the position of Numberboxes
    /// </summary>
    /// <param name="imageLines"></param>
    /// <returns></returns>
    private static IEnumerable<(int StartPos, int Width)> FindImageBoxes(string[] imageLines)
    {
        var rowsWithData = FindDataColumns(imageLines, 0, imageLines.Length - 1).ToArray();
        var detectedChars = FindChars(rowsWithData);

        return detectedChars;
    }

    /// <summary>
    /// Find the start and width of character segments in a boolean array
    /// </summary>
    /// <param name="array"></param>
    /// <returns></returns>
    private static IEnumerable<(int Start, int Width)> FindChars(bool[] array)
    {
        if (array.Length == 0) yield break;
        var current = array[0];
        var start = 0;

        for (var i = 1; i < array.Length; i++)
            if (array[i] != current)
            {
                if (current)
                    yield return (start, i - start);

                current = array[i];
                start = i;
            }

        if (current)
            yield return (start, array.Length - start);
    }

    /// <summary>
    /// Find columns that contain data (non-space characters) in the specified range of lines
    /// </summary>
    /// <param name="lines"></param>
    /// <param name="firstLineNo"></param>
    /// <param name="lastLineNo"></param>
    /// <returns></returns>
    private static IEnumerable<bool> FindDataColumns(string[] lines, int firstLineNo, int lastLineNo)
    {
        var lineNos = Enumerable.Range(firstLineNo, lastLineNo - firstLineNo + 1).ToArray();
        var maxLineLength =
            lineNos
                .Select(no => lines[no].Length)
                .Max();


        for (var colNr = 0; colNr < maxLineLength; colNr++)
        {
            var anyChar = lineNos.Any(l => lines[l].Length > colNr && lines[l][colNr] != ' ');
            yield return anyChar;
        }
    }


    /// <summary>
    /// Prepare the lines by reading and normalizing them
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    private static string[] ReadAndNormalize(string location)
    {
        var lines =
       File.ReadAllLines(location)
         .Select(StringCleaner.DropTabs)
         .Where(line => !string.IsNullOrEmpty(line))
         .ToArray();

        return lines;
    }

}


public class ParserConfiguration
{
    public int LineHeight { get; init; }

    public CharacterDetection DetectionAlgorithm { get; init; }

    public char? UnknownDetectedCharacter { get; init; }


    public delegate char? CharacterDetection(char[][] characterImage);
}
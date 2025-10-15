/// <summary>
/// Class for detecting characters represented in a four-line format
/// </summary>
public static class FourLinesCharacters
{
    /// <summary>
    /// Dictionary of known characters represented in a format
    /// </summary>
    private static Dictionary<char, string> KnownChars = new() {
        { '1', "|\n|\n|\n|"},
        { '2', "---\n _|\n|  \n---"},
        { '3', "---\n / \n \\ \n-- "},
        { '4', "|   |\n|___|\n    |\n    |"},
        { '5', "-----\n|___ \n    |\n____|"}
    };


    /// <summary>
    /// Detects a character from a  character image
    /// </summary>
    /// <param name="charImage"></param>
    /// <returns></returns>
    public static char? DetectChar(char[][] charImage)
    {

        var detectedChar =  KnownChars.FirstOrDefault(kvp => IsMatch(kvp.Value, charImage)).Key;
        return detectedChar == '\0' ? null : detectedChar;
    }

    /// <summary>
    /// Check if the provided character image matches the known character representation
    /// </summary>
    /// <param name="x"></param>
    /// <param name="box"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private static bool IsMatch(string x, char[][] box)
    {
        if (string.IsNullOrWhiteSpace(x))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(x));
    
        var b = string.Join('\n', box.Select(line => new string(line)).ToArray());

        var isMatch =  x == b;
        return isMatch;
    }
}

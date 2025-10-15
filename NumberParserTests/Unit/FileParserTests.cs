using Shouldly;
using System.Runtime.InteropServices;


[TestFixture]
public class FileParserTests
{
    /// <summary>
    /// Parse files with numbers from 1 to 5 and error character X including more lines
    /// </summary>
    /// <param name="path"></param>
    /// <param name="expectedValue"></param>
    [TestCase("./samples/1.txt", "1")]
    [TestCase("./samples/2.txt", "2")]
    [TestCase("./samples/3.txt", "3")]
    [TestCase("./samples/4.txt", "4")]
    [TestCase("./samples/5.txt", "5")]
    [TestCase("./samples/X.txt", "X")]
    [TestCase("./samples/451.txt", "451\n451")]
    [TestCase("./samples/45X.txt", "45X")]
    public void ParseNumberOnetoFiveAndErrorTestParseFile_ShouldReturnExpectedValue_WhenParsingLocalFile(string path, string expectedValue)
    {
        // Arrange
        var config = new ParserConfiguration
        {
            LineHeight = 4,
            DetectionAlgorithm = FourLinesCharacters.DetectChar,
            UnknownDetectedCharacter = 'X'
        };

        var parser = new FileParser(config);
        File.Exists(path).ShouldBeTrue($"Expected test file at '{path}'.");

        // Act
        var parsedLines = parser.ParseFile(path).ToArray();
        var value = string.Join('\n', parsedLines);

        // Assert
        value.ShouldBe(expectedValue);
    }

    /// <summary>
    /// If on current rules are more lines with content than multiple of LineHeight
    /// </summary>
    [Test]
    public void ParseFile_ShouldThrowException_IfSourceFileContainsInvalidLineCount()
    {
        // Arrange
        var config = new ParserConfiguration
        {
            LineHeight = 4,
            DetectionAlgorithm = FourLinesCharacters.DetectChar,
            UnknownDetectedCharacter = 'X'
        };

        string path = "./samples/invalidLineCount.txt";

        var parser = new FileParser(config);
        File.Exists(path).ShouldBeTrue($"Expected test file at '{path}'.");

        // Act & Assert
        var callToTest = () => parser.ParseFile(path).ToArray();
        Should
            .Throw<ArgumentException>(callToTest)
            .Message.ShouldStartWith("The number of lines with content");
    }
}

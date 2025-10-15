using CliWrap;
using CliWrap.Buffered;
using Shouldly;

/// <summary>
/// Make sure the output with the user workflow is as expected
/// </summary>
[TestFixture]
public class CliCalls
{
    [TestCase("./samples/1.txt", "1")]
    [TestCase("./samples/2.txt", "2")]
    [TestCase("./samples/3.txt", "3")]
    [TestCase("./samples/4.txt", "4")]
    [TestCase("./samples/5.txt", "5")]
    [TestCase("./samples/X.txt", "X")]
    [TestCase("./samples/451.txt", "451\n451")]
    [TestCase("./samples/45X.txt", "45X")]
    public async Task InvocationOfCliTool_ShouldReturnExpectedValue_IfSourceFilePassedInAsParameter(string source, string expectedValue)
    {
        //arrange
        var command = Cli.Wrap("NumberParser.exe")
            .WithArguments(source)
            .WithValidation(CommandResultValidation.ZeroExitCode);

        //act
        var result = await command.ExecuteBufferedAsync();

        //assert
        var stdOutValue =
            result.StandardOutput
            .Trim()
            .Replace(Environment.NewLine, "\n");

        stdOutValue.ShouldBe(expectedValue);
    }
}

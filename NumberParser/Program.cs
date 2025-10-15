
namespace NumberParser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //parsing configurationn for current file , Numbers 1 to 5 with height of 4 lines
            var config = new ParserConfiguration { 
                LineHeight = 4, 
                DetectionAlgorithm = FourLinesCharacters.DetectChar ,
                UnknownDetectedCharacter = 'X'

            };
            
            var fileToParse = (args.Length == 1) ? args.Single() : "./numbers.txt";
            var parser = new FileParser(config);
            var parsedLines = parser.ParseFile(fileToParse);

            parsedLines.ToList().ForEach(Console.WriteLine);

        }

    }
}

global using System;
global using System.IO;
global using System.Linq;
global using LanguageExt;
global using LanguageExt.Common;
global using static LanguageExt.Prelude;
global using LanguageExt.Parsec;
using LanguageExt.Pipes;

namespace RobotApp;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0) throw new Exception(
            "No file path passed in as first argument.");
        var path = args[0];

        var fileContent = File.ReadAllText(path);
        
        var parseResult = Parser.gridJourneys.Parse(fileContent);
        if (parseResult.IsFaulted) throw new Exception("Could not parse the file.");

        var grid = parseResult.Reply.Result.grid;
        var journeys = parseResult.Reply.Result.journeys;

        var results = journeys.Map(j => 
            RobotJourney.journeyResult(grid, j.start, j.expected));

        var output = string.Join("\n", results);
        System.Console.WriteLine(output);
    }
}

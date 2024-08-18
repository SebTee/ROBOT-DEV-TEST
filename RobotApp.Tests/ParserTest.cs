namespace RobotApp.Tests;

using Journey = (Robot, Robot);

public class ParserTest
{
    [Theory]
    [InlineData("1  1  E", 1, 1, Direction.East)]
    [InlineData("10 67 S L", 10, 67, Direction.South, Command.Left)]
    [InlineData("0  100 W\nRFL", 0, 100, Direction.West, 
        Command.Right, 
        Command.Forward, 
        Command.Left
    )]
    [InlineData("37 0 N FF", 37, 0, Direction.North, 
        Command.Forward, 
        Command.Forward
    )]
    public void ParseRobotSuccess(
        string input, 
        int x, 
        int y,
        Direction direction,
        params Command[] commands
    )
    {
        var expected = new Robot((x, y), direction, commands.ToSeq());

        var result = Parser.robot.Parse(input);

        Assert.False(result.IsFaulted);
        Assert.Equal(expected, result.Reply.Result);
    }

    [Theory]
    [InlineData("-1 1  E")]
    [InlineData("1  -1 S")]
    [InlineData("1  1  B")]
    [InlineData("A  B  N")]
    [InlineData("1  1  n")]
    [InlineData("1  1  e")]
    [InlineData("1  1  s")]
    [InlineData("1  1  w")]
    [InlineData("1  1")]
    [InlineData("1  N")]
    [InlineData("1")]
    [InlineData("")]
    public void ParseRobotFailure(string input)
    {
        var result = Parser.robot.Parse(input);
        Assert.True(result.IsFaulted);
    }
    
    public static IEnumerable<object[]> ParseGridSuccessTestCases = [
        ["GRID 4x3", 4, 3],
        ["GRID 10x457\nOBSTACLE 1 2", 10, 457, (1, 2)],
        ["GRID 3x3 OBSTACLE 10 3\nOBSTACLE 3 5", 3, 3, (10, 3), (3, 5)]
    ];

    [Theory, MemberData(nameof(ParseGridSuccessTestCases))]
    public void ParseGridSuccess(
        string input, 
        int maxX, 
        int maxY,
        params (int, int)[] obstacles
    )
    {
        var expected = new Grid((maxX, maxY), 
            LanguageExt.Set.createRange(obstacles));

        var result = Parser.grid.Parse(input);

        Assert.False(result.IsFaulted);
        Assert.Equal(expected, result.Reply.Result);
    }

    [Theory]
    [InlineData("GRID 4 3")]
    [InlineData("GRiD 4x3")]
    [InlineData("GRID 4x3 OBSTACLE 1x2")]
    [InlineData("GRID 4x3 OBSTACLE 1 2 OBStACLE 2 3")]
    [InlineData("GRID 4x")]
    [InlineData("GRID 4")]
    [InlineData("GRID")]
    [InlineData("")]
    public void ParseGridFailure(string input)
    {
        var result = Parser.grid.Parse(input);
        Assert.True(result.IsFaulted);
    }

    [Fact]
    public void ParseGridJourneysSuccess()
    {
        var input = @"
            GRID 10x10

            OBSTACLE 1 2
            OBSTACLE 2 3

            1 1 E
            RFRL
            1 2 W

            3 2 S
            FRL
            5 3 N
        ";

        var expectedGrid = new Grid((10, 10), 
            LanguageExt.Set.createRange([(1, 2), (2, 3)]));

        Seq<Journey> expectedJourney = Seq([
            (
                new Robot((1, 1), Direction.East, Seq([
                    Command.Right,
                    Command.Forward,
                    Command.Right,
                    Command.Left
                ])),
                new Robot((1, 2), Direction.West)
            ),
            (
                new Robot((3, 2), Direction.South, Seq([
                    Command.Forward,
                    Command.Right,
                    Command.Left
                ])),
                new Robot((5, 3), Direction.North)
            )
        ]);

        var result = Parser.gridJourneys.Parse(input);

        Assert.False(result.IsFaulted);
        var gridJourneys = result.Reply.Result;
        Assert.Equal(expectedGrid, gridJourneys.grid);
        Assert.Equal(expectedJourney, gridJourneys.journeys);
    }
}
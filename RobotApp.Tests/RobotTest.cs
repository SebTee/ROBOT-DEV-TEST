namespace RobotApp.Tests;

public class RobotTest
{
    [Theory]
    [InlineData(Direction.North, 1, 2)]
    [InlineData(Direction.East, 2, 1)]
    [InlineData(Direction.South, 1, 0)]
    [InlineData(Direction.West, 0, 1)]
    public void Move(Direction direction, int x, int y)
    {
        var robot = new Robot((1, 1), direction, new ([Command.Forward]));
        robot.Step().ShouldBeSome(r => {
            Assert.Equal(x, r.X);
            Assert.Equal(y, r.Y);
            Assert.True(r.Commands.IsEmpty);
        });
    }
    
    [Theory]
    [InlineData(Direction.North, Direction.West)]
    [InlineData(Direction.East, Direction.North)]
    [InlineData(Direction.South, Direction.East)]
    [InlineData(Direction.West, Direction.South)]
    public void TurnLeft(Direction start, Direction end) =>
        turn(Command.Left, start, end);
    
    [Theory]
    [InlineData(Direction.North, Direction.East)]
    [InlineData(Direction.East, Direction.South)]
    [InlineData(Direction.South, Direction.West)]
    [InlineData(Direction.West, Direction.North)]
    public static void TurnRight(Direction start, Direction end) =>
        turn(Command.Right, start, end);

    private static void turn(Command turn, Direction start, Direction end)
    {
        var robot = new Robot((0, 0), start, new ([turn]));
        robot.Step().ShouldBeSome(r => {
            Assert.Equal(end, r.Direction);
            Assert.True(r.Commands.IsEmpty);
        });
    }

    public static IEnumerable<object[]> JourneyTestCases = [
        [
            new Robot((0, 0), Direction.North, Seq([
                Command.Forward,
                Command.Right,
                Command.Forward,
                Command.Left,
                Command.Forward,
                Command.Left
            ])),
            new Grid((10, 10)),
            PositionStatus.Safe,
            new Robot((1, 2), Direction.West)
        ],
        [
            new Robot((0, 0), Direction.North, Seq([
                Command.Forward,
                Command.Right,
                Command.Forward,
                Command.Left,
                Command.Forward,
                Command.Left
            ])),
            new Grid((10, 10), Set((1, 2))),
            PositionStatus.Obstacle,
            new Robot((1, 2), Direction.North, Seq([Command.Left]))
        ],
        [
            new Robot((0, 0), Direction.North, Seq([
                Command.Forward,
                Command.Forward,
                Command.Forward,
                Command.Forward
            ])),
            new Grid((10, 3)),
            PositionStatus.OutOfBounds,
            new Robot((0, 3), Direction.North, Seq([Command.Forward]))
        ]
    ];
    
    [Theory, MemberData(nameof(JourneyTestCases))]
    public void JourneyTest(
        Robot robot,
        Grid grid,
        PositionStatus expectedStatus,
        Robot expectedFinalState
    )
    {
        var result = robot.Journey(grid);

        Assert.Equal(expectedStatus, result.status);
        Assert.Equal(expectedFinalState, result.finalState);
    }

    public static IEnumerable<object[]> JourneyResultTestCases = [
        [
            PositionStatus.OutOfBounds,
            new Robot((-1, 0), Direction.West),
            new Robot((1, 1), Direction.North),
            "OUT OF BOUNDS"
        ],
        [
            PositionStatus.Obstacle,
            new Robot((1, 0), Direction.West),
            new Robot((1, 1), Direction.North),
            "CRASHED 1 0"
        ],
        [
            PositionStatus.Safe,
            new Robot((1, 0), Direction.West),
            new Robot((1, 1), Direction.North),
            "FAILURE 1 0 W"
        ],
        [
            PositionStatus.Safe,
            new Robot((1, 1), Direction.North),
            new Robot((1, 1), Direction.North),
            "SUCCESS 1 1 N"
        ],
    ];

    [Theory, MemberData(nameof(JourneyResultTestCases))]
    public void FormatJourneyResultTest(
        PositionStatus status, 
        Robot final, 
        Robot expected, 
        string expectedMessage
    )
    {
        var message = RobotJourney.formatJourneyResult(status, final, expected);
        Assert.Equal(expectedMessage, message);
    }
}
namespace RobotApp;

public enum Direction
{
    North,
    East,
    South,
    West
}

public static class DirectionExtention
{
    public static Direction Left(this Direction direction)
    {
        return direction switch 
        {
            Direction.North => Direction.West,
            Direction.East => Direction.North,
            Direction.South => Direction.East,
            Direction.West => Direction.South,
            _ => throw new ArgumentException()
        };
    }

    public static Direction Right(this Direction direction)
    {
        return direction switch 
        {
            Direction.North => Direction.East,
            Direction.East => Direction.South,
            Direction.South => Direction.West,
            Direction.West => Direction.North,
            _ => throw new ArgumentException()
        };
    }

    public static Pos Move (this Direction direction)
    {
        return direction switch 
        {
            Direction.North => (0, 1),
            Direction.East => (1, 0),
            Direction.South => (0, -1),
            Direction.West => (-1, 0),
            _ => throw new ArgumentException()
        };
    }

    public static string Letter (this Direction direction)
    {
        return direction switch 
        {
            Direction.North => "N",
            Direction.East => "E",
            Direction.South => "S",
            Direction.West => "W",
            _ => throw new ArgumentException()
        };
    }
}

public enum Command
{
    Left,
    Right,
    Forward
}
    
public readonly struct Robot
{
    public Robot(Pos pos, Direction direction)
    {
        X = pos.x;
        Y = pos.y;
        Direction = direction;
    }

    public Robot(
        (int, int) pos, 
        Direction direction, 
        Seq<Command> commands
    ) : this(pos, direction)
    {
        Commands = commands;
    }

    public int X { get; init; }
    public int Y { get; init; }
    public Direction Direction { get; init; }
    public Seq<Command> Commands { get; init; } = Empty;
    public Pos Pos => (X, Y);
}

public static class RobotJourney
{
    public static Option<Robot> Step(this Robot robot)
    {
        var commands = robot.Commands;

        if (commands.IsEmpty) return None;
        
        var move = robot.Direction.Move();

        return commands.Head switch
        {
            Command.Left => robot with {
                Commands = commands.Tail,
                Direction = robot.Direction.Left()
            },
            Command.Right => robot with {
                Commands = commands.Tail,
                Direction = robot.Direction.Right()
            },
            Command.Forward => robot with {
                Commands = commands.Tail,
                X = robot.X + move.x,
                Y = robot.Y + move.y
            },
            _ => throw new ArgumentException()
        };
    }

    public static (PositionStatus status, Robot finalState) Journey(
        this Robot robot, 
        Grid grid) =>
            grid[robot.Pos] switch
            {
                PositionStatus.Safe =>
                    robot.Step().Match(
                        None: () => (PositionStatus.Safe, robot),
                        Some: r => r.Journey(grid)
                    ),
                var failureStatus => (failureStatus, robot)
            };

    public static string journeyResult(Grid grid, Robot start, Robot expected)
    {
        var result = start.Journey(grid);

        return formatJourneyResult(result.status, result.finalState, expected);
    }

    public static string formatJourneyResult(
        PositionStatus status, 
        Robot final, 
        Robot expected
    ) =>
        status switch
        {
            PositionStatus.OutOfBounds => "OUT OF BOUNDS",
            PositionStatus.Obstacle => $"CRASHED {final.X} {final.Y}",
            PositionStatus.Safe => (final.Equals(expected) ? "SUCCESS" : "FAILURE") +
                $" {final.X} {final.Y} {final.Direction.Letter()}",
            _ => throw new ArgumentException()
        };
}
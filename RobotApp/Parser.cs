using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;
using LanguageExt.UnsafeValueAccess;

namespace RobotApp;

using Journey = (Robot start, Robot expected);

public static class Parser
{
    public static Parser<(Grid grid, Seq<Journey> journeys)> gridJourneys =>
        from _1 in spaces
        from g in grid
        from _2 in spaces
        from j in journeys
        select (g, j);

    public static Parser<Grid> grid =>
        from maxPos in gridSize
        from _1 in spaces
        from obs in obstacles
        select new Grid(maxPos, LanguageExt.Set.createRange(obs));

    public static Parser<Robot> robot =>
        from p in position
        from _1 in spaces
        from d in direction
        from _2 in spaces
        from c in commands
        select new Robot(p, d, c);

    private static Parser<(int, int)> position =>
        from x in integer
        from _1 in spaces
        from y in integer
        select (x, y);

    private static Parser<int> integer =>
        from x in asInteger(many1(digit))
        select x.Value();

    private static HashMap<char, Direction> directionMap =>
        new ([
            ('N', Direction.North),
            ('E', Direction.East),
            ('S', Direction.South),
            ('W', Direction.West)
        ]);

    private static Parser<Direction> direction =>
        from d in oneOf(string.Concat(directionMap.Keys))
        select directionMap[d];

    private static HashMap<char, Command> commandMap =>
        new ([
            ('L', Command.Left),
            ('R', Command.Right),
            ('F', Command.Forward)
        ]);

    private static Parser<Command> command =>
        from c in oneOf(string.Concat(commandMap.Keys))
        select commandMap[c];

    private static Parser<Seq<Command>> commands =>
        many(command);

    private static Parser<(int, int)> gridSize =>
        from _1 in str("GRID")
        from _2 in spaces
        from x in integer
        from _3 in ch('x')
        from y in integer
        select (x, y);

    private static Parser<(int, int)> obstacle =>
        from _1 in str("OBSTACLE")
        from _2 in spaces
        from pos in position
        select pos;

    private static Parser<Seq<T>> spaceSeperatedMany<T>(Parser<T> p) =>
        many(
            from x in p
            from _1 in spaces
            select x
        );

    private static Parser<Seq<(int, int)>> obstacles =>
        spaceSeperatedMany(obstacle);

    private static Parser<Journey> journey =>
        from r1 in robot
        from _1 in spaces
        from r2 in robot
        select (r1, r2);

    private static Parser<Seq<Journey>> journeys =>
        spaceSeperatedMany(journey);
}
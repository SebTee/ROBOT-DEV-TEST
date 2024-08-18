global using Pos = (int x, int y);

namespace RobotApp;

public enum PositionStatus
{
    OutOfBounds,
    Obstacle,
    Safe
}

public readonly struct Grid
{
    public Grid(Pos maxPos)
    {
        MaxX = maxPos.x;
        MaxY = maxPos.y;
    }

    public Grid(Pos maxPos, Set<(int, int)> obstacles)
    : this(maxPos)
    {
        Obstacles = obstacles;
    }

    public int MaxX { get; init; }
    public int MaxY { get; init; }
    public Set<(int, int)> Obstacles { get; init; } = Empty;

    public PositionStatus this[Pos pos]
    {
        get 
        {
            if (pos.x < 0 || pos.x >= MaxX || pos.y < 0 || pos.y >= MaxY)
                return PositionStatus.OutOfBounds;
            else if (Obstacles.Contains((pos.x, pos.y)))
                return PositionStatus.Obstacle;
            else return PositionStatus.Safe;
        }
    }
}
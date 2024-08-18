namespace RobotApp.Tests;

public class GridTest
{
    [Theory]
    [InlineData(0, 0, PositionStatus.Safe)]
    [InlineData(1, 1, PositionStatus.Safe)]
    [InlineData(4, 3, PositionStatus.Safe)]
    [InlineData(6, 4, PositionStatus.Safe)]
    [InlineData(7, 4, PositionStatus.OutOfBounds)]
    [InlineData(4, 5, PositionStatus.OutOfBounds)]
    [InlineData(8, 4, PositionStatus.OutOfBounds)]
    [InlineData(4, 6, PositionStatus.OutOfBounds)]
    [InlineData(-1, 4, PositionStatus.OutOfBounds)]
    [InlineData(4, -1, PositionStatus.OutOfBounds)]
    [InlineData(2, 2, PositionStatus.Obstacle)]
    public void PositionStatusTest(
        int x, int y,
        PositionStatus expected
    )
    {
        var grid = new Grid((7, 5), Set((2, 2)));

        var result = grid[(x, y)];
        
        Assert.Equal(expected, result);
    }
}
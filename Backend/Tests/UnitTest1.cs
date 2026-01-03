namespace Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        // arrange

        // act

        Assert.Equal(1, 1);
    }

    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(5, 5, 10)]
    public void Test2(int x, int y, int expected)
    {
        // act
        var result = x + y;

        // assert
        Assert.Equal(expected, result);
    }
}

namespace GameTests;

public class GameTests
{
    private readonly IServiceProvider provider;

    public GameTests()
    {
        this.provider = GameServiceCollection.Initialize();
    }

    [Fact]
    public void PlayerHasPosition()
    {
        var state = this.provider.GetRequiredService<StateService>();
        
        Assert.Equal(Vector2.Zero, state.PlayerPosition);
    }

    [Fact]
    public void CanMovePlayerDown()
    {
        var game = this.provider.GetRequiredService<XnaGame>();
        var state = this.provider.GetRequiredService<StateService>();

        state.ControllerDirection = Vector2.UnitX;

        game.Tick();

        Assert.GreaterThan(0, state.PlayerPosition.X);
    }
}
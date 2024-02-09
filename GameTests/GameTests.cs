namespace GameTests;

public class GameTests
{
    private readonly IServiceProvider provider;

    public GameTests()
    {
        provider = GameServiceCollection.Initialize();
    }

    [Fact]
    public void PlayerHasPosition()
    {
        var state = provider.GetRequiredService<StateService>();
        
        Assert.Equal(Vector2.Zero, state.PlayerPosition);
    }

    [Fact]
    public void CanMovePlayerDown()
    {
        var game = provider.GetRequiredService<GameService>();
        var state = provider.GetRequiredService<StateService>();

        state.ControllerDirection = Vector2.UnitX;

        game.Tick();

        Assert.GreaterThan(0, state.PlayerPosition.X);
    }

    [Fact]
    public void GraphicsDeviceManagerExistsInCollection()
    {
        provider.GetRequiredService<GraphicsDeviceManager>();
    }
}
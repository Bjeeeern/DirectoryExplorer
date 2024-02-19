namespace GameTests;

public class GameTests
{

    private readonly IServiceProvider provider;

    public GameTests()
    {
        provider = GameServiceCollection.Initialize();
    }

    [Fact]
    public void CanMovePlayerDown()
    {
        var game = provider.GetRequiredService<GameService>();
        var state = provider.GetRequiredService<StateService>();

        state.AddScene("test", new SceneModel());

        Assert.Null(state.Current);

        state.LoadScene("test");

        Assert.NotNull(state.Current);
        Assert.Equal(Vector2.Zero, state.Current.PlayerPosition);

        state.Current.ControllerDirection = Vector2.UnitX;
        game.Tick();

        Assert.GreaterThan(0, state.Current.PlayerPosition.X);

        state.LoadScene("test");

        Assert.Equal(Vector2.Zero, state.Current.PlayerPosition);
    }

    [Fact]
    public void GraphicsDeviceManagerExistsInCollection()
    {
        provider.GetRequiredService<GraphicsDeviceManager>();
    }
}
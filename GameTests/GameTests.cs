using Game.Extensions;

namespace GameTests;

public class GameTests
{
    private const string PlayerAtXUnitVector = "player at X unit vector";
    private const string PlayerAtYUnitVector = "player at Y unit vector";
    private readonly IServiceProvider provider;

    public GameTests()
    {
        provider = GameServiceCollection.Initialize();
    }

    [Fact]
    public void CanSetupScene()
    {
        var state = provider.GetRequiredService<StateService>();

        Assert.Equal(Vector2.Zero, state.Current.Player.Position);

        state.AddScene(PlayerAtXUnitVector, GetSceneWithPlayerAt(Vector2.UnitX));
        state.LoadScene(PlayerAtXUnitVector);

        Assert.Equal(Vector2.UnitX, state.Current.Player.Position);
    }

    [Fact]
    public void CanSwitchScenes()
    {
        var state = provider.GetRequiredService<StateService>();

        state.AddScene(PlayerAtXUnitVector, GetSceneWithPlayerAt(Vector2.UnitX));
        state.AddScene(PlayerAtYUnitVector, GetSceneWithPlayerAt(Vector2.UnitY));

        state.LoadScene(PlayerAtXUnitVector);

        Assert.Equal(Vector2.UnitX, state.Current.Player.Position);

        state.LoadScene(PlayerAtYUnitVector);

        Assert.Equal(Vector2.UnitY, state.Current.Player.Position);
    }

    [Fact]
    public void CanDeepCloneScene()
    {
        var scene = new SceneModel
        {
            ControllerDirection = Vector2.UnitX,
            Player = new()
            {
                Position = Vector2.UnitX,
            },
        };

        var clone = scene.DeepClone();

        Assert.Equivalent(scene, clone, strict: true);

        Assert.NotSame(scene.Player, clone.Player);
    }

    [Fact]
    public void CanReloadScene()
    {
        var state = provider.GetRequiredService<StateService>();

        state.AddScene(PlayerAtXUnitVector, GetSceneWithPlayerAt(Vector2.UnitX));
        state.LoadScene(PlayerAtXUnitVector);

        state.Current.Player.Position = Vector2.Zero;

        state.LoadScene(PlayerAtXUnitVector);

        Assert.Equal(Vector2.UnitX, state.Current.Player.Position);
    }

    [Fact]
    public void CanMovePlayerDown()
    {
        var game = provider.GetRequiredService<GameService>();
        var state = provider.GetRequiredService<StateService>();

        state.Current.ControllerDirection = Vector2.UnitX;
        game.Tick();

        Assert.GreaterThan(0, state.Current.Player.Position.X);
    }

    [Fact]
    public void GraphicsDeviceManagerInstantiatedWhenGameInstantiated()
    {
        var game = provider.GetRequiredService<GameService>();

        // Microsoft.Xna.Framework.GraphicsDeviceManager (GDM) has a dependency on Microsoft.Xna.Framework.Game (XnaGame)
        // but XnaGame also has a implicit runtime dependency on GDM, so GDM needs to be instantiated whenever XnaGame is
        // instantiated if it hasn't already been.
        game.Tick();
    }

    private static SceneModel GetSceneWithPlayerAt(Vector2 vector) =>
        new() { Player = new() { Position = vector } };
}
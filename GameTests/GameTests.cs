using Microsoft.Xna.Framework.Input;
using System.Numerics;

namespace GameTests;

public class GameTests
{
    [Fact]
    public void CanInitServiceCollection()
    {
        GameServiceCollection.Initialize();
    }

    [Fact]
    public void CanRetrieveGameService()
    {
        var provider = GameServiceCollection.Initialize();

        provider.GetRequiredService<GameService>();
    }

    [Fact]
    public void RunOneFrame()
    {
        var game = GameServiceCollection
            .Initialize()
            .GetRequiredService<GameService>();

        game.Tick();
    }

    [Fact] public void PlayerHasPosition()
    {
        var provider = GameServiceCollection.Initialize();

        var state = provider.GetRequiredService<StateService>();
        
        Assert.Equal(Vector2.Zero, state.PlayerPosition);
    }

    [Fact]
    public void CanMovePlayerDown()
    {
        var provider = GameServiceCollection.Initialize();

        var game = provider.GetRequiredService<GameService>();
        var state = provider.GetRequiredService<StateService>();

        state.ControllerDirection = Vector2.UnitX;

        game.Tick();

        Assert.True(state.PlayerPosition.X > 0);
    }
}
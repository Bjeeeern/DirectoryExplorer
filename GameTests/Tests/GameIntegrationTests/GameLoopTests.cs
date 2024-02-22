namespace GameTests.Tests.GameIntegration;

public partial class GameIntegrationTests
{
    [Fact]
    public void CanMovePlayerDown()
    {
        var game = provider.GetRequiredService<GameService>();
        var state = provider.GetRequiredService<StateService>();

        state.Current.ControllerDirection = Vector2.UnitX;
        game.Tick();

        Assert.GreaterThan(0, state.Current.Player.Position.X);
    }
}
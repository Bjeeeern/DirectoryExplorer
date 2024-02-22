namespace GameTests.Tests.GameIntegration;

public partial class GameIntegrationTests
{
    private readonly IServiceProvider provider;

    public GameIntegrationTests()
    {
        provider = GameServiceCollection.Initialize();
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
}
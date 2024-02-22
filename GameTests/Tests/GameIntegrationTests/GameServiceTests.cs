namespace GameTests.Tests.GameIntegration;

public partial class GameIntegrationTests
{
    private readonly IServiceProvider provider;
    private readonly GameService game;

    public GameIntegrationTests()
    {
        provider = GameServiceCollection.Initialize();
        game = provider.GetRequiredService<GameService>();
    }

    [Fact]
    public void GraphicsDeviceManagerInstantiatedWhenGameInstantiated()
    {
        // Microsoft.Xna.Framework.GraphicsDeviceManager (GDM) has a dependency on Microsoft.Xna.Framework.Game (XnaGame)
        // but XnaGame also has a implicit runtime dependency on GDM, so GDM needs to be instantiated whenever XnaGame is
        // instantiated if it hasn't already been.
        game.Tick();
    }

    [Fact]
    public async Task CanRunGame()
    {
        var task = game.RunAsync();

        task.Start();
        Thread.Sleep(1);

        game.Stop();
        await task;

        Assert.True(task.IsCompletedSuccessfully);
    }
}
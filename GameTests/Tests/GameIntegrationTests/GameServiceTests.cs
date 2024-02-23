using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

namespace GameTests.Tests.GameIntegration;

public partial class GameIntegrationTests : IDisposable
{
    private readonly IServiceProvider provider;
    private readonly GameService game;
    private readonly StateService state;

    public GameIntegrationTests()
    {
        provider = GameServiceCollection.Initialize();
        game = provider.GetRequiredService<GameService>();
        state = provider.GetRequiredService<StateService>();

        state.Current.Player.SpriteAsset = "Villager";

        typeof(GraphicsDevice).Assembly
            .GetType("Microsoft.Xna.Framework.Threading")!
            .GetField("_mainThreadId", BindingFlags.Static | BindingFlags.NonPublic)!
            .SetValue(null, Thread.CurrentThread.ManagedThreadId);
    }

    public void Dispose()
    {
        var wrapper = provider.GetRequiredService<XnaGameWrapperService>();
        wrapper.Dispose();
    }

    [Fact]
    public void GraphicsDeviceManagerInstantiatedWhenGameInstantiated()
    {
        // Microsoft.Xna.Framework.GraphicsDeviceManager (GDM) has a dependency on Microsoft.Xna.Framework.Game (XnaGame)
        // but XnaGame also has a implicit runtime dependency on GDM, so GDM needs to be instantiated whenever XnaGame is
        // instantiated if it hasn't already been.
        game.RunOneFrame();
    }

    [Fact]
    public void AllHandlersInitialize()
    {
        var wrapper = provider.GetRequiredService<XnaGameWrapperService>();
        var handlers = typeof(XnaGameWrapperService)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.Name.EndsWith("Handler"));

        foreach (var handler in handlers)
        {
            Assert.NotNull(handler.GetValue(wrapper));
        }
    }
}
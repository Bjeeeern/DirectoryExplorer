using Game.Services;

namespace Game;

public static class GameServiceCollection
{
    public static IServiceProvider Initialize()
    {
        var services = new ServiceCollection()
            .AddSingleton<XnaGame, GameService>()
            .AddSingleton<StateService>();

        return services.BuildServiceProvider();
    }
}
namespace Game;

public static class GameServiceCollection
{
    public static IServiceProvider Initialize()
    {
        var services = new ServiceCollection();

        services.AddSingleton<GameService>();

        return services.BuildServiceProvider();
    }
}
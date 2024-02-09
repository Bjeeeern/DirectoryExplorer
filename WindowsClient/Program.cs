ApplicationConfiguration.Initialize();
GameServiceCollection.Initialize()
    .GetRequiredService<GameService>()
    .Run();

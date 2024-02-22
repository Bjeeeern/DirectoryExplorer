ApplicationConfiguration.Initialize();
await GameServiceCollection.Initialize()
    .GetRequiredService<GameService>()
    .RunAsync();

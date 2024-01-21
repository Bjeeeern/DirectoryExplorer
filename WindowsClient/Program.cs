ApplicationConfiguration.Initialize();
GameServiceCollection.Initialize()
    .GetRequiredService<XnaGame>()
    .Run();

namespace Game.Services;

internal class GameService : XnaGame
{
    private readonly StateService state;

    public GameService(StateService state)
    {
        new GraphicsDeviceManager(this);
        this.state = state;
    }

    protected override void Update(GameTime gameTime)
    {
        this.state.PlayerPosition = this.state.ControllerDirection;
    }
}
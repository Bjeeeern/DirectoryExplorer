namespace Game.Services;

public class GameService
{
    private readonly StateService state;
    private readonly XnaGameWrapperService xnaGameWrapper;

    public GameService(StateService state, XnaGameWrapperService xnaGameWrapper)
    {
        this.state = state;
        this.xnaGameWrapper = xnaGameWrapper;

        xnaGameWrapper.UpdateHandler = Update;
    }

    public Task RunAsync() =>
        new(xnaGameWrapper.Run);

    internal void Stop() =>
        xnaGameWrapper.Exit();

    internal void Tick() =>
        xnaGameWrapper.Tick();

    private void Update()
    {
        state.Current.Player.Position = state.Current.ControllerDirection;
    }
}
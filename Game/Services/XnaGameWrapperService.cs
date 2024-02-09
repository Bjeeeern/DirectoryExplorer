namespace Game.Services;

public class XnaGameWrapperService : XnaGame
{
    public Action? UpdateHandler { get; internal set; }

    protected override void Update(GameTime gameTime) =>
        UpdateHandler!.Invoke();
}
using Microsoft.Xna.Framework.Graphics;

namespace GameTests.Tests.GameIntegration;

public partial class GameIntegrationTests
{
    [Fact]
    public void CanMovePlayerDown()
    {
        state.Current.ControllerDirection = Vector2.UnitX;
        game.RunOneFrame();

        Assert.GreaterThan(0, state.Current.Player.Position.X);
    }

    [Fact]
    public void CanRenderPlayer()
    {
        var gdm = provider.GetRequiredService<GraphicsDeviceManager>();
        var wrapper = provider.GetRequiredService<XnaGameWrapperService>();

        gdm.ApplyChanges();

        var image = new Color[32 * 32];
        var render = new Color[32 * 32];

        Texture2D.FromStream(gdm.GraphicsDevice, File.OpenRead(@"..\..\..\..\RawAssets\Villager.png"))
            .GetData(image);

        wrapper.DrawHandler += () =>
        {
            var backBufferWidth = gdm.GraphicsDevice.PresentationParameters.BackBufferWidth;
            var backBufferHeight = gdm.GraphicsDevice.PresentationParameters.BackBufferHeight;
            var expectedArea = new Rectangle(
                backBufferWidth / 2 - 32 / 2,
                backBufferHeight / 2 - 32 / 2,
                32,
                32);

            gdm.GraphicsDevice.GetBackBufferData(expectedArea, render, 0, 32 * 32);
        };

        game.RunOneFrame();

        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 32; x++)
            {
                var expected = image[y * 32 + x];
                var actual = render[y * 32 + x];

                actual.A = expected.A;
                Assert.Equal(expected, actual);
            }
        }
    }
}
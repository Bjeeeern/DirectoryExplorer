using Microsoft.Xna.Framework;

namespace DirectoryExplorer.Utility.Extensions
{
    internal static class GraphicsDeviceManagerExtensions
    {
        public static Vector3 GetScreenScale(this GraphicsDeviceManager graphicsDeviceManager) =>
            new Vector3(graphicsDeviceManager.PreferredBackBufferWidth, graphicsDeviceManager.PreferredBackBufferHeight, 0.0f);

        public static void SetScreenScale(this GraphicsDeviceManager graphicsDeviceManager, int width, int height)
        {
            graphicsDeviceManager.PreferredBackBufferWidth = width;
            graphicsDeviceManager.PreferredBackBufferHeight = height;
        }
    }
}

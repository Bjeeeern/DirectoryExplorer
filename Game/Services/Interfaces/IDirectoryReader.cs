using Game.Services.Models;

namespace Game.Services.Interfaces
{
    internal interface IDirectoryReader
    {
        Directory ReadDirectory(string path);
    }
}
using Game.Services.Models;

namespace DirectoryExplorer.Services.Interfaces
{
    internal interface IDirectoryReader
    {
        Directory ReadDirectory(string path);
    }
}
﻿using System.IO;
using Game.Services.Interfaces;
using GameDirectory = Game.Services.Models.Directory;
using Directory = System.IO.Directory;

namespace Game.Services.Providers
{
    class DirectoryReader : IDirectoryReader
    {
        public GameDirectory ReadDirectory(string path)
        {
            var current = new DirectoryInfo(path);
            return new GameDirectory
            {
                Parent = Directory.GetParent(path),
                Current = current,
                Children = current.GetDirectories(),
                Files = current.GetFiles(),
                Drive = new DriveInfo(current.Root.Name),
            };
        }
    }
}
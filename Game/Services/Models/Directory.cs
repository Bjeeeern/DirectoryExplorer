using System.Collections.Generic;
using System.IO;

namespace Game.Services.Models
{
    public class Directory
    {
        public DirectoryInfo? Parent { get; init; }
        public DirectoryInfo Current { get; init; }
        public IReadOnlyList<DirectoryInfo> Children { get; init; }
        public IReadOnlyList<FileInfo> Files { get; init; }
        public DriveInfo Drive { get; init; }
    }
}
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryExplorer.Primitives
{
    internal interface ICamera: IEntity, IMovable
    {
        Matrix Transform { get; set; }
    }
}

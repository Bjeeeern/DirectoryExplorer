using DirectoryExplorer.Utility;
using System;

namespace DirectoryExplorer.Primitives
{
    internal interface ITrigger : IEntity
    {
        RectangleF Area { get; set; }

        bool Safety { get; set; }

        Action Action { get; set; }
        bool TriggerOnce { get; set; }
    }
}

using DirectoryExplorer.Primitives;
using DirectoryExplorer.Utility;
using System;

namespace DirectoryExplorer.Entities
{
    internal class Trigger : ITrigger
    {
        public RectangleF Area { get; set; }
        public bool Safety { get; set; }
        public bool TriggerOnce { get; set; }
        public Action Action { get; set; }
    }
}

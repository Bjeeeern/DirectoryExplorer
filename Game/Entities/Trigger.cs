using Game.Primitives;
using Game.Utility;
using System;

namespace Game.Entities
{
    internal class Trigger : ITrigger
    {
        public RectangleF Area { get; set; }
        public bool Safety { get; set; }
        public bool TriggerOnce { get; set; }
        public Action Action { get; set; }
    }
}

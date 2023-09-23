﻿namespace DirectoryExplorer.Primitives
{
    interface IBody : IPositioned, IMovable
    {
        float Radius { get; set; }
    }
}
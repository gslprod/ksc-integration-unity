using System;
using System.Collections.Generic;
using UnityEngine;

public interface IConnectable
{
    event Action OnPositionChanged;

    Vector3 Position { get; }
    List<ConnectionPath> ConnectionPaths { get; }
    string Name { get; }
    string Type { get; }

    void AddConnect(ConnectionPath connectionPath);
    void RemoveConnect(ConnectionPath connectionPath);
}

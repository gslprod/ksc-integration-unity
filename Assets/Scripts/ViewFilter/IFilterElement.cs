using System;
using UnityEngine;

public interface IFilterElement
{
    public enum VisibilityMode
    {
        ObeysToFilter = 0,
        AlwaysInvisible,
        AlwaysVisible
    }

    event Action OnVisibilityChanged;
    event Action<IFilterElement> OnFloorChanged;
    event Action<IFilterElement> OnModeChanged;

    bool Visible { get; }
    int Floor { get; }
    VisibilityMode Mode { get; }
    Vector3 Position { get; }

    void SetFloor(int newFloor);
    void SetVisibilityMode(VisibilityMode newMode);
}

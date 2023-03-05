using System;

public static class ViewFilter
{
    public enum ViewMode
    {
        All = 0,
        OnlyFirstFloor,
        OnlySecondFloor,
        OnlyThirdFloor,
        OnlyFourthFloor
    }

    public static event Action OnModeChanged;

    public static ViewMode Mode { get; private set; }

    public static void SetMode(ViewMode mode)
    {
        if (Mode == mode)
            return;

        Mode = mode;
        OnModeChanged?.Invoke();
    }
}

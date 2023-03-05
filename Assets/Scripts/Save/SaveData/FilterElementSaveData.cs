using System;

[Serializable]
public class FilterElementSaveData : ISaveData
{
    public IFilterElement.VisibilityMode Visibility;
    public int Floor;
}

using System;
using UnityEngine;

[Serializable]
public class DeviceSaveData : ISaveData
{
    public string Type;
    public string Name;
    public Vector3 Position;
}

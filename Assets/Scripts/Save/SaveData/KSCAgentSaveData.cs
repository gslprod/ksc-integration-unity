using System;

[Serializable]
public class KSCAgentSaveData : ISaveData
{
    public string IP, HostName;
    public bool UseKSCName;
}


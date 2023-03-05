using KSC;
using KSC.Clients;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class DevicesEditor : MonoBehaviour
{
    public enum Direction
    {
        Left = 1,
        Right,
        Up,
        Down,
        Forward,
        Backward
    }

    public event Action<IDevice> OnDeviceDestroyed;
    public event Action<IDevice> OnDeviceCreated;
    public event Action<ConnectionPath> OnPathDestroyed;
    public event Action<ConnectionPath> OnPathCreated;
    public event Action<bool, string> OnSaveResult;
    public event Action<bool, string> OnLoadResult;

    public IDevice[] Devices => _devices.ToArray();

    [SerializeField] private Canvas _worldSpaceCanvas;
    [SerializeField] private Sprite[] _devicesSprites;
    [SerializeField] private DevicePrefab _devicePrefab;
    [SerializeField] private FloorCalculator _floorCalculator;

    private List<IDevice> _devices = new List<IDevice>();
    private ISaver _saver = new NETJSONSaver();

    public IDevice CreateDevice(Type deviceType, Vector3 position)
    {
        var createdDevicePrefab = Instantiate(_devicePrefab, position, Quaternion.identity, _worldSpaceCanvas.transform);
        var createdGameObject = createdDevicePrefab.gameObject;

        var device = (IDevice)createdGameObject.AddComponent(deviceType);
        device.SetTextMesh(createdDevicePrefab.Text);
        device.SetImage(createdDevicePrefab.Icon);
        SetIconByType(deviceType, createdDevicePrefab.Icon);

        RegisterDevice(device);
        OnDeviceCreated?.Invoke(device);

        return device;
    }

    public void ChangeDeviceName(IDevice target, string newName)
    {
        if (target == null)
            return;

        target.ChangeName(newName);
    }

    public void DestroyDevice(IDevice device)
    {
        if (device is IConnectable connectable)
            for (int i = connectable.ConnectionPaths.Count - 1; i >= 0; i--)
                DestroyPath(connectable.ConnectionPaths[i]);

        Destroy(device.GameObject);
        UnregisterDevice(device);
        OnDeviceDestroyed?.Invoke(device);
    }

    public void DestroyAllDevices()
    {
        for (int i = _devices.Count - 1; i >= 0; i--)
            DestroyDevice(_devices[i]);
    }

    public void DestroyPath(ConnectionPath path)
    {
        path.First.RemoveConnect(path);
        path.Second.RemoveConnect(path);
        Destroy(path.gameObject);
        OnPathDestroyed?.Invoke(path);
    }

    public void ChangeDevicePosition(IDevice device, Vector3 newPosition)
    {
        device.ChangePosition(newPosition);
    }

    public void ChangeDevicePositionByRotation(IDevice device, Direction dir, float offset)
    {
        var offsetVector = dir switch
        {
            Direction.Left => -device.GameObject.transform.right.ClearDimension(Vector3Dimensions.Y).normalized * offset,
            Direction.Right => device.GameObject.transform.right.ClearDimension(Vector3Dimensions.Y).normalized * offset,
            Direction.Up => Vector3.up * offset,
            Direction.Down => Vector3.down * offset,
            Direction.Forward => device.GameObject.transform.forward.ClearDimension(Vector3Dimensions.Y).normalized * offset,
            Direction.Backward => -device.GameObject.transform.forward.ClearDimension(Vector3Dimensions.Y).normalized * offset,
            _ => Vector3.zero,
        };
        device.ChangePosition(device.Position + offsetVector);
    }

    public void ConnectTwoConnectableItems(IConnectable first, IConnectable second)
    {
        if (first == null || second == null)
            return;

        if (first == second)
            return;

        foreach (var existPath in first.ConnectionPaths)
        {
            if (second.ConnectionPaths.Contains(existPath))
                return;
        }

        var createdObj = new GameObject("Path");
        var path = createdObj.AddComponent<ConnectionPath>();
        createdObj.AddComponent<LineRenderer>();
        path.SetConnection(first, second);
        first.AddConnect(path);
        second.AddConnect(path);
        OnPathCreated?.Invoke(path);
    }

    public void AutoCalculateFloor(IFilterElement filterElement)
    {
        filterElement.SetFloor(_floorCalculator.CalculateFloor(filterElement.Position));
    }

    public void AutoCalculateFloorForAllElements()
    {
        var filterElements = GetDevicesOfType<IFilterElement>();

        foreach (var filterElement in filterElements)
            AutoCalculateFloor(filterElement);
    }

    public T[] GetMonoBehavioursOfType<T>(bool includeInactive)
    {
        var allMonoBehaviours = FindObjectsOfType<MonoBehaviour>(includeInactive);

        List<T> tElements = new List<T>();
        foreach (var mb in allMonoBehaviours)
            if (mb is T tElement)
                tElements.Add(tElement);

        return tElements.ToArray();
    }

    public T[] GetDevicesOfType<T>()
    {
        List<T> tElements = new List<T>();
        foreach (var mb in _devices)
            if (mb is T tElement)
                tElements.Add(tElement);

        return tElements.ToArray();
    }

    public void SaveAllDevices(string savePath, bool includeInactive)
    {
        try
        {
            if (_devices.Count == 0)
                throw new InvalidOperationException("Отсутствуют объекты для сохранения");

            List<SaveContainer> saveContainers = new List<SaveContainer>();
            foreach (var device in _devices)
            {
                SaveContainer saveContainer = new SaveContainer();

                DeviceSaveData deviceSaveData = new DeviceSaveData
                {
                    Name = device.Name,
                    Type = device.GetType().Name,
                    Position = device.Position
                };

                saveContainer.DeviceSaveData = deviceSaveData;

                if (device is IConnectable connectable)
                {
                    var connectableSaveData = new ConnectableSaveData
                    {
                        ConnectionIndeces = new int[connectable.ConnectionPaths.Count]
                    };

                    for (int i = 0; i < connectable.ConnectionPaths.Count; i++)
                    {
                        ConnectionPath path = connectable.ConnectionPaths[i];

                        IConnectable other = path.First == connectable ? path.Second : path.First;
                        connectableSaveData.ConnectionIndeces[i] = _devices.IndexOf((IDevice)other);
                    }

                    saveContainer.ConnectableSaveData = connectableSaveData;
                }

                if (device is IFilterElement filterElement)
                {
                    var filterElementSaveData = new FilterElementSaveData
                    {
                        Visibility = filterElement.Mode,
                        Floor = filterElement.Floor
                    };

                    saveContainer.FilterElementSaveData = filterElementSaveData;
                }

                if (device is IKSCAgent kscAgent)
                {
                    var kscAgentSaveData = new KSCAgentSaveData
                    {
                        IP = kscAgent.IPAddress,
                        HostName = kscAgent.HostName,
                        UseKSCName = kscAgent.UseKSCAgentName
                    };

                    saveContainer.KSCAgentSaveData = kscAgentSaveData;
                }

                saveContainers.Add(saveContainer);
            }

            _saver.Save(saveContainers.ToArray(), savePath);

            OnSaveResult?.Invoke(true, "Успешное сохранение");
        }
        catch (Exception ex)
        {
            OnSaveResult?.Invoke(false, $"Сбой сохранения. Техническая информация: {ex.GetType().Name} - {ex.Message}");
            throw;
        }
    }

    public void LoadDevices(string path)
    {
        try
        {
            var saveContainers = _saver.Load<SaveContainer[]>(path);

            List<IDevice> createdDevices = new List<IDevice>();
            for (int numberOfSaveContainer = 0; numberOfSaveContainer < saveContainers.Length; numberOfSaveContainer++)
            {
                var saveContainer = saveContainers[numberOfSaveContainer];
                var deviceSaveData = saveContainer.DeviceSaveData;

                var createdDevice = CreateDevice(Type.GetType(deviceSaveData.Type, true, false), deviceSaveData.Position);
                ChangeDeviceName(createdDevice, deviceSaveData.Name);
                createdDevices.Add(createdDevice);

                if (saveContainer.ConnectableSaveData != null)
                {
                    var connectableSaveData = saveContainer.ConnectableSaveData;
                    var connectable = (IConnectable)createdDevice;

                    for (int connectionNumber = 0; connectionNumber < connectableSaveData.ConnectionIndeces.Length; connectionNumber++)
                    {
                        var connectionIndex = connectableSaveData.ConnectionIndeces[connectionNumber];

                        if (connectionIndex >= numberOfSaveContainer)
                            continue;

                        var other = (IConnectable)createdDevices[connectionIndex];
                        ConnectTwoConnectableItems(connectable, other);
                    }
                }

                if (saveContainer.FilterElementSaveData != null)
                {
                    var filterElementSaveData = saveContainer.FilterElementSaveData;
                    var filterElement = (IFilterElement)createdDevice;

                    filterElement.SetFloor(filterElementSaveData.Floor);
                    filterElement.SetVisibilityMode(filterElementSaveData.Visibility);
                }

                if (saveContainer.KSCAgentSaveData != null)
                {
                    var kscAgentSaveData = saveContainer.KSCAgentSaveData;
                    var kscAgent = (IKSCAgent)createdDevice;

                    kscAgent.SetIP(kscAgentSaveData.IP);
                    kscAgent.SetHostName(kscAgentSaveData.HostName);
                    kscAgent.SetKSCNameUsing(kscAgentSaveData.UseKSCName);
                }
            }

            OnLoadResult?.Invoke(true, "Успешная загрузка сохранения");
        }
        catch (Exception ex)
        {
            OnLoadResult?.Invoke(false, $"Сбой загрузки сохранения. Возможно, некоторые объекты успели загрузиться. Техническая информация: {ex.GetType().Name} - {ex.Message}");
        }
    }

    public void LoadDevicesObsolete(string path)
    {
        try
        {
            List<string> lines = new List<string>();
            using (StreamReader sr = File.OpenText(path))
            {
                string line = null;
                line = sr.ReadLine();
                while (line != null)
                {
                    lines.Add(line);
                    line = sr.ReadLine();
                }
            }

            List<IDevice> createdDevices = new List<IDevice>();
            Dictionary<int, List<int>> connections = new Dictionary<int, List<int>>();

            foreach (var line in lines)
            {
                var saveParams = line.Split('|');
                if (saveParams.Length < 2)
                    throw new Exception($"Обнаружено повреждение сохранения, загрузка сохранения остановлена (поврежденная строка: {line})");

                if (saveParams[1].ToLower() == "device")
                {
                    if (saveParams.Length < 6)
                        throw new Exception($"Обнаружено повреждение сохранения, загрузка сохранения остановлена (поврежденная строка: {line})");

                    var posInfo = saveParams[3].Split(';');
                    if (posInfo.Length < 3)
                        throw new Exception($"Обнаружено повреждение сохранения, загрузка сохранения остановлена (поврежденная строка: {line})");

                    var device = CreateDevice(
                        Type.GetType(saveParams[2].ToLower(), true, true),
                        new Vector3(float.Parse(posInfo[0]), float.Parse(posInfo[1]), float.Parse(posInfo[2])));
                    device.ChangeName(saveParams[4]);
                    createdDevices.Add(device);

                    if (!string.IsNullOrEmpty(saveParams[5]))
                    {
                        var connectionsInfo = saveParams[5].Split(';');
                        var currentIndex = int.Parse(saveParams[0]);

                        foreach (var connectionInfo in connectionsInfo)
                        {
                            var otherIndex = int.Parse(connectionInfo);
                            if (connections.ContainsKey(otherIndex))
                                break;

                            if (connections.ContainsKey(currentIndex))
                                connections[currentIndex].Add(otherIndex);
                            else
                                connections.Add(currentIndex, new List<int>() { otherIndex });
                        }
                    }
                }
            }

            foreach (var connection in connections)
            {
                var first = (IConnectable)createdDevices[connection.Key];
                foreach (var secondIndex in connection.Value)
                {
                    var second = (IConnectable)createdDevices[secondIndex];
                    ConnectTwoConnectableItems(first, second);
                }
            }

            OnLoadResult?.Invoke(true, "Успешная загрузка сохранения");
        }
        catch (Exception ex)
        {
            OnLoadResult?.Invoke(false, "Сбой загрузки сохранения. Возможно, некоторые объекты успели загрузиться. Техническая информация: " + ex.GetType().Name + " - " + ex.Message);
        }
    }

    public void AutoAssignAgents()
    {
        if (Server.RegisteredServers.Length == 0)
            return;

        var kscAgents = GetDevicesOfType<IKSCAgent>();
        var usedAgents = new List<Agent>();
        var notAssignedKSCObjects = new List<IKSCAgent>();

        foreach (var kscAgent in kscAgents)
        {
            if (kscAgent.Agent == null)
            {
                notAssignedKSCObjects.Add(kscAgent);
                continue;
            }

            usedAgents.Add(kscAgent.Agent);
        }

        List<Agent> allAgents = new List<Agent>();

        var registeredServers = Server.RegisteredServers;
        foreach (var server in registeredServers)
            allAgents.AddRange(server.Agents);

        List<Agent> notUsedAgents = new List<Agent>(allAgents.Except(usedAgents));

        foreach (var kscAgent in notAssignedKSCObjects)
        {
            if (string.IsNullOrEmpty(kscAgent.HostName) && string.IsNullOrEmpty(kscAgent.IPAddress))
                continue;

            var agent = notUsedAgents.Find((agent) => agent.HostName == kscAgent.HostName || agent.IP == kscAgent.IPAddress);
            if (agent == null)
                continue;

            kscAgent.SetKSCAgent(agent);
            notUsedAgents.Remove(agent);
        }
    }

    private void SetIconByType(Type deviceType, Image target)
    {
        if (deviceType == typeof(PC))
            target.sprite = _devicesSprites[0];
        else if (deviceType == typeof(IPTelephone))
            target.sprite = _devicesSprites[1];
        else if (deviceType == typeof(ConnectionLink))
            target.sprite = _devicesSprites[2];
        else if (deviceType == typeof(Videcam))
            target.sprite = _devicesSprites[3];
    }

    private void RegisterDevice(IDevice device)
    {
        _devices.Add(device);
    }

    private void UnregisterDevice(IDevice device)
    {
        _devices.Remove(device);
    }
}


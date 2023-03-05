using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using KSC.Clients;
using KSC;
using UnityEngine.UI;

public class PC : MonoBehaviour, IDevice, IConnectable, IFilterElement, IKSCAgent
{
    public event Action OnPositionChanged;
    public event Action OnVisibilityChanged;
    public event Action<IFilterElement> OnFloorChanged;
    public event Action<IFilterElement> OnModeChanged;
    public event Action<IDevice> OnNameChanged;
    public event Action<Agent> OnAgentSet;
    public event Action<Agent> OnAgentRemoved;
    public event Action<IKSCAgent> OnHostNameChanged;
    public event Action<IKSCAgent> OnIPAddressChanged;
    public event Action<IKSCAgent> OnKSCNameUsingChanged;

    public static string StaticType => "Компьютер";
    public string Type => StaticType;

    public Vector3 Position => transform.position;
    public GameObject GameObject => gameObject;
    public List<ConnectionPath> ConnectionPaths { get; private set; } = new List<ConnectionPath>();
    public string Name { get; private set; }
    public bool Visible { get; private set; }
    public int Floor { get; private set; }
    public IFilterElement.VisibilityMode Mode { get; private set; }
    public Agent Agent { get; private set; }
    public string HostName { get; private set; }
    public string IPAddress { get; private set; }
    public bool UseKSCAgentName { get; private set; } = true;

    private TextMeshProUGUI _textMesh;
    private Image _image;

    private void Start()
    {
        UpdateVisibility();
        SubscribeToFilter();
    }

    private void OnDestroy()
    {
        UnsubscribeFromFilter();

        if (Agent != null)
            UnsubscribeFromKSCAgent();
    }

    public void AddConnect(ConnectionPath connectionPath)
    {
        if (ConnectionPaths.Contains(connectionPath))
            return;

        ConnectionPaths.Add(connectionPath);
    }

    public void RemoveConnect(ConnectionPath connectionPath)
    {
        if (!ConnectionPaths.Contains(connectionPath))
            return;

        ConnectionPaths.Remove(connectionPath);
    }

    public void ChangePosition(Vector3 newPosition)
    {
        transform.position = newPosition;
        OnPositionChanged?.Invoke();
    }

    public void ChangeName(string newName)
    {
        ChangeNameInternal(newName);
    }

    public void SetTextMesh(TextMeshProUGUI textMesh)
    {
        _textMesh = textMesh;
        _textMesh.text = Name;
    }

    public void SetImage(Image image)
    {
        _image = image;
    }

    public void SetFloor(int newFloor)
    {
        if (Floor == newFloor)
            return;

        Floor = newFloor;
        OnFloorChanged?.Invoke(this);

        UpdateVisibility();
    }

    public void SetVisibilityMode(IFilterElement.VisibilityMode newMode)
    {
        if (Mode == newMode)
            return;

        Mode = newMode;
        OnModeChanged?.Invoke(this);

        UpdateVisibility();
    }

    public void SetKSCAgent(Agent agent)
    {
        if (Agent != null)
            UnsubscribeFromKSCAgent();

        Agent = agent;

        SubscribeToKSCAgent();
        UpdateKSCInfo();

        OnAgentSet?.Invoke(agent);
    }

    public void RemoveKSCAgent()
    {
        if (Agent != null)
            UnsubscribeFromKSCAgent();

        var removed = Agent;

        Agent = null;

        UpdateIcon();

        OnAgentRemoved?.Invoke(removed);
    }

    public void SetHostName(string hostName)
    {
        if (Agent != null)
            throw new InvalidOperationException();

        HostName = hostName;
        OnHostNameChanged?.Invoke(this);
    }

    public void SetIP(string ip)
    {
        if (Agent != null)
            throw new InvalidOperationException();

        IPAddress = ip;
        OnIPAddressChanged?.Invoke(this);
    }

    public void SetKSCNameUsing(bool use)
    {
        UseKSCAgentName = use;

        if (use && Agent != null)
            UpdateNameByKSCAgent();

        OnKSCNameUsingChanged?.Invoke(this);
    }

    private void ChangeNameInternal(string newName)
    {
        Name = newName;
        if (_textMesh != null)
            _textMesh.text = Name;

        OnNameChanged?.Invoke(this);
    }

    private void UpdateNameByKSCAgent()
    {
        ChangeNameInternal($"{Agent.IP}, {Agent.Name} ({Agent.Group})");
    }

    private void UpdateVisibility()
    {
        bool active;

        if (ViewFilter.Mode == ViewFilter.ViewMode.All || Mode == IFilterElement.VisibilityMode.AlwaysVisible)
            active = true;
        else if (Mode == IFilterElement.VisibilityMode.AlwaysInvisible)
            active = false;
        else
            active = Floor == (int)ViewFilter.Mode;

        gameObject.SetActive(active);
        Visible = active;

        OnVisibilityChanged?.Invoke();
    }

    private void UpdateKSCInfo()
    {
        HostName = Agent.HostName;
        IPAddress = Agent.IP;
        OnIPAddressChanged?.Invoke(this);
        OnHostNameChanged?.Invoke(this);

        UpdateIcon();
        if (UseKSCAgentName)
            UpdateNameByKSCAgent();
    }

    private void UpdateIcon()
    {
        if (Agent == null)
        {
            _image.color = Color.black;
            return;
        }

        _image.color = Agent.Status switch
        {
            AgentStatus.OK => new Color(0.2f, 0.75f, 0.15f),
            AgentStatus.Critical => new Color(0.75f, 0.15f, 0.15f),
            AgentStatus.Warning => new Color(0.8f, 0.65f, 0.15f),
            _ => Color.black,
        };
    }

    private void SubscribeToFilter()
    {
        ViewFilter.OnModeChanged += UpdateVisibility;
    }

    private void SubscribeToKSCAgent()
    {
        Agent.OnInfoUpdated += AgentInfoUpdated;
        Agent.OnDispose += AgentDispose;
    }

    private void UnsubscribeFromFilter()
    {
        ViewFilter.OnModeChanged -= UpdateVisibility;
    }

    private void UnsubscribeFromKSCAgent()
    {
        Agent.OnInfoUpdated -= AgentInfoUpdated;
        Agent.OnDispose -= AgentDispose;
    }

    private void AgentInfoUpdated(Agent agent)
    {
        UpdateKSCInfo();
    }

    private void AgentDispose(Agent agent)
    {
        RemoveKSCAgent();
    }
}

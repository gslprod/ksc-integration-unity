using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Videcam : MonoBehaviour, IDevice, IConnectable, IFilterElement
{
    public event Action OnPositionChanged;
    public event Action OnVisibilityChanged;
    public event Action<IFilterElement> OnFloorChanged;
    public event Action<IFilterElement> OnModeChanged;
    public event Action<IDevice> OnNameChanged;

    public static string StaticType => "Камера видеонаблюдения";
    public string Type => StaticType;

    public Vector3 Position => transform.position;
    public GameObject GameObject => gameObject;
    public string Name { get; private set; }
    public List<ConnectionPath> ConnectionPaths { get; private set; } = new List<ConnectionPath>();
    public bool Visible { get; private set; }
    public int Floor { get; private set; }
    public IFilterElement.VisibilityMode Mode { get; private set; }

    private TextMeshProUGUI _textMesh;
    private Image _image;

    private void Start()
    {
        SubscribeToFilter();
    }

    private void OnDestroy()
    {
        UnsubscribeFromFilter();
    }

    public void ChangePosition(Vector3 newPosition)
    {
        transform.position = newPosition;
        OnPositionChanged?.Invoke();
    }

    public void ChangeName(string newName)
    {
        Name = newName;
        if (_textMesh != null)
            _textMesh.text = Name;

        OnNameChanged?.Invoke(this);
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

    private void SubscribeToFilter()
    {
        ViewFilter.OnModeChanged += UpdateVisibility;
    }

    private void UnsubscribeFromFilter()
    {
        ViewFilter.OnModeChanged -= UpdateVisibility;
    }
}

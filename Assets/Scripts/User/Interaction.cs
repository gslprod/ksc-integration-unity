using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    public event Action<IDevice> OnSelected;
    public event Action<IDevice> OnDeselected;
    public event Action<IInteractionState> OnStateChanged;

    public DevicesEditor Editor => _editor;
    public IDevice SelectedDevice => _selectedDevice;
    public SimpleCameraController CameraController { get; private set; }
    public IInteractionState State => _state;

    [SerializeField] private DevicesEditor _editor;
    [SerializeField] private Canvas _worldSpaceCanvas;
    [SerializeField] private Canvas _cameraCanvas;
    [SerializeField] private EventSystem _eventSystem;

    private IInteractionState _state { get { return state; } set { state = value; OnStateChanged?.Invoke(value); } }
    private IInteractionState state;

    private Camera _camera;
    private GraphicRaycaster _worldCanvasRaycaster;
    private GraphicRaycaster _cameraCanvasRaycaster;
    private IDevice _selectedDevice { get { return selectedDevice; } set { var deselected = selectedDevice; selectedDevice = value; OnDeselected?.Invoke(deselected); OnSelected?.Invoke(value);  } }
    private IDevice selectedDevice;

    private void Awake()
    {
        _camera = _cameraCanvas.GetComponentInParent<Camera>();
        _worldCanvasRaycaster = _worldSpaceCanvas.GetComponent<GraphicRaycaster>();
        _cameraCanvasRaycaster = _cameraCanvas.GetComponent<GraphicRaycaster>();
    }

    private void Start()
    {
        CameraController = GetComponent<SimpleCameraController>();
        _editor.OnDeviceDestroyed += DeviceDestroyed;
        ResetState();
    }

    public void ChangeState(IInteractionState newState)
    {
        if (newState == null)
            return;

        if (_state != null)
            _state.StateExit();

        _state = newState;
        _state.StateEnter(this);
    }

    public void ResetState()
    {
        ChangeState(new ObjectInteractionState());
    }

    public void InputKey(KeyCode keyCode)
    {
        _state.InputKey(keyCode);
    }

    public void DestroySelectedDevice()
    {
        _editor.DestroyDevice(_selectedDevice);
    }

    private void DeviceDestroyed(IDevice obj)
    {
        if (obj == _selectedDevice)
        {
            ResetSelect();
            ResetState();
        }
    }

    public void Interact(Vector2 positionOnScreen)
    {
        _state.StateInteract(positionOnScreen);
    }

    public bool RaycastToUI(Vector2 positionOnScreen, out IDevice resultDevice)
    {
        resultDevice = null;
        var eventData = new PointerEventData(_eventSystem);
        eventData.position = positionOnScreen;
        var result = new List<RaycastResult>();

        if (CheckForUIGraphics(eventData))
            return false;

        _worldCanvasRaycaster.Raycast(eventData, result);
        if (result.Count == 0)
            return true;

        resultDevice = result[0].gameObject.GetComponent<IDevice>();
        return true;
    }

    public bool RaycastToWorld(Vector2 positionOnScreen, out Vector3? positionOnWorld)
    {
        positionOnWorld = null;

        var eventData = new PointerEventData(_eventSystem);
        eventData.position = positionOnScreen;
        if (CheckForUIGraphics(eventData))
            return false;

        if (!Physics.Raycast(_camera.ScreenPointToRay(positionOnScreen), out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Default")))
            return false;

        positionOnWorld = hit.point;
        return true;
    }

    private bool CheckForUIGraphics(PointerEventData eventData)
    {
        var result = new List<RaycastResult>();
        _cameraCanvasRaycaster.Raycast(eventData, result);
        if (result.Count > 0)
            return true;

        var children = _cameraCanvasRaycaster.GetComponentsInChildren<GraphicRaycaster>();
        if (children.Length == 0)
            return false;

        foreach (var child in children)
        {
            child.Raycast(eventData, result);
            if (result.Count > 0)
                return true;
        }

        return false;
    }

    public void DestroyAllPathsOfSelectedDevice()
    {
        IConnectable connectable;
        if ((connectable = _selectedDevice.GameObject.GetComponent<IConnectable>()) != null)
            for (int i = connectable.ConnectionPaths.Count - 1; i >= 0; i--)
                _editor.DestroyPath(connectable.ConnectionPaths[i]);
    }

    public void DestroyPathOfSelectedDevice(int index)
    {
        IConnectable connectable;
        if ((connectable = _selectedDevice.GameObject.GetComponent<IConnectable>()) != null && connectable.ConnectionPaths.Count >= index + 1)
            _editor.DestroyPath(connectable.ConnectionPaths[index]);
    }

    public void SetSelect(IDevice device)
    {
        _selectedDevice = device;
    }

    public void ResetSelect()
    {
        _selectedDevice = null;
    }

    private void OnDestroy()
    {
        _editor.OnDeviceDestroyed -= DeviceDestroyed;
    }
}

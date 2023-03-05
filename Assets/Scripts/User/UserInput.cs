using System.Collections.Generic;
using System;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    public bool Locked => _lockSources.Count > 0;

    private SimpleCameraController _controller;
    private Interaction _interaction;
    private Camera _camera;

    private List<object> _lockSources = new List<object>();

    private void Start()
    {
        _camera = GetComponent<Camera>();
        _controller = GetComponent<SimpleCameraController>();
        _interaction = GetComponent<Interaction>();
    }

    private void Update()
    {
        if (Locked)
            return;

        if (IsLeftMouseButtonDown() && !_controller.CameraRotationAllowed)
            _interaction.Interact(Input.mousePosition);

        KeyCode keyCode;

        if (IsUpButtonDown(out keyCode))
            _interaction.InputKey(keyCode);

        if (IsDownButtonDown(out keyCode))
            _interaction.InputKey(keyCode);

        if (IsRightButtonDown(out keyCode))
            _interaction.InputKey(keyCode);

        if (IsLeftButtonDown(out keyCode))
            _interaction.InputKey(keyCode);

        if (IsForwardButtonDown(out keyCode))
            _interaction.InputKey(keyCode);

        if (IsBackwardButtonDown(out keyCode))
            _interaction.InputKey(keyCode);
    }

    public void SetLock(object source)
    {
        if (_lockSources.Contains(source))
            throw new InvalidOperationException();

        _lockSources.Add(source);
    }

    public void RemoveLock(object source)
    {
        if (!_lockSources.Contains(source))
            throw new InvalidOperationException();

        _lockSources.Remove(source);
    }

    public Vector3 GetLookPosition(float maxDistance)
    {
        var ray = _camera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        return ray.GetPoint(maxDistance);
    }

    #region Inputs

    public Vector3 GetInputTranslationDirection()
    {
        if (Locked)
            return Vector3.zero;

        Vector3 direction = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector3.right;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            direction += Vector3.down;
        }
        if (Input.GetKey(KeyCode.E))
        {
            direction += Vector3.up;
        }

        return direction;
    }

    public bool IsCameraRotationButtonDown()
    {
        if (Locked)
            return false;

        return Input.GetKeyDown(KeyCode.LeftControl);
    }

    public Vector2 GetInputLookRotation()
    {
        if (Locked)
            return Vector2.zero;

        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 10;
    }

    public bool IsBoostPressed()
    {
        if (Locked)
            return false;

        return Input.GetKey(KeyCode.LeftShift);
    }

    public bool IsEscapeDown()
    {
        if (Locked)
            return false;

        return Input.GetKeyDown(KeyCode.Escape);
    }

    public bool IsLeftMouseButtonDown()
    {
        if (Locked)
            return false;

        return Input.GetMouseButtonDown(0);
    }

    public bool IsRightMouseButtonDown()
    {
        if (Locked)
            return false;

        return Input.GetMouseButtonDown(1);
    }

    private bool IsForwardButtonDown(out KeyCode keyCode)
    {
        keyCode = KeyCode.UpArrow;
        if (Locked)
            return false;

        return Input.GetKeyDown(keyCode);
    }

    private bool IsLeftButtonDown(out KeyCode keyCode)
    {
        keyCode = KeyCode.LeftArrow;
        if (Locked)
            return false;

        return Input.GetKeyDown(keyCode);
    }

    private bool IsRightButtonDown(out KeyCode keyCode)
    {
        keyCode = KeyCode.RightArrow;
        if (Locked)
            return false;

        return Input.GetKeyDown(keyCode);
    }

    private bool IsBackwardButtonDown(out KeyCode keyCode)
    {
        keyCode = KeyCode.DownArrow;
        if (Locked)
            return false;

        return Input.GetKeyDown(keyCode);
    }

    private bool IsUpButtonDown(out KeyCode keyCode)
    {
        keyCode = KeyCode.Equals;
        if (Locked)
            return false;

        return Input.GetKeyDown(keyCode);
    }

    private bool IsDownButtonDown(out KeyCode keyCode)
    {
        keyCode = KeyCode.Minus;
        if (Locked)
            return false;

        return Input.GetKeyDown(keyCode);
    }

    #endregion
}

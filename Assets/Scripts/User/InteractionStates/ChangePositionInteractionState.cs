using UnityEngine;

public class ChangePositionInteractionState : IInteractionState
{
    private Interaction _context;

    public void InputKey(KeyCode keycode)
    {
        switch (keycode)
        {
            case KeyCode.UpArrow:
                _context.Editor.ChangeDevicePositionByRotation(_context.SelectedDevice, DevicesEditor.Direction.Forward, 0.5f);
                break;
            case KeyCode.DownArrow:
                _context.Editor.ChangeDevicePositionByRotation(_context.SelectedDevice, DevicesEditor.Direction.Backward, 0.5f);
                break;
            case KeyCode.LeftArrow:
                _context.Editor.ChangeDevicePositionByRotation(_context.SelectedDevice, DevicesEditor.Direction.Left, 0.5f);
                break;
            case KeyCode.RightArrow:
                _context.Editor.ChangeDevicePositionByRotation(_context.SelectedDevice, DevicesEditor.Direction.Right, 0.5f);
                break;
            case KeyCode.Equals:
                _context.Editor.ChangeDevicePositionByRotation(_context.SelectedDevice, DevicesEditor.Direction.Up, 0.5f);
                break;
            case KeyCode.Minus:
                _context.Editor.ChangeDevicePositionByRotation(_context.SelectedDevice, DevicesEditor.Direction.Down, 0.5f);
                break;
        }
    }

    public void StateEnter(Interaction context)
    {
        _context = context;
    }

    public void StateExit()
    {

    }

    public void StateInteract(Vector2 positionOnScreen)
    {
        if (!_context.RaycastToWorld(positionOnScreen, out Vector3? positionOnWorld))
            return;

        _context.Editor.ChangeDevicePosition(_context.SelectedDevice, positionOnWorld ?? _context.SelectedDevice.Position);
    }
}

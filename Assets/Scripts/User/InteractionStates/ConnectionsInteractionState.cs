using UnityEngine;

public class ConnectionsInteractionState : IInteractionState
{
    private Interaction _context;

    public void InputKey(KeyCode keycode)
    {
        
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
        if (!_context.RaycastToUI(positionOnScreen, out IDevice device))
            return;

        if (device == null)
            return;

        IConnectable selected, raycasted;
        if ((selected = _context.SelectedDevice.GameObject.GetComponent<IConnectable>()) == null ||
            (raycasted = device.GameObject.GetComponent<IConnectable>()) == null)
            return;

        _context.Editor.ConnectTwoConnectableItems(selected, raycasted);
    }
}

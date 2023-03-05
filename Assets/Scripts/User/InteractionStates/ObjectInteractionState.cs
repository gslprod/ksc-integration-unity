using UnityEngine;

public class ObjectInteractionState : IInteractionState
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

        if (device != null)
            _context.SetSelect(device);
        else
            _context.ResetSelect();
    }
}

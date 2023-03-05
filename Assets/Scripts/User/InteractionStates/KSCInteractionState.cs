using KSC.Clients;
using System.Runtime.Remoting.Contexts;
using UnityEngine;

public class KSCInteractionState : IInteractionState
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

        if (device != null && device is IKSCAgent)
            _context.SetSelect(device);
    }
}


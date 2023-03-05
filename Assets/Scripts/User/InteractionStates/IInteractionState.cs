using UnityEngine;

public interface IInteractionState
{
    void StateEnter(Interaction context);
    void StateInteract(Vector2 positionOnScreen);
    void StateExit();
    void InputKey(KeyCode keycode);
}

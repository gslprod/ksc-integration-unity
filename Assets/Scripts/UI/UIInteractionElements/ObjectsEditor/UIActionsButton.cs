using UnityEngine;
using UnityEngine.UI;

namespace UIGroups.ObjectsEditorGroup
{
    public class UIActionsButton : MonoBehaviour
    {
        [SerializeField] private UIFrameControl _workFrame;
        [SerializeField] private UIFrameControl _actionsFrame;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            SubscribeToButton();
        }

        private void OnDisable()
        {
            UnsubscribeFromButton();
        }

        private void ButtonClick()
        {
            _workFrame.SwitchSubFrame(_actionsFrame);
        }

        private void SubscribeToButton()
        {
            _button.onClick.AddListener(ButtonClick);
        }

        private void UnsubscribeFromButton()
        {
            _button.onClick.RemoveListener(ButtonClick);
        }
    }
}


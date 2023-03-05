using UnityEngine;
using UnityEngine.UI;

namespace UIGroups.ObjectsEditorGroup
{
    public class UIKSCAPIButton : MonoBehaviour
    {
        [SerializeField] private UIFrameControl _workFrame;
        [SerializeField] private UIFrameControl _kscFrame;

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
            _workFrame.SwitchSubFrame(_kscFrame);
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

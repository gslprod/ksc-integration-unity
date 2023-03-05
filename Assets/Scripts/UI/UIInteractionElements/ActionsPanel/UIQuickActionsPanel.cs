using UnityEngine;
using UnityEngine.UI;

namespace UIGroups.QuickActions
{
    public class UIQuickActionsPanel : MonoBehaviour
    {
        [SerializeField] private DevicesEditor _devicesEditor;
        [SerializeField] private UIFrameControl _actionsFrame;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private Button _autoFloorButton;
        [SerializeField] private Button _autoAssignAgentsButton;
        [SerializeField] private Button _deleteAllButton;

        private void OnEnable()
        {
            SubscribeToUICallbacks();
        }

        private void OnDisable()
        {
            UnsubscribeFromUICallbacks();
        }

        private void CancelButtonClick()
        {
            _actionsFrame.SetActive(false);
        }

        private void AutoFloorButtonClick()
        {
            _devicesEditor.AutoCalculateFloorForAllElements();
        }

        private void AutoAssignAgentsClick()
        {
            _devicesEditor.AutoAssignAgents();
        }

        private void DeleteAllClick()
        {
            _devicesEditor.DestroyAllDevices();
        }

        private void SubscribeToUICallbacks()
        {
            _cancelButton.onClick.AddListener(CancelButtonClick);
            _autoFloorButton.onClick.AddListener(AutoFloorButtonClick);
            _deleteAllButton.onClick.AddListener(DeleteAllClick);
            _autoAssignAgentsButton.onClick.AddListener(AutoAssignAgentsClick);
        }

        private void UnsubscribeFromUICallbacks()
        {
            _cancelButton.onClick.RemoveListener(CancelButtonClick);
            _autoFloorButton.onClick.RemoveListener(AutoFloorButtonClick);
            _deleteAllButton.onClick.RemoveListener(DeleteAllClick);
            _autoAssignAgentsButton.onClick.RemoveListener(AutoAssignAgentsClick);

        }
    }
}

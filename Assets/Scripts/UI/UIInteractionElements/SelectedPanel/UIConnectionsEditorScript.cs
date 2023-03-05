using KSC.Clients;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UIGroups.SelectedView
{
    public class UIConnectionsEditorScript : MonoBehaviour
    {
        [SerializeField] private Interaction _interaction;
        [SerializeField] private TMP_Dropdown _pathsDropdown;
        [SerializeField] private TextMeshProUGUI _selectedDeviceText;
        [SerializeField] private Button _quitButton;
        [SerializeField] private Button _deleteSelectedPathButton;
        [SerializeField] private Button _deleteAllPathsButton;

        private UIFrameControl _thisFrame;

        private void Start()
        {
            _thisFrame = GetComponent<UIFrameControl>();

            UpdateVisibility();
            SubscribeToInteraction();
        }

        private void OnEnable()
        {
            SubscribeToUICallbacks();
        }

        private void OnDisable()
        {
            UnsubscribeFromUICallbacks();
        }

        private void OnDestroy()
        {
            UnsubscribeFromInteraction();
        }

        private void InitPathsDropdown()
        {
            if (_interaction.SelectedDevice == null)
                return;

            List<string> pathNames = new List<string>();
            IConnectable connectable;
            if ((connectable = _interaction.SelectedDevice.GameObject.GetComponent<IConnectable>()) == null)
                return;

            foreach (var path in connectable.ConnectionPaths)
            {
                var other = path.First == _interaction.SelectedDevice ? path.Second : path.First;
                pathNames.Add($"{other.Name.NameFormat()} ({other.Type})".Clamp(50));
            }

            _pathsDropdown.ClearOptions();
            _pathsDropdown.AddOptions(pathNames);
        }

        private void DeleteSelectedPathButtonClick()
        {
            _interaction.DestroyPathOfSelectedDevice(_pathsDropdown.value);
        }

        private void DeleteAllPathsButtonClick()
        {
            _interaction.DestroyAllPathsOfSelectedDevice();
        }

        private void QuitButtonClick()
        {
            _interaction.ResetState();
        }

        private void PathCreated(ConnectionPath path)
        {
            InitPathsDropdown();
        }

        private void PathDestroyed(ConnectionPath path)
        {
            InitPathsDropdown();
        }

        private void InteractionStateChanged(IInteractionState obj)
        {
            UpdateVisibility();
        }

        private void InteractionSelected(IDevice obj)
        {
            UpdateVisibility();

            if (obj == null)
                return;

            SubscribeToDevice(obj);
        }

        private void InteractionDeselected(IDevice obj)
        {
            if (obj == null)
                return;

            UnsubscribeFromDevice(obj);
        }

        private void UpdateUIName(IDevice device)
        {
            _selectedDeviceText.text = $"{device.Name.NameFormat()} ({device.Type})";
        }

        private void UpdateVisibility()
        {
            bool isActive = _interaction.SelectedDevice != null && _interaction.State is ConnectionsInteractionState;

            _thisFrame.SetActive(isActive);
            if (isActive)
                InitPathsDropdown();
        }

        private void SubscribeToInteraction()
        {
            _interaction.OnStateChanged += InteractionStateChanged;
            _interaction.OnSelected += InteractionSelected;
            _interaction.OnDeselected += InteractionDeselected;
            _interaction.Editor.OnPathDestroyed += PathDestroyed;
            _interaction.Editor.OnPathCreated += PathCreated;
        }

        private void UnsubscribeFromInteraction()
        {
            _interaction.OnStateChanged -= InteractionStateChanged;
            _interaction.OnSelected -= InteractionSelected;
            _interaction.OnDeselected -= InteractionDeselected;
            _interaction.Editor.OnPathDestroyed -= PathDestroyed;
            _interaction.Editor.OnPathCreated -= PathCreated;
        }

        private void SubscribeToUICallbacks()
        {
            _quitButton.onClick.AddListener(QuitButtonClick);
            _deleteSelectedPathButton.onClick.AddListener(DeleteSelectedPathButtonClick);
            _deleteAllPathsButton.onClick.AddListener(DeleteAllPathsButtonClick);
        }

        private void UnsubscribeFromUICallbacks()
        {
            _quitButton.onClick.RemoveListener(QuitButtonClick);
            _deleteSelectedPathButton.onClick.RemoveListener(DeleteSelectedPathButtonClick);
            _deleteAllPathsButton.onClick.RemoveListener(DeleteAllPathsButtonClick);
        }

        private void SubscribeToDevice(IDevice device)
        {
            UpdateUIName(device);

            device.OnNameChanged += UpdateUIName;
        }

        private void UnsubscribeFromDevice(IDevice device)
        {
            device.OnNameChanged -= UpdateUIName;
        }
    }
}
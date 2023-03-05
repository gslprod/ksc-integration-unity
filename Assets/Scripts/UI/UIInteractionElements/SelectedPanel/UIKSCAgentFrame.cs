using KSC;
using KSC.Clients;
using TMPro;
using UIGroups.KSC;
using UIGroups.KSCAgents;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UIGroups.SelectedView
{
    public class UIKSCAgentFrame : MonoBehaviour
    {
        [SerializeField] private UserInput _input;
        [SerializeField] private UIKSCAgentsPanel _agentsPanel;
        [SerializeField] private UIFrameControl _agentsSelectFrame;
        [SerializeField] private Interaction _interaction;
        [SerializeField] private TextMeshProUGUI _selectedDeviceText;
        [SerializeField] private TextMeshProUGUI _assignStateText;
        [SerializeField] private TextMeshProUGUI _agentInfoText;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Button _quitButton;
        [SerializeField] private Button _assignButton;
        [SerializeField] private Button _removeButton;
        [SerializeField] private TMP_InputField _ipInputField;
        [SerializeField] private TMP_InputField _hostNameInputField;
        [SerializeField] private Toggle _overrideNameToggle;

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

        private void AssignButtonClick()
        {
            _agentsSelectFrame.SetActive(true);

            SubscribeToAgentsSelectPanel();
        }

        private void RemoveButtonClick()
        {
            if (_interaction.SelectedDevice is IKSCAgent agentObject)
                agentObject.RemoveKSCAgent();
        }

        private void SetAgentToSelectedObject(Agent agent)
        {
            if (_interaction.SelectedDevice is IKSCAgent agentObject)
                agentObject.SetKSCAgent(agent);
        }

        private void IPValueChanged(string value)
        {
            if (_interaction.SelectedDevice is IKSCAgent agentObject)
                agentObject.SetIP(value);
        }

        private void HostNameValueChanged(string value)
        {
            if (_interaction.SelectedDevice is IKSCAgent agentObject)
                agentObject.SetHostName(value);
        }

        private void QuitButtonClick()
        {
            _interaction.ResetState();
        }

        private void AnyInputFieldSelect(string value)
        {
            _input.SetLock(this);
        }

        private void AnyInpuFieldDeselect(string value)
        {
            _input.RemoveLock(this);
        }

        private void OverrideNameToggleValueChanged(bool value)
        {
            if (_interaction.SelectedDevice is IKSCAgent agentObject)
                agentObject.SetKSCNameUsing(value);
        }

        private void AgentInfoUpdated(Agent agent)
        {
            UpdateUIAgentInfo(agent);
            UpdateBackGround(agent);
        }

        private void AgentRemoved(Agent agent)
        {
            UnsubscribeFromAgent(agent);

            UpdateUIByAgent(null);
        }

        private void AgentAdded(Agent agent)
        {
            SubscribeToAgent(agent);

            UpdateUIByAgent(agent);
        }

        private void UpdateUIByAgent(Agent agent)
        {
            var agentAssigned = agent != null;

            UpdateInputFieldsAvailability(agentAssigned);
            UpdateUIAgentAssignmentStatus(agentAssigned);
            UpdateUIAgentInfo(agent);
            UpdateBackGround(agent);
            UpdateButtonsVisibility(agentAssigned);
        }

        private void UpdateUIHostName(IKSCAgent agentObject)
        {
            _hostNameInputField.SetTextWithoutNotify(agentObject.HostName);
        }

        private void UpdateUIIPAddress(IKSCAgent agentObject)
        {
            _ipInputField.SetTextWithoutNotify(agentObject.IPAddress);
        }

        private void UpdateUIKSCNameUsing(IKSCAgent agentObject)
        {
            _overrideNameToggle.SetIsOnWithoutNotify(agentObject.UseKSCAgentName);
        }

        private void UpdateInputFieldsAvailability(bool agentAssigned)
        {
            _ipInputField.interactable = !agentAssigned;
            _hostNameInputField.interactable = !agentAssigned;
        }

        private void UpdateUIAgentAssignmentStatus(bool agentAssigned)
        {
            _assignStateText.text = agentAssigned ? "Объект связан с агентом" : "Объект не связан с агентом";
        }

        private void UpdateUIAgentInfo(Agent agent)
        {
            var agentAssigned = agent != null;

            _agentInfoText.gameObject.SetActive(agentAssigned);

            if (!agentAssigned)
                return;

            _agentInfoText.text = $"IP: {agent.IP}\r\n" +
                                        $"Имя: {agent.Name}\r\n" +
                                        $"Группа: {agent.Group}\r\n" +
                                        $"Состояние: {agent.Status.StatusEnumToText()}";
        }

        private void UpdateBackGround(Agent agent)
        {
            var agentAssigned = agent != null;
            if (!agentAssigned)
            {
                _backgroundImage.color = new Color(0, 0, 0, 0.35f);
                return;
            }

            _backgroundImage.color = agent.Status switch
            {
                AgentStatus.OK => new Color(0, 0.8f, 0, 0.35f),
                AgentStatus.Critical => new Color(0.8f, 0, 0, 0.35f),
                AgentStatus.Warning => new Color(0.8f, 0.76f, 0, 0.35f),
                _ => new Color(0, 0, 0, 0.35f),
            };
        }

        private void UpdateButtonsVisibility(bool agentAssigned)
        {
            _assignButton.gameObject.SetActive(!agentAssigned);
            _removeButton.gameObject.SetActive(agentAssigned);
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
            if (obj is IKSCAgent kscAgent)
                SubscribeToAgentObject(kscAgent);
        }

        private void InteractionDeselected(IDevice obj)
        {
            if (obj == null)
                return;

            UnsubscribeFromDevice(obj);
            if (obj is IKSCAgent kscAgent)
                UnsubscribeFromAgentObject(kscAgent);
        }

        private void SelectionsPanelUIAgentClick(UIAgent uiAgent)
        {
            SetAgentToSelectedObject(uiAgent.Agent);

            _agentsSelectFrame.SetActive(false);

            UnsubscribeFromAgentsSelectPanel();
        }

        private void UpdateUIName(IDevice device)
        {
            _selectedDeviceText.text = $"{device.Name.NameFormat()} ({device.Type})";
        }

        private void UpdateVisibility()
        {
            bool isActive = _interaction.SelectedDevice != null && _interaction.State is KSCInteractionState;

            _thisFrame.SetActive(isActive);
        }

        private void SubscribeToInteraction()
        {
            _interaction.OnSelected += InteractionSelected;
            _interaction.OnDeselected += InteractionDeselected;
            _interaction.OnStateChanged += InteractionStateChanged;
        }

        private void UnsubscribeFromInteraction()
        {
            _interaction.OnSelected -= InteractionSelected;
            _interaction.OnDeselected -= InteractionDeselected;
            _interaction.OnStateChanged -= InteractionStateChanged;
        }

        private void SubscribeToUICallbacks()
        {
            _quitButton.onClick.AddListener(QuitButtonClick);
            _assignButton.onClick.AddListener(AssignButtonClick);
            _removeButton.onClick.AddListener(RemoveButtonClick);
            _ipInputField.onValueChanged.AddListener(IPValueChanged);
            _hostNameInputField.onValueChanged.AddListener(HostNameValueChanged);
            _ipInputField.onSelect.AddListener(AnyInputFieldSelect);
            _hostNameInputField.onSelect.AddListener(AnyInputFieldSelect);
            _ipInputField.onDeselect.AddListener(AnyInpuFieldDeselect);
            _hostNameInputField.onDeselect.AddListener(AnyInpuFieldDeselect);
            _overrideNameToggle.onValueChanged.AddListener(OverrideNameToggleValueChanged);
        }

        private void UnsubscribeFromUICallbacks()
        {
            _quitButton.onClick.RemoveListener(QuitButtonClick);
            _assignButton.onClick.RemoveListener(AssignButtonClick);
            _removeButton.onClick.RemoveListener(RemoveButtonClick);
            _ipInputField.onValueChanged.RemoveListener(IPValueChanged);
            _hostNameInputField.onValueChanged.RemoveListener(HostNameValueChanged);
            _ipInputField.onSelect.RemoveListener(AnyInputFieldSelect);
            _hostNameInputField.onSelect.RemoveListener(AnyInputFieldSelect);
            _ipInputField.onDeselect.RemoveListener(AnyInpuFieldDeselect);
            _hostNameInputField.onDeselect.RemoveListener(AnyInpuFieldDeselect);
            _overrideNameToggle.onValueChanged.RemoveListener(OverrideNameToggleValueChanged);
        }

        private void SubscribeToAgentObject(IKSCAgent agentObject)
        {
            var agentAssigned = agentObject.Agent != null;
            if (agentAssigned)
                SubscribeToAgent(agentObject.Agent);

            UpdateUIByAgent(agentObject.Agent);
            UpdateUIHostName(agentObject);
            UpdateUIIPAddress(agentObject);
            UpdateUIKSCNameUsing(agentObject);
           
            agentObject.OnAgentSet += AgentAdded;
            agentObject.OnAgentRemoved += AgentRemoved;
            agentObject.OnHostNameChanged += UpdateUIHostName;
            agentObject.OnIPAddressChanged += UpdateUIIPAddress;
            agentObject.OnKSCNameUsingChanged += UpdateUIKSCNameUsing;

        }

        private void UnsubscribeFromAgentObject(IKSCAgent agent)
        {
            var agentAssigned = agent.Agent != null;
            if (agentAssigned)
                UnsubscribeFromAgent(agent.Agent);

            agent.OnAgentSet -= AgentAdded;
            agent.OnAgentRemoved -= AgentRemoved;
            agent.OnHostNameChanged -= UpdateUIHostName;
            agent.OnIPAddressChanged -= UpdateUIIPAddress;
            agent.OnKSCNameUsingChanged -= UpdateUIKSCNameUsing;

        }

        private void SubscribeToAgent(Agent agent)
        {
            agent.OnInfoUpdated += AgentInfoUpdated;
        }

        private void UnsubscribeFromAgent(Agent agent)
        {
            agent.OnInfoUpdated -= AgentInfoUpdated;
        }

        private void SubscribeToAgentsSelectPanel()
        {
            _agentsPanel.OnUIAgentClick += SelectionsPanelUIAgentClick;
            _agentsPanel.OnCancelClick += UnsubscribeFromAgentsSelectPanel;
        }

        private void UnsubscribeFromAgentsSelectPanel()
        {
            _agentsPanel.OnUIAgentClick -= SelectionsPanelUIAgentClick;
            _agentsPanel.OnCancelClick -= UnsubscribeFromAgentsSelectPanel;
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
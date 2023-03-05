using KSC;
using KSC.Clients;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UIGroups.DialogBox;
using UIGroups.KSC;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UIGroups.KSCAgents
{
    public class UIKSCAgentsPanel : MonoBehaviour
    {
        public event Action<UIAgent> OnUIAgentClick;
        public event Action OnCancelClick;

        [SerializeField] private UserInput _input;
        [SerializeField] private DevicesEditor _editor;
        [SerializeField] private UICore _uiCore;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private UIFrameControl _kscAgentsFrame;
        [SerializeField] private UIAgent _uiAgentsPrefab;
        [SerializeField] private Transform _uiAgentsParent;
        [Header("Agent actions buttons")]
        [SerializeField] private Button _updateAllButton;
        [SerializeField] private Button _searchButton;
        [SerializeField] private Button _resetButton;
        [SerializeField] private TMP_InputField _searchInputField;

        private List<Agent> _freeAgents = new List<Agent>();
        private List<IKSCAgent> _kscObjects = new List<IKSCAgent>();
        private List<UIAgent> _uiAgents = new List<UIAgent>();

        private void Awake()
        {
            SubscribeToRegisteredServers();
            SubscribeToServerRegisters();
            SubscribeToAllKSCObjects();
            SubscribeToEditor();
        }

        private void OnEnable()
        {
            SubscribeToButtons();
            _input.SetLock(this);
        }

        private void OnDisable()
        {
            UnsubscribeFromButtons();
            _input.RemoveLock(this);
        }

        private void OnDestroy()
        {
            UnsubscribeFromRegisteredServers();
            UnsubscribeFromServerRegisters();
            UnsubscribeFromAllKSCObjects();
            UnsubscribeFromEditor();
        }

        private void GetFreeAgentsFromServer(Server server)
        {
            foreach (var agent in server.Agents)
                if (!_kscObjects.Exists((kscObject) => kscObject.Agent == agent) && !_freeAgents.Contains(agent))
                    AddFreeAgent(agent);
        }

        private void RemoveFreeAgentsOfServer(Server server)
        {
            foreach (var agent in server.Agents)
            {
                int index;
                if ((index = _freeAgents.IndexOf(agent)) != -1)
                    RemoveFreeAgentAt(index);
            }
        }

        private void AddFreeAgent(Agent agent)
        {
            CreateUIAgent(agent);
        }

        private void RemoveFreeAgent(Agent agent)
        {
            DestroyUIAgent(agent);
        }

        private void RemoveFreeAgentAt(int index)
        {
            var agent = _freeAgents[index];

            DestroyUIAgent(agent);
        }

        private void CreateUIAgent(Agent source)
        {
            var createdUIElement = Instantiate(_uiAgentsPrefab, _uiAgentsParent);
            createdUIElement.Init(source);
            SubscribeToUIAgent(createdUIElement);
        }

        private void DestroyUIAgent(Agent source)
        {
            var toDestroy = _uiAgents.Find((uiAgent) => uiAgent.Agent == source);
            Destroy(toDestroy);
        }

        private void CancelClick()
        {
            _kscAgentsFrame.SetActive(false);
            OnCancelClick?.Invoke();
        }

        private async void UpdateAllClick()
        {
            var buttons = new DialogBoxButton[]
            {
                new DialogBoxButton("Принудительно закрыть (не рекомендуется)", () =>{ }, true, new Color(1, 0, 0))
            };
            var loadingDialogBox = DialogBoxElement.Factory.Create(_uiCore, $"Обновление информации о серверах",
                "Пожалуйста, подождите. Это может занять некоторое время. Данное окно закроется автоматически после завершения операции.\n" +
                "Закрывать данное окно вручную не рекомендуется, однако в случае зависания вы можете сделать это.",
                buttons);

            try
            {
                DisableFilter();

                var registeredServers = Server.RegisteredServers;
                var tasks = new Task[registeredServers.Length];

                for (int i = 0; i < registeredServers.Length; i++)
                    tasks[i] = registeredServers[i].UpdateInfoAsync();

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                var errorDBbuttons = new DialogBoxButton[]
                {
                    new DialogBoxButton("Закрыть", () =>{ }, true, new Color(0.5f, 0.5f, 0.5f))
                };
                DialogBoxElement.Factory.Create(_uiCore, "Сбой обновления информации о серверах",
                    $"Причина: {ex.GetType().Name} - {ex.Message}",
                    errorDBbuttons);
            }
            finally
            {
                loadingDialogBox.Close();
            }
        }

        private void SearchClick()
        {
            EnableFilter();
        }

        private void ResetClick()
        {
            DisableFilter();
        }

        private void EnableFilter()
        {
            var searchQuery = _searchInputField.text;
            _uiAgents.ForEach((uiAgent) =>
            {
                uiAgent.gameObject
                        .SetActive(searchQuery
                            .ContainsIn(false,
                                uiAgent.Agent.Name,
                                uiAgent.Agent.IP,
                                uiAgent.Agent.Group));
            });
        }

        private void DisableFilter()
        {
            _uiAgents.ForEach((uiAgent) => uiAgent.gameObject.SetActive(true));
        }

        private void SubscribeToButtons()
        {
            _cancelButton.onClick.AddListener(CancelClick);
            _updateAllButton.onClick.AddListener(UpdateAllClick);
            _searchButton.onClick.AddListener(SearchClick);
            _resetButton.onClick.AddListener(ResetClick);
        }

        private void UnsubscribeFromButtons()
        {
            _cancelButton.onClick.RemoveListener(CancelClick);
            _updateAllButton.onClick.RemoveListener(UpdateAllClick);
            _searchButton.onClick.RemoveListener(SearchClick);
            _resetButton.onClick.RemoveListener(ResetClick);
        }

        private void SubscribeToServerRegisters()
        {
            Server.OnServerRegistered += SubscribeToServer;
            Server.OnServerUnregistered += UnsubscribeFromServer;
        }

        private void UnsubscribeFromServerRegisters()
        {
            Server.OnServerRegistered -= SubscribeToServer;
            Server.OnServerUnregistered -= UnsubscribeFromServer;
        }

        private void SubscribeToRegisteredServers()
        {
            foreach (var server in Server.RegisteredServers)
                SubscribeToServer(server);
        }

        private void UnsubscribeFromRegisteredServers()
        {
            foreach (var server in Server.RegisteredServers)
                UnsubscribeFromServer(server);
        }

        private void SubscribeToServer(Server server)
        {
            GetFreeAgentsFromServer(server);

            server.OnInfoUpdated += ServerInfoUpdated;
        }

        private void UnsubscribeFromServer(Server server)
        {
            RemoveFreeAgentsOfServer(server);

            server.OnInfoUpdated -= ServerInfoUpdated;
        }

        private void SubscribeToUIAgent(UIAgent uiAgent)
        {
            _uiAgents.Add(uiAgent);
            _freeAgents.Add(uiAgent.Agent);

            uiAgent.OnDispose += UIAgentDispose;
            uiAgent.OnButtonClick += UIAgentClick;
        }

        private void UnsubscribeFromUIAgent(UIAgent uiAgent)
        {
            _uiAgents.Remove(uiAgent);
            _freeAgents.Remove(uiAgent.Agent);

            uiAgent.OnDispose -= UIAgentDispose;
            uiAgent.OnButtonClick -= UIAgentClick;
        }

        private void SubscribeToKSCObject(IKSCAgent kscObject)
        {
            kscObject.OnAgentSet += RemoveFreeAgent;
            kscObject.OnAgentRemoved += AddFreeAgent;
            _kscObjects.Add(kscObject);
        }

        private void UnsubscribeFromKSCObject(IKSCAgent kscObject)
        {
            kscObject.OnAgentSet -= RemoveFreeAgent;
            kscObject.OnAgentRemoved -= AddFreeAgent;
            _kscObjects.Remove(kscObject);
        }

        private void SubscribeToAllKSCObjects()
        {
            var kscObjects = _editor.GetDevicesOfType<IKSCAgent>();
            foreach (var kscObject in kscObjects)
                SubscribeToKSCObject(kscObject);
        }

        private void UnsubscribeFromAllKSCObjects()
        {
            for (int i = _kscObjects.Count - 1; i >= 0; i--)
                UnsubscribeFromKSCObject(_kscObjects[i]);
        }

        private void SubscribeToEditor()
        {
            _editor.OnDeviceCreated += DeviceCreated;
            _editor.OnDeviceDestroyed += DeviceDestroyed;
        }

        private void UnsubscribeFromEditor()
        {
            _editor.OnDeviceCreated -= DeviceCreated;
            _editor.OnDeviceDestroyed -= DeviceDestroyed;
        }

        private void DeviceCreated(IDevice device)
        {
            if (device is IKSCAgent kscObject)
                SubscribeToKSCObject(kscObject);
        }

        private void DeviceDestroyed(IDevice device)
        {
            if (device is IKSCAgent kscObject)
                UnsubscribeFromKSCObject(kscObject);
        }

        private void ServerInfoUpdated(Server server)
        {
            GetFreeAgentsFromServer(server);
        }

        private void UIAgentDispose(UIAgent uiAgent)
        {
            UnsubscribeFromUIAgent(uiAgent);
        }

        private void UIAgentClick(UIAgent uiAgent)
        {
            OnUIAgentClick?.Invoke(uiAgent);
        }
    }
}
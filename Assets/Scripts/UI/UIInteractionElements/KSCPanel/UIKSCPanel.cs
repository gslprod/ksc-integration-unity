using KSC;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UIGroups.DialogBox;
using UnityEngine;
using UnityEngine.UI;

namespace UIGroups.KSC
{
    public class UIKSCPanel : MonoBehaviour
    {
        [SerializeField] private UserInput _input;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private UIFrameControl _kscFrame;
        [SerializeField] private UIFrameControl _authFrame;
        [SerializeField] private UIServer _uiServerPrefab;
        [SerializeField] private Transform _uiServersParent;
        [Header("Server actions buttons")]
        [SerializeField] private Button _createButton;
        [SerializeField] private Button _deleteSelectedButton;
        [SerializeField] private Button _updateSelectedButton;
        [SerializeField] private Button _updateAllButton;

        private List<UIServer> _serverElements = new List<UIServer>();
        private UIServer _selectedServerElement;
        private UICore _uiCore;

        private void Awake()
        {
            SubscribeToServerRegisters();
            UpdateButtonsActivity();

            _uiCore = GetComponentInParent<UICore>();
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
            UnsubscribeFromServerRegisters();
        }

        private void AddServer(Server server)
        {
            var createdUIElement = Instantiate(_uiServerPrefab, _uiServersParent);
            createdUIElement.Init(server);
            SubscribeToUIServer(createdUIElement);

            _serverElements.Add(createdUIElement);

            UpdateServerInfo(createdUIElement);

            UpdateButtonsActivity();
        }

        private async void DeleteServer(UIServer uiServer)
        {
            var buttons = new DialogBoxButton[]
            {
                new DialogBoxButton("Принудительно закрыть (не рекомендуется)", () =>{ }, true, new Color(1, 0, 0))
            };
            var loadingDialogBox = DialogBoxElement.Factory.Create(_uiCore, "Отключение от сервера",
                "Пожалуйста, подождите. Это может занять некоторое время. Данное окно закроется автоматически после завершения операции.\n" +
                "Закрывать данное окно вручную не рекомендуется, однако в случае зависания вы можете сделать это.",
                buttons);

            try
            {
                await uiServer.Server.DisposeAsync();
            }
            catch (Exception e)
            {
                var errorDBbuttons = new DialogBoxButton[]
                {
                    new DialogBoxButton("Закрыть", () =>{ }, true, new Color(0.5f, 0.5f, 0.5f))
                };
                var errorDialogBox = DialogBoxElement.Factory.Create(_uiCore, "Сбой отключения от сервера",
                    $"Причина: {e.GetType().Name} - {e.Message}",
                    errorDBbuttons);
            }
            finally
            {
                loadingDialogBox.Close();
            }
        }

        private async void UpdateServerInfo(UIServer uiServer)
        {
            var buttons = new DialogBoxButton[]
            {
                new DialogBoxButton("Принудительно закрыть (не рекомендуется)", () =>{ }, true, new Color(1, 0, 0))
            };
            var loadingDialogBox = DialogBoxElement.Factory.Create(_uiCore, $"Обновление информации о сервере {uiServer.Server.IPAddress}",
                "Пожалуйста, подождите. Это может занять некоторое время. Данное окно закроется автоматически после завершения операции.\n" +
                "Закрывать данное окно вручную не рекомендуется, однако в случае зависания вы можете сделать это.",
                buttons);

            try
            {
                await uiServer.Server.UpdateInfoAsync();
            }
            catch (Exception e)
            {
                var errorDBbuttons = new DialogBoxButton[]
                {
                    new DialogBoxButton("Закрыть", () =>{ }, true, new Color(0.5f, 0.5f, 0.5f))
                };
                var errorDialogBox = DialogBoxElement.Factory.Create(_uiCore, $"Сбой обновления информации о сервере {uiServer.Server.IPAddress}",
                    $"Причина: {e.GetType().Name} - {e.Message}",
                    errorDBbuttons);
            }
            finally
            {
                loadingDialogBox.Close();
            }
        }

        private async void UpdateAllServersInfoAsync()
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
                var tasks = new Task[_serverElements.Count];
                for (int i = 0; i < _serverElements.Count; i++)
                    tasks[i] = _serverElements[i].Server.UpdateInfoAsync();

                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                var errorDBbuttons = new DialogBoxButton[]
                {
                    new DialogBoxButton("Закрыть", () =>{ }, true, new Color(0.5f, 0.5f, 0.5f))
                };
                var errorDialogBox = DialogBoxElement.Factory.Create(_uiCore, "Сбой обновления информации о серверах",
                    $"Причина: {e.GetType().Name} - {e.Message}",
                    errorDBbuttons);
            }
            finally
            {
                loadingDialogBox.Close();
            }
        }

        private void UIServerDispose(UIServer disposingElement)
        {
            UnsubscribeFromUIServer(disposingElement);
            if (disposingElement == _selectedServerElement)
                ResetSelect();

            _serverElements.Remove(disposingElement);

            UpdateButtonsActivity();
        }

        private void UIServerButtonClick(UIServer uiServer)
            => SelectServerElement(uiServer);

        private void SelectServerElement(UIServer toSelect)
        {
            if (_selectedServerElement != null)
                _selectedServerElement.ChangeBackGroundColor(new Color(0.25f, 0.25f, 0.25f, 0.75f));
            toSelect.ChangeBackGroundColor(new Color(0f, 0.75f, 1f, 1f));

            _selectedServerElement = toSelect;
            UpdateButtonsActivity();
        }

        private void ResetSelect()
        {
            _selectedServerElement.ChangeBackGroundColor(new Color(0.25f, 0.25f, 0.25f, 0.75f));

            _selectedServerElement = null;
            UpdateButtonsActivity();
        }

        private void UpdateButtonsActivity()
        {
            var isSelected = _selectedServerElement != null;
            _updateSelectedButton.interactable = isSelected;
            _deleteSelectedButton.interactable = isSelected;

            var isUIServersExist = _serverElements.Count != 0;
            _updateAllButton.interactable = isUIServersExist;
        }

        private void CancelClick()
        {
            _kscFrame.SetActive(false);
        }

        private void CreateClick()
        {
            _authFrame.SetActive(true);
        }

        private void DeleteSelectedClick()
        {
            DeleteServer(_selectedServerElement);
        }

        private void UpdateSelectedClick()
        {
            UpdateServerInfo(_selectedServerElement);
        }

        private void UpdateAllClick()
        {
            UpdateAllServersInfoAsync();
        }

        private void SubscribeToUIServer(UIServer uiServer)
        {
            uiServer.OnButtonClick += UIServerButtonClick;
            uiServer.OnDispose += UIServerDispose;
        }

        private void UnsubscribeFromUIServer(UIServer uiServer)
        {
            uiServer.OnButtonClick -= UIServerButtonClick;
            uiServer.OnDispose -= UIServerDispose;
        }

        private void SubscribeToButtons()
        {
            _cancelButton.onClick.AddListener(CancelClick);
            _createButton.onClick.AddListener(CreateClick);
            _deleteSelectedButton.onClick.AddListener(DeleteSelectedClick);
            _updateSelectedButton.onClick.AddListener(UpdateSelectedClick);
            _updateAllButton.onClick.AddListener(UpdateAllClick);
        }

        private void UnsubscribeFromButtons()
        {
            _cancelButton.onClick.RemoveListener(CancelClick);
            _createButton.onClick.RemoveListener(CreateClick);
            _deleteSelectedButton.onClick.RemoveListener(DeleteSelectedClick);
            _updateSelectedButton.onClick.RemoveListener(UpdateSelectedClick);
            _updateAllButton.onClick.RemoveListener(UpdateAllClick);
        }

        private void SubscribeToServerRegisters()
        {
            Server.OnServerRegistered += AddServer;
        }

        private void UnsubscribeFromServerRegisters()
        {
            Server.OnServerRegistered -= AddServer;
        }
    }
}
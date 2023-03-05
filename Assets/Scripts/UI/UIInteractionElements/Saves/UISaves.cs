using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SFB;
using System;
using Utilities;

namespace UIGroups.SavePanel
{
    public class UISaves : MonoBehaviour
    {
        [SerializeField] private DevicesEditor _editor;
        [SerializeField] private TextMeshProUGUI _resultsText;
        [SerializeField] private Image _resultsImage;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _loadButton;
        [SerializeField] private Button _loadObsoleteButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private UIFrameControl _saveFrame;

        private void Start()
        {
            SubcribeToSaveCallbacks();
        }

        private void OnEnable()
        {
            SubscribeToButtons();
        }

        private void OnDisable()
        {
            UnsubscribeFromButtons();
        }

        private void OnDestroy()
        {
            UnsubscribeFromSaveCallbacks();
        }

        private void SaveOrLoadResult(bool arg1, string arg2)
        {
            _resultsText.text = arg2.Clamp(300);
            _resultsImage.color = arg1 ? new Color(0, 0.75f, 0, 0.2f) : new Color(0.75f, 0, 0, 0.2f);
        }

        private void SaveClick()
        {
            var extensions = new[] { new ExtensionFilter("Файл сохранения формата JSON", "json") };

            StandaloneFileBrowser.SaveFilePanelAsync("Cохранение", "", "Сохранение Карты Устройств " + DateTime.Now.ToString().Replace(":", "."), extensions,
                (savePath) =>
                {
                    if (string.IsNullOrEmpty(savePath))
                    {
                        SaveOrLoadResult(false, "Процесс сохранения был отменен");
                        return;
                    }

                    _editor.SaveAllDevices(savePath, true);
                });
        }

        private void LoadClick()
        {
            var extensions = new[] { new ExtensionFilter("Файл сохранения формата JSON", "json") };

            StandaloneFileBrowser.OpenFilePanelAsync("Открытие сохранения", "", extensions, false,
                (path) =>
                {
                    if (path.Length == 0)
                    {
                        SaveOrLoadResult(false, "Процесс открытия сохранения был отменен");
                        return;
                    }

                    _editor.LoadDevices(path[0]);
                });
        }

        private void LoadObsoleteClick()
        {
            var extensions = new[] { new ExtensionFilter("Устаревший текстовый файл сохранения", "txt") };

            StandaloneFileBrowser.OpenFilePanelAsync("Открытие сохранения", "", extensions, false,
                (path) =>
                {
                    if (path.Length == 0)
                    {
                        SaveOrLoadResult(false, "Процесс открытия сохранения был отменен");
                        return;
                    }

                    _editor.LoadDevicesObsolete(path[0]);
                });
        }

        private void CancelClick()
        {
            _saveFrame.SetActive(false);
        }

        private void SubscribeToButtons()
        {
            _saveButton.onClick.AddListener(SaveClick);
            _loadButton.onClick.AddListener(LoadClick);
            _cancelButton.onClick.AddListener(CancelClick);
            _loadObsoleteButton.onClick.AddListener(LoadObsoleteClick);
        }

        private void SubcribeToSaveCallbacks()
        {
            _editor.OnSaveResult += SaveOrLoadResult;
            _editor.OnLoadResult += SaveOrLoadResult;
        }

        private void UnsubscribeFromButtons()
        {
            _saveButton.onClick.RemoveListener(SaveClick);
            _loadButton.onClick.RemoveListener(LoadClick);
            _cancelButton.onClick.RemoveListener(CancelClick);
            _loadObsoleteButton.onClick.RemoveListener(LoadObsoleteClick);
        }

        private void UnsubscribeFromSaveCallbacks()
        {
            _editor.OnSaveResult -= SaveOrLoadResult;
            _editor.OnLoadResult -= SaveOrLoadResult;
        }
    }
}
using GameFramework;
using GameFramework.Event;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatchWindow : MonoBehaviour
{
    /// <summary>
    /// 对话框封装类
    /// </summary>
    private class MessageBox
    {
        private GameObject _cloneObject;
        private Text _content;
        private Button _btnOK;
        private System.Action _clickOK;

        public bool ActiveSelf
        {
            get
            {
                return _cloneObject.activeSelf;
            }
        }

        public void Create(GameObject cloneObject)
        {
            _cloneObject = cloneObject;
            _content = cloneObject.transform.Find("txt_content").GetComponent<Text>();
            _btnOK = cloneObject.transform.Find("btn_ok").GetComponent<Button>();
            _btnOK.onClick.AddListener(OnClickYes);
        }
        public void Show(string content, System.Action clickOK)
        {
            _content.text = content;
            _clickOK = clickOK;
            _cloneObject.SetActive(true);
            _cloneObject.transform.SetAsLastSibling();
        }
        public void Hide()
        {
            _content.text = string.Empty;
            _clickOK = null;
            _cloneObject.SetActive(false);
        }
        private void OnClickYes()
        {
            _clickOK?.Invoke();
            Hide();
        }
    }


    private IEventManager eventManager = GameFrameworkEntry.GetModule<IEventManager>();
    private readonly List<MessageBox> _msgBoxList = new List<MessageBox>();

    // UGUI相关
    private GameObject _messageBoxObj;
    private Slider _slider;
    private Text _tips;


    void Awake()
    {
        _slider = transform.Find("UIWindow/Slider").GetComponent<Slider>();
        _tips = transform.Find("UIWindow/Slider/txt_tips").GetComponent<Text>();
        _tips.text = "Initializing the game world !";
        _messageBoxObj = transform.Find("UIWindow/MessgeBox").gameObject;
        _messageBoxObj.SetActive(false);

        eventManager.Subscribe(PatchEventDefine.InitializeFailed.EventId, EventMessage);
        eventManager.Subscribe(PatchEventDefine.PatchStatesChange.EventId, EventMessage);
        eventManager.Subscribe(PatchEventDefine.FoundUpdateFiles.EventId, EventMessage);
        eventManager.Subscribe(PatchEventDefine.DownloadProgressUpdate.EventId, EventMessage);
        eventManager.Subscribe(PatchEventDefine.PackageVersionUpdateFailed.EventId, EventMessage);
        eventManager.Subscribe(PatchEventDefine.PatchManifestUpdateFailed.EventId, EventMessage);
        eventManager.Subscribe(PatchEventDefine.WebFileDownloadFailed.EventId, EventMessage);
    }
    void OnDestroy()
    {
        //_eventGroup.RemoveAllListener();
    }

    private void EventMessage(object sender, GameEventArgs args)
    {
        if (args is PatchEventDefine.InitializeFailed)
        {
            System.Action callback = () =>
            {
                UserEventDefine.UserTryInitialize.SendEventMessage(this);
            };
            ShowMessageBox($"Failed to initialize package !", callback);
        }
        else if (args is PatchEventDefine.PatchStatesChange)
        {
            var msg = args as PatchEventDefine.PatchStatesChange;
            _tips.text = msg.Tips;
        }
        else if (args is PatchEventDefine.FoundUpdateFiles)
        {
            var msg = args as PatchEventDefine.FoundUpdateFiles;
            System.Action callback = () =>
            {
                UserEventDefine.UserBeginDownloadWebFiles.SendEventMessage(this);
            };
            float sizeMB = msg.TotalSizeBytes / 1048576f;
            sizeMB = Mathf.Clamp(sizeMB, 0.1f, float.MaxValue);
            string totalSizeMB = sizeMB.ToString("f1");
            ShowMessageBox($"Found update patch files, Total count {msg.TotalCount} Total szie {totalSizeMB}MB", callback);
        }
        else if (args is PatchEventDefine.DownloadProgressUpdate)
        {
            var msg = args as PatchEventDefine.DownloadProgressUpdate;
            _slider.value = (float)msg.CurrentDownloadCount / msg.TotalDownloadCount;
            string currentSizeMB = (msg.CurrentDownloadSizeBytes / 1048576f).ToString("f1");
            string totalSizeMB = (msg.TotalDownloadSizeBytes / 1048576f).ToString("f1");
            _tips.text = $"{msg.CurrentDownloadCount}/{msg.TotalDownloadCount} {currentSizeMB}MB/{totalSizeMB}MB";
        }
        else if (args is PatchEventDefine.PackageVersionUpdateFailed)
        {
            System.Action callback = () =>
            {
                UserEventDefine.UserTryUpdatePackageVersion.SendEventMessage(this);
            };
            ShowMessageBox($"Failed to update static version, please check the network status.", callback);
        }
        else if (args is PatchEventDefine.PatchManifestUpdateFailed)
        {
            System.Action callback = () =>
            {
                UserEventDefine.UserTryUpdatePatchManifest.SendEventMessage(this);
            };
            ShowMessageBox($"Failed to update patch manifest, please check the network status.", callback);
        }
        else if (args is PatchEventDefine.WebFileDownloadFailed)
        {
            var msg = args as PatchEventDefine.WebFileDownloadFailed;
            System.Action callback = () =>
            {
                UserEventDefine.UserTryDownloadWebFiles.SendEventMessage(this);
            };
            ShowMessageBox($"Failed to download file : {msg.FileName}", callback);
        }
        else
        {
            throw new System.NotImplementedException($"{sender.GetType()}");
        }
    }

    /// <summary>
    /// 显示对话框
    /// </summary>
    private void ShowMessageBox(string content, System.Action ok)
    {
        // 尝试获取一个可用的对话框
        MessageBox msgBox = null;
        for (int i = 0; i < _msgBoxList.Count; i++)
        {
            var item = _msgBoxList[i];
            if (item.ActiveSelf == false)
            {
                msgBox = item;
                break;
            }
        }

        // 如果没有可用的对话框，则创建一个新的对话框
        if (msgBox == null)
        {
            msgBox = new MessageBox();
            var cloneObject = GameObject.Instantiate(_messageBoxObj, _messageBoxObj.transform.parent);
            msgBox.Create(cloneObject);
            _msgBoxList.Add(msgBox);
        }

        // 显示对话框
        msgBox.Show(content, ok);
    }
}
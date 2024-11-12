using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using UnityEngine.UI;
using Flower.Data;
using GameFramework.Resource;

namespace Flower
{
    public class ItemLevelSelectionButton : ItemLogicEx
    {
        public GameObject mask;
        public Image downloadProgress;
        public Text progressText;
        public Text levelTitle;
        public Text levelDescription;
        public Button button;
        public Image[] stars;
        public CanvasGroup content;

        private LevelData levelData;

        private bool frameUpdate = false;
        private bool updateResourceGroup = false;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            frameUpdate = true;
        }

        public void SetLevelData(LevelData levelData)
        {
            this.levelData = levelData;

            levelTitle.text = levelData.Name;
            levelDescription.text = levelData.Description;
            button.onClick.AddListener(OnButtonClick);

            int currentStarCount = GameEntry.Setting.GetInt(string.Format(Constant.Setting.LevelStarRecord, levelData.Id), 0);
            for (int i = 0; i < stars.Length; i++)
            {

                stars[i].gameObject.SetActive(i < currentStarCount);
            }

            SetDownFinishState();


        }


        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            button.onClick.RemoveAllListeners();
            levelTitle.text = "";
            levelDescription.text = "";
            levelData = null;
            SetDownFinishState();

            updateResourceGroup = false;
        }

        private void SetNeedDownloadState(float currentProgress = 0)
        {
            mask.SetActive(true);
            content.alpha = 0.2f;
            progressText.gameObject.SetActive(true);

            progressText.text = GameEntry.Localization.GetString("Download");

            downloadProgress.fillAmount = currentProgress;
        }

        private void SetDownFinishState()
        {
            mask.SetActive(false);
            content.alpha = 1;
            progressText.gameObject.SetActive(false);
            progressText.text = "";
            downloadProgress.fillAmount = 1;
        }

        private void OnButtonClick()
        {
            if (levelData == null)
                return;

            GameEntry.Sound.PlaySound(EnumSound.ui_sound_forward);
            GameEntry.Data.GetData<DataLevel>().LoadLevel(levelData.Id);
        }

    }
}



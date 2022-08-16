using UnityEngine;
using UnityEngine.UI;
using QFramework;
using UnityEngine.AddressableAssets;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace HotFix
{
	public partial class GameInit : UIPanel
	{
        private AudioClip micRecord;
        private string deviceName;
        private float volume;
        public GameObject player;
        public GameObject floor;
        public Button restar;
        public Button floorBtn;
        public Text value;
        public Image timeImage;
        public int time;
        private Vector2 localPos;
        private Vector2 initPos;
        private bool isOver;
        public Image localTest;

        // 定义每帧累加时间
        private float totalTimer;

        void Start()
		{
            initPos = player.transform.localPosition;
            isOver = false;
            time = 0;
            Application.RequestUserAuthorization(UserAuthorization.Microphone);
            if (Microphone.devices.Length > 0)
            {
                deviceName = Microphone.devices[0];
                micRecord = Microphone.Start(deviceName, true, 999, 44100);
            }
            else
            {
                Debug.LogError("没有麦克风");
            }
            restar.onClick.AddListener(()=> {
                Restart();
                UIKit.ClosePanel("HotFixEntry");
            });
            floorBtn.onClick.AddListener(()=> {
                player.transform.localPosition = new Vector3(localPos.x, localPos.y + 5);
                localPos = player.transform.localPosition;
            });
            var local = Addressables.LoadAssetAsync<Sprite>("fbs-45");
            local.WaitForCompletion();
            localTest.sprite = local.Result;
        }

        public void Restart()
        {
            time = 0;
            AddNumber(0);
            isOver = false;
            player.transform.localPosition = initPos;
        }

        private void Update()
        {
            if (isOver)
            {
                return;
            }
            totalTimer += Time.deltaTime;
            if (totalTimer >= 1)
            {
                float temp = GetVolume();
                value.text = ((int)temp).ToString();
                localPos = player.transform.localPosition;
                if (temp <= 1)
                {
                    player.transform.localPosition = new Vector3(localPos.x, localPos.y - 50);
                }
                else if (temp > 1 && temp < 5)
                {
                    player.transform.localPosition = new Vector3(localPos.x, localPos.y + 20);
                }
                else if (temp > 5 && temp < 20)
                {
                    player.transform.localPosition = new Vector3(localPos.x, localPos.y + 30);
                }
                else if (temp > 20 && temp < 50)
                {
                    player.transform.localPosition = new Vector3(localPos.x, localPos.y + 50);
                }
                else
                {
                    player.transform.localPosition = new Vector3(localPos.x, localPos.y + 100);
                }
                // 累加时间归0，重新累加
                totalTimer = 0;
                // 判断位置
                if (floor.transform.localPosition.y > player.transform.localPosition.y)
                {
                    Debug.LogError("Game Over");
                    UIKit.OpenPanel<GameOver>(UILevel.Common, null, "GameOver", "GameOver");
                    isOver = true;
                }
                time++;
                AddNumber(time);
            }

        }

        private void AddNumber(int time)
        {
            if (time > 99)
            {
                return;
            }
            var ten = timeImage.transform.GetChild(0);
            if (time >= 10)
            {
                var opTen = Addressables.LoadAssetAsync<Sprite>((time / 10).ToString());
                opTen.WaitForCompletion();
                ten.GetComponent<Image>().sprite = opTen.Result;
                time %= 10;
                ten.GetComponent<CanvasGroup>().alpha = 1;
            }
            else
            {
                ten.GetComponent<CanvasGroup>().alpha = 0;
            }
            var op = Addressables.LoadAssetAsync<Sprite>(time.ToString());
            op.WaitForCompletion();
            timeImage.sprite = op.Result;
        }

        /// <summary>获取麦克风音量</summary>
        /// <returns>麦克风的音量数值</returns>
        private float GetVolume()
        {
            float levelMax = 0;
            if (Microphone.IsRecording(deviceName))
            {
                float[] samples = new float[128];
                int startPosition = Microphone.GetPosition(deviceName) - (128 + 1);
                if (startPosition >= 0)
                {//当麦克风还未正式启动时，该值会为负值，AudioClip.GetData函数会报错
                    micRecord.GetData(samples, startPosition);
                    for (int i = 0; i < 128; i++)
                    {
                        float wavePeak = samples[i];
                        if (levelMax < wavePeak)
                        {
                            levelMax = wavePeak;
                        }
                    }
                    levelMax = levelMax * 99;
                    // Debug.Log("麦克风音量：" + levelMax);
                }
            }
            return levelMax;
        }

        protected override void OnClose()
        {

        }

    }
}

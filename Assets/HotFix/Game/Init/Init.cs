using UnityEngine;
using QFramework;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace HotFix
{
	public partial class Init : UIPanel
	{
        private AudioClip micRecord;
        private string deviceName;
        private float volume;
        public GameObject player;
        private Vector2 localPos;

        // 定义每帧累加时间
        private float totalTimer;

        void Start()
		{
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
        }

        private void Update()
        {
            totalTimer += Time.deltaTime;
            if (totalTimer >= 1)
            {
                float temp = GetVolume();
                localPos = player.transform.localPosition;
                if (temp < 10)
                {
                    player.transform.localPosition = new Vector3(localPos.x, localPos.y - 10);
                }
                else if (temp > 10 && temp < 50)
                {
                    player.transform.localPosition = new Vector3(localPos.x, localPos.y + 20);
                }
                else
                {
                    player.transform.localPosition = new Vector3(localPos.x, localPos.y + 30);
                }
                // 累加时间归0，重新累加
                totalTimer = 0;
            }

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetAudioValue : MonoBehaviour
{
    private AudioClip micRecord;
    private string deviceName;
    private float volume;
    private Text showValue;
    public GameObject cube;
    void Start()
    {
        deviceName = Microphone.devices[0];
        micRecord = Microphone.Start(deviceName, true, 999, 44100);
        showValue = transform.GetComponent<Text>();
    }

    void Update()
    {
        float temp = GetVolume();
        showValue.text = temp.ToString();
        cube.transform.localScale = new Vector3(temp, temp, temp);
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

}

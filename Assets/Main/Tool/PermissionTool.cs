using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif
public class PermissionTool : MonoBehaviour
{
    private string deviceName;
    private AudioClip micRecord;
    void Start()
    {
        RequestUserPermission();
    }

    public void RequestUserPermission()
    {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
        Application.RequestUserAuthorization(UserAuthorization.Microphone);
        if (Microphone.devices.Length > 0)
        {
            deviceName = Microphone.devices[0];
            micRecord = Microphone.Start(deviceName, true, 999, 44100);
            GetVolume();
        }
#endif
    }

    private float GetVolume()
    {
        float levelMax = 0;
        if (Microphone.IsRecording(deviceName))
        {
            float[] samples = new float[128];
            int startPosition = Microphone.GetPosition(deviceName) - (128 + 1);
            if (startPosition >= 0)
            {
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
                // Debug.Log("Microphone volume:" + levelMax);
            }
        }
        return levelMax;
    }
}

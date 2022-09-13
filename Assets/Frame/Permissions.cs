using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class Permissions : MonoBehaviour
{
    protected AudioClip mResultClip = null;
    protected bool mIsTiming = false;
    protected float mStartTime;
    protected readonly int mSamplesLength;

    protected string mDeviceNameMIC;
    public int LengthSec { get; set; }
    public int Frequency { get; set; }
    public void Start()
    {
        mDeviceNameMIC = null;
        LengthSec = 60;
        Frequency = 16000;
        RequestUserPermission();
    }

    public void RequestUserPermission()
    {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
        GetVolume();
#endif
    }

    public float GetVolume()
    {
        float levelMax = 0;
        if (mResultClip != null && Microphone.IsRecording(mDeviceNameMIC))
        {
            float[] samples = new float[mSamplesLength];
            int startPosition = Microphone.GetPosition(mDeviceNameMIC) - mSamplesLength + 1;
            if (startPosition >= 0)
            {
                mResultClip.GetData(samples, startPosition);
                for (int i = 0; i < mSamplesLength; i++)
                {
                    float wavePeak = samples[i];
                    if (levelMax < wavePeak)
                    {
                        levelMax = wavePeak;
                    }
                }
                levelMax = levelMax * 99;
            }
        }

        // Log.I("MicrophoneManager.GetVolume = " + levelMax);
        return levelMax;
    }


}

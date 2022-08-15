using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.Android;

public class MicrophoneManager : Singleton<MicrophoneManager>
{
    private MicrophoneManager()
    {
        mSamplesLength = 128;
        mDeviceNameMIC = null;
        LengthSec = 60;//ASR最长60秒
        Frequency = 16000;
    }

    /// <summary>录制的音频源</summary>
    protected AudioClip mResultClip = null;
    /// <summary>是否正在计时</summary>
    protected bool mIsTiming = false;
    /// <summary>开始计时（录音）的应用时间</summary>
    protected float mStartTime;
    /// <summary>音频数据长度</summary>
    protected readonly int mSamplesLength;

    /// <summary>设备名麦克风</summary>
    protected string mDeviceNameMIC;
    /// <summary>录音产生的AudioClip的长度</summary>
    public int LengthSec { get; set; }
    /// <summary>由录音产生的AudioClip的采样率</summary>
    public int Frequency { get; set; }

    public static void RequestUserPermission()
    {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
#endif
    }

    /// <summary>
    /// 开始录制
    /// </summary>
    /// <param name="isLoop">是否循环</param>
    /// <returns></returns>
    public AudioClip Start(bool isLoop = false)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        {
            End();
            mResultClip = Microphone.Start(mDeviceNameMIC, isLoop, LengthSec, Frequency);
            mIsTiming = true;
            mStartTime = Time.time;
        }
            return mResultClip;
    }

    /// <summary>结束</summary>
    public void End()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        {
            Microphone.End(mDeviceNameMIC);
            GameObject.Destroy(mResultClip);
            mResultClip = null;
            mIsTiming = false;
        }
    }

    /// <summary>获取麦克风音量</summary>
    /// <returns>麦克风的音量数值</returns>
    public float GetVolume()
    {
        float levelMax = 0;
        if (mResultClip != null && Microphone.IsRecording(mDeviceNameMIC))
        {
            float[] samples = new float[mSamplesLength];
            int startPosition = Microphone.GetPosition(mDeviceNameMIC) - mSamplesLength + 1;
            if (startPosition >= 0)
            {//当麦克风还未正式启动时，该值会为负值，AudioClip.GetData函数会报错
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

    /// <summary>计时</summary>
    public virtual void KeepTime()
    {
        if (mIsTiming)
        {
            if (Time.time - mStartTime >= LengthSec)
            {
                End();
            }
        }
    }
}

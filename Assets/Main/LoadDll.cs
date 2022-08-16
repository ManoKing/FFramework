using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
#if !UNITY_EDITOR
using UnityEngine.AddressableAssets;
#endif

/// <summary>
/// 加载热更新Dll
/// </summary>
public class LoadDll : MonoBehaviour
{
    Assembly gameAss;
    public static TextAsset[] aotDllBytes;
    public static readonly List<string> aotDlls = new List<string>()
    {
        "mscorlib.dll",
        "System.dll",
        "System.Core.dll",// 如果使用了Linq，需要这个
        // "Newtonsoft.Json.dll",
        // "protobuf-net.dll",
        // "Google.Protobuf.dll",
        // "MongoDB.Bson.dll",
        // "DOTween.Modules.dll",
        // "UniTask.dll",
    };

    void Start()
    {
        LoadGameDll();
        RunMain();
    }

    void LoadGameDll()
    {
#if !UNITY_EDITOR
        aotDllBytes = new TextAsset[aotDlls.Count];
        for (int i = 0; i < aotDlls.Count; i++)
        {
            aotDllBytes[i] = Addressables.LoadAssetAsync<TextAsset>(aotDlls[i]).WaitForCompletion();
        }
        TextAsset hotfixDll = Addressables.LoadAssetAsync<TextAsset>("HotFix.dll").WaitForCompletion();
        gameAss = Assembly.Load(hotfixDll.bytes);
#else

        gameAss = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == "HotFix");
#endif
    }

    public void RunMain()
    {
        if (gameAss == null)
        {
            Debug.LogError("dll未加载");
            return;
        }
        var appType = gameAss.GetType("HotFix.App");
        var mainMethod = appType.GetMethod("Main");
        mainMethod.Invoke(null, null);

        // 如果是Update之类的函数，推荐先转成Delegate再调用，如
        //var updateMethod = appType.GetMethod("Update");
        //var updateDel = System.Delegate.CreateDelegate(typeof(Action<float>), null, updateMethod);
        //updateDel(deltaTime);
    }
}
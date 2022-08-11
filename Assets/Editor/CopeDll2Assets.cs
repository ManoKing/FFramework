using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HybridCLR;
using System.IO;

public class CopeDll2Assets : Editor
{
    [MenuItem("Tools/复制Dll到Assets/ActiveBuildTarget")]
    static void CopeByActive()
    {
        Copy(EditorUserBuildSettings.activeBuildTarget);
    }
    [MenuItem("Tools/复制Dll到Assets/Win32")]
    static void CopeByStandaloneWindows32()
    {
        Copy(BuildTarget.StandaloneWindows);
    }
    [MenuItem("Tools/复制Dll到Assets/Win64")]
    static void CopeByStandaloneWindows64()
    {
        Copy(BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Tools/复制Dll到Assets/Android")]
    static void CopeByAndroid()
    {
        Copy(BuildTarget.Android);
    }
    [MenuItem("Tools/复制Dll到Assets/IOS")]
    static void CopeByIOS()
    {
        Copy(BuildTarget.iOS);
    }

    static void Copy(BuildTarget target)
    {
        List<string> copyDlls = new List<string>()
        {
            "HotFix.dll",
        }; 
        string outDir = BuildConfig.GetHotFixDllsOutputDirByTarget(target);
        string exportDir = Application.dataPath + "/Res/Dlls";
        if (!Directory.Exists(exportDir))
        {
            Directory.CreateDirectory(exportDir);
        }
        foreach (var copyDll in copyDlls)
        {
            File.Copy($"{outDir}/{copyDll}", $"{exportDir}/{copyDll}.bytes", true);
        }

        string aotDllDir = $"{BuildConfig.AssembliesPostIl2CppStripDir}/{target}";
        foreach (var dll in LoadDll.aotDlls)
        {
            string dllPath = $"{aotDllDir}/{dll}";
            if (!File.Exists(dllPath))
            {
                Debug.LogError($"ab中添加AOT补充元数据dll:{dllPath} 时发生错误,文件不存在。需要构建一次主包后才能生成裁剪后的AOT dll");
                continue;
            }
            string dllBytesPath = $"{exportDir}/{dll}.bytes";
            File.Copy(dllPath, dllBytesPath, true);
        }
        AssetDatabase.Refresh();
        Debug.Log("热更Dll复制成功！");
    }
}

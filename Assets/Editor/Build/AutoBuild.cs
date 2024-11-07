using HybridCLR.Editor;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AutoBuild : Editor
{
    public static string targetPath = "./build/";
    public static string packName = "FFramework.apk";

    [MenuItem("HybridCLR/Build/Build Android")]
    public static void BuildAndroid()
    {
        Build();
    }

    [MenuItem("HybridCLR/Build/Build Android Project")]
    public static void BuildAndroidProject()
    {
        packName = "FFrameworkProject";
        EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
        Build();
    }

    [MenuItem("HybridCLR/Build/Build Hot Resources")]
    public static void BuildHotResources()
    {
        AutoHybridCLR.CheckHybridCLRInstall();
        BuildAssetsCommand.BuildAndCopyABAOTHotUpdateDlls();
        //AutoAddressables.AutoUpdate();
    }


    public static void Build()
    {
        // 非播放模式
        if (EditorApplication.isPlaying)
        {
            Debug.LogError("Editor is in Play mode.");
            return;
        }

        // 检查是否安装
        AutoHybridCLR.CheckHybridCLRInstall();

        // 代码热更相关处理
        BuildAssetsCommand.BuildAndCopyABAOTHotUpdateDlls();


        // 资源相关处理
        //AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        //AddressableAssetSettings.BuildPlayerContent();


        // 获取场景列表
        List<string> levels = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled) continue;

#if UNITY_EDITOR_OSX
            string path =  Path.Combine(Application.dataPath.Replace("Assets", ""), scene.path).Replace("\\", "/");
#else
            string path = Path.Combine(Application.dataPath.Replace("Assets", ""), scene.path).Replace("/", "\\");

#endif
            if (File.Exists(path))
            {
                levels.Add(scene.path);
            }
            else
            {
                continue;
            }

        }

        // 打包
        BuildPipeline.BuildPlayer(levels.ToArray(), targetPath + packName, BuildTarget.Android,
               EditorUserBuildSettings.development == true ? BuildOptions.Development : BuildOptions.None);

    }
}

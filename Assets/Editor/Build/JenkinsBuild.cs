using HybridCLR.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class JenkinsBuild
{
    public static string targetPath = "./build/";
    public static string packName = "FFramework";

    [MenuItem("Build/Build Android")]
    public static void BuildAndroidEditor()
    {
        EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
        Build(BuildTarget.Android, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
    }

    [MenuItem("Build/Build Android Project")]
    public static void BuildAndroidProjectEditor()
    {
        packName = "FFrameworkProject";
        EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
        Build(BuildTarget.Android, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
    }

    [MenuItem("Build/Build Hot Resources")]
    public static void BuildHotResourcesEditor()
    {
        AutoHybridCLR.CheckHybridCLRInstall();
        BuildAssetsCommand.BuildAndCopyABAOTHotUpdateDlls();
        AutoYooAsset.BuildYooAsset(BuildTarget.Android, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
    }

    public static void Build(BuildTarget buildTarget, string version)
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
        AutoYooAsset.BuildYooAsset(buildTarget, version);

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
        BuildPipeline.BuildPlayer(levels.ToArray(), targetPath + packName, buildTarget,
               EditorUserBuildSettings.development == true ? BuildOptions.Development : BuildOptions.None);

    }

    public static void BuildAndroid()
    {
        // 热更只上传资源
        var isHotFix = GetCommandLineArg("isHotFix");
        Debug.Log($"isHotFix:{bool.Parse(isHotFix)}");
        var version = GetCommandLineArg("version");
        Build(BuildTarget.Android, version);
    }

    public static void BuildiOS()
    {
        var isHotFix = GetCommandLineArg("isHotFix");
        var version = GetCommandLineArg("version");
        Build(BuildTarget.iOS, version);
    }

    /// <summary>
    /// 获取Jenkins 传参的值
    /// </summary>
    /// <param name="args"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    private static string GetCommandLineArg(string key)
    {
        var args = Environment.GetCommandLineArgs().ToList();
        int index = args.IndexOf("-" + key);
        return index > 0 && index < args.Count ? args[index + 1] : "";
    }
}

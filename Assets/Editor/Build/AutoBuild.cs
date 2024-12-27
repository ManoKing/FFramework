using HybridCLR.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using YooAsset.Editor;

public class AutoBuild : Editor
{
    public static string targetPath = "./build/";
    public static string packName = "FFramework.apk";

    [MenuItem("HybridCLR/Build/Build Android")]
    public static void BuildAndroid()
    {
        EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
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
        BuildYooAsset(BuildTarget.Android, DateTime.Now.ToString("MM-dd-HH-mm"));
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
        BuildYooAsset(BuildTarget.Android, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));

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

    private static void BuildYooAsset(BuildTarget buildTarget, string buildVersion, string packageName = "DefaultPackage", EBuildinFileCopyOption copyOp = EBuildinFileCopyOption.ClearAndCopyAll)
    {
        Debug.Log($"开始构建 : {buildTarget}");

        var buildoutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot();
        var streamingAssetsRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot();

        // 构建参数
        ScriptableBuildParameters scriptableBuildParameters = new ScriptableBuildParameters();
        scriptableBuildParameters.BuildOutputRoot = buildoutputRoot;
        scriptableBuildParameters.BuildinFileRoot = streamingAssetsRoot;
        scriptableBuildParameters.BuildPipeline = EBuildPipeline.ScriptableBuildPipeline.ToString();
        scriptableBuildParameters.BuildTarget = buildTarget;
        scriptableBuildParameters.BuildMode = EBuildMode.IncrementalBuild;
        scriptableBuildParameters.PackageName = packageName;
        scriptableBuildParameters.PackageVersion = buildVersion;
        scriptableBuildParameters.VerifyBuildingResult = true;
        scriptableBuildParameters.EnableSharePackRule = true; //启用共享资源构建模式，兼容1.5x版本
        scriptableBuildParameters.BuildinFileCopyOption = copyOp;
        scriptableBuildParameters.BuildinFileCopyParams = string.Empty;
        scriptableBuildParameters.CompressOption = ECompressOption.LZ4;

        // Bundle格式
        scriptableBuildParameters.FileNameStyle = EFileNameStyle.BundleName;
        scriptableBuildParameters.EncryptionServices = null;



        // 执行构建
        ScriptableBuildPipeline pipeline = new ScriptableBuildPipeline();
        var buildResult = pipeline.Run(scriptableBuildParameters, true);
        if (buildResult.Success)
        {
            Debug.Log($"构建成功 : {buildResult.OutputPackageDirectory}");
        }
        else
        {
            Debug.LogError($"构建失败 : {buildResult.ErrorInfo}");
        }
    }
}

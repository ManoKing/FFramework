using HybridCLR.Editor.Commands;
using HybridCLR.Editor.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.Graphs;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;

namespace HybridCLR.Editor
{
    public static class BuildAssetsCommand
    {

        [MenuItem("HybridCLR/Build/BuildAssetsAndCopyToRes")]
        public static void BuildAndCopyABAOTHotUpdateDlls()
        {
            // 设置AOT
            Settings.ScriptableSingleton<HybridCLRSettings>.Instance.patchAOTAssemblies = AOTGenericReferences.PatchedAOTAssemblyList.ToArray();
            Settings.ScriptableSingleton<HybridCLRSettings>.Save();

            // Copy Dll bytes
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            CompileDllCommand.CompileDll(target);
            CopyABAOTHotUpdateDlls(target);
            AssetDatabase.Refresh();

            // 加入aa分组
            AssignAddressableGroup();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        public static void CopyABAOTHotUpdateDlls(BuildTarget target)
        {
            CopyAOTAssembliesToStreamingAssets();
            CopyHotUpdateAssembliesToStreamingAssets();
        }

        public static void CopyAOTAssembliesToStreamingAssets()
        {
            var target = EditorUserBuildSettings.activeBuildTarget;
            string aotAssembliesSrcDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);
            string aotAssembliesDstDir = Application.dataPath + "/GameRes/Dlls";

            foreach (var dll in SettingsUtil.AOTAssemblyNames)
            {
                string srcDllPath = $"{aotAssembliesSrcDir}/{dll}";
                if (!File.Exists(srcDllPath))
                {
                    Debug.LogError($"ab中添加AOT补充元数据dll:{srcDllPath} 时发生错误,文件不存在。裁剪后的AOT dll在BuildPlayer时才能生成，因此需要你先构建一次游戏App后再打包。");
                    continue;
                }
                string dllBytesPath = $"{aotAssembliesDstDir}/{dll}.bytes";
                File.Copy(srcDllPath, dllBytesPath, true);
                Debug.Log($"[CopyAOTAssembliesToStreamingAssets] copy AOT dll {srcDllPath} -> {dllBytesPath}");
            }
        }

        public static void CopyHotUpdateAssembliesToStreamingAssets()
        {
            var target = EditorUserBuildSettings.activeBuildTarget;

            string hotfixDllSrcDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
            string hotfixAssembliesDstDir = Application.dataPath + "/GameRes/Dlls";

            foreach (var dll in SettingsUtil.HotUpdateAssemblyFilesExcludePreserved)
            {
                string dllPath = $"{hotfixDllSrcDir}/{dll}";
                string dllBytesPath = $"{hotfixAssembliesDstDir}/{dll}.bytes";
                File.Copy(dllPath, dllBytesPath, true);
                Debug.Log($"[CopyHotUpdateAssembliesToStreamingAssets] copy hotfix dll {dllPath} -> {dllBytesPath}");
            }
        }

        public static void AssignAddressableGroup()
        {
            var addressableAssetSettings = AddressableAssetSettingsDefaultObject.Settings;

            // 确认Dlls分组存在，如果不存在，就创建
            var group = addressableAssetSettings.FindGroup("Dlls");
            if (group == null)
            {
                group = addressableAssetSettings.CreateGroup("Dlls", false, false, true, new List<AddressableAssetGroupSchema>(), typeof(BundledAssetGroupSchema));
            }

            // 你要添加资源的文件夹路径，如 "Assets/MyFolder"
            string myFolder = "Assets/GameRes/Dlls";

            // 获取在你的文件夹路径下所有文件的路径
            string[] allAssetPaths = Directory.GetFiles(myFolder, "*", SearchOption.AllDirectories);

            foreach (var assetPath in allAssetPaths)
            {
                // 这个过滤条件可以根据实际需要修改
                if (Path.GetExtension(assetPath) == ".meta") continue;

                var guid = AssetDatabase.AssetPathToGUID(assetPath);
                var entry = addressableAssetSettings.CreateOrMoveEntry(guid, group);
                entry.address = assetPath;

                // 将更改保存下来
                addressableAssetSettings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
            }
            SimplifyNames();
        }

        public static void SimplifyNames()
        {
            var addressableAssetSettings = AddressableAssetSettingsDefaultObject.Settings;

            // 找到'Dlls'分组
            var group = addressableAssetSettings.FindGroup("Dlls");
            if (group == null)
            {
                return;
            }

            // 对分组下所有的entry进行处理
            foreach (var entry in group.entries)
            {
                // 用资源文件的名字来作为Address，而不使用完整路径
                entry.address = Path.GetFileNameWithoutExtension(entry.AssetPath);
                entry.SetLabel("Pre", true);
            }

            // 将更改保存下来
            addressableAssetSettings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, null, true);
        }
    }
}
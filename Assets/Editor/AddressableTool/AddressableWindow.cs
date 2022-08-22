using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using System.IO;
using UnityEditor;
using UnityEngine.U2D;
using UnityEditor.U2D;
using System.Text;
using System;

public class AddressableWindow : EditorWindow
{
    // 如果文件地址修改，只需该此路径
    public const string filePath = "/Res";
    private static AddressableAssetSettings setting;
    public static Dictionary<string, List<string>> addressDic = new Dictionary<string, List<string>>();
    // 打包策略为，相同文件打包到相同包体
    public static Dictionary<string, string> nameMap = new Dictionary<string, string>()
    {
        [".prefab"] = "Prefab",
        [".spriteatlas"] = "Atals",
        [".wav"] = "Audio",
        [".shader"] = "Shader"
    };

    [MenuItem("Tools/自动标记资源地址")]
    public static void AutoMarkAddress()
    {
        setting = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>("Assets/AddressableAssetsData/AddressableAssetSettings.asset");
        Mark();
        EditorUtility.DisplayDialog("自动标记", "自动标记成功", "确定");
    }

    public static void Mark()
    {
        addressDic.Clear();
        GetFileFull(Application.dataPath + filePath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static void GetFileFull(string dirs)
    {
        DirectoryInfo dir = new DirectoryInfo(dirs);
        FileSystemInfo[] fsinfos = dir.GetFileSystemInfos();

        foreach (FileSystemInfo fsinfo in fsinfos)
        {
            //判断是否为空文件夹　　
            if (fsinfo is DirectoryInfo)
            {
                GetFileFull(fsinfo.FullName);
            }
            else
            {
                AutoMarkRootAddress(fsinfo);
            }
        }
    }

    public static void AutoMarkRootAddress(FileSystemInfo file)
    {
        string groupName = string.Empty;
        if (nameMap.ContainsKey(file.Extension))
        {
            //Debug.LogError(file);
            string address = file.Name;
            int index = address.IndexOf(".");
            address = address.Remove(index, address.Length - index);
            groupName = nameMap[file.Extension];
            string assetPath = file.FullName;

            string[] sArray = assetPath.Split(new string[] { "Assets" }, StringSplitOptions.RemoveEmptyEntries);
            assetPath = "Assets" + sArray[1];
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            //Debug.LogError(guid);
            var group = setting.FindGroup(groupName);
            if (group != null && guid != "")
            {
                var entry = setting.CreateOrMoveEntry(guid, group);
                if (entry.address != address)
                {
                    entry.SetAddress(address);
                    addAddressInfo(groupName, address);
                    List<string> oldLabels = new List<string>();
                    foreach (var item in entry.labels)
                    {
                        oldLabels.Add(item);
                    }
                    for (int i = 0; i < oldLabels.Count; i++)
                    {
                        entry.SetLabel(oldLabels[i], false);
                        setting.RemoveLabel(oldLabels[i]);
                    }
                    if (!setting.GetLabels().Contains(groupName))
                    {
                        setting.AddLabel(groupName);
                    }
                    entry.SetLabel(groupName, true);
                }
            }
        }
    }

    private static void addAddressInfo(string group, string _address)
    {
        List<string> list;
        if (addressDic.TryGetValue(group, out list))
        {
            if (!list.Contains(_address))
            {
                list.Add(_address);
            }
            else
            {
                Debug.LogError("命名重复\n在" + group + "中已经存在" + _address);
            }
        }
        else
        {
            list = new List<string>();
            list.Add(_address);
            addressDic.Add(group, list);
        }
    }
}
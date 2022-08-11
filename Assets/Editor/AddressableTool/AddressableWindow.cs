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
    private static AddressableAssetSettings setting;

    [MenuItem("BuildTools/自动标记资源地址")]
    public static void AutoMarkAddress()
    {
        setting = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>("Assets/AddressableAssetsData/AddressableAssetSettings.asset");
        Mark();
        EditorUtility.DisplayDialog("自动标记", "自动标记成功", "确定");
    }

    public static Dictionary<string, List<string>> addressDic = new Dictionary<string, List<string>>();

    public static void Mark()
    {
        addressDic.Clear();

        ///创建分组
        string loaclRoot = Application.dataPath + "/Romantory/Art";
        DirectoryInfo[] dirs = new DirectoryInfo(loaclRoot).GetDirectories();
        foreach (var info in dirs)
        {
            AutoMarkRootAddress(info);
        }

        string rolePrefab = Application.dataPath + "/RemoteRes/RolePrefab";
        DirectoryInfo[] roleDirs = new DirectoryInfo(rolePrefab).GetDirectories();
        foreach (var info in roleDirs)
        {
            AutoMarkRootAddress(info);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static void AutoMarkRootAddress(DirectoryInfo dir)
    {
        var files = GetAllFileInfo(dir);
        //var files = dir.GetFiles();
        if (files != null && files.Length > 0)
        {
            foreach (var file in files)
            {
                if (file.Extension == ".prefab")
                {
                    string address = file.Name;
                    int index = address.IndexOf(".");
                    address = address.Remove(index, address.Length - index);
                    string group_name = dir.Name;
                    string assetPath = file.FullName;

                    string[] sArray = assetPath.Split(new string[] { "Assets" }, StringSplitOptions.RemoveEmptyEntries);
                    assetPath = "Assets" + sArray[1];
                    var guid = AssetDatabase.AssetPathToGUID(assetPath);
                    //Debug.LogError(guid);
                    var group = setting.FindGroup("Prefab");
                    if (group != null && guid != "")
                    {
                        var entry = setting.CreateOrMoveEntry(guid, group);
                        if (entry.address != address)
                        {
                            entry.SetAddress(address);
                            addAddressInfo(group_name, address);
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
                            if (!setting.GetLabels().Contains(dir.Name))
                            {
                                setting.AddLabel(dir.Name);
                            }
                            entry.SetLabel(dir.Name, true);
                        }
                    }
                }
            }
        }
    }

    private static FileInfo[] GetAllFileInfo(DirectoryInfo dir)
    {
        return dir.GetFiles(".", SearchOption.AllDirectories);

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
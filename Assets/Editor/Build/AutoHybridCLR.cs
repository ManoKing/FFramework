using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using HybridCLR.Editor.Installer;
public class AutoHybridCLR 
{
    private static InstallerController _controller;
    private static InstallerController Controller
    {
        get
        {
            if (_controller == null)
                _controller = new InstallerController();
            return _controller;
        }
    }

    /// <summary>
    /// 检测CLRInstall是否已经安装并且编译，没安装就会执行安装步骤，并编译生成所有dll
    /// </summary>
    public static void CheckHybridCLRInstall()
    {
        if (!HasInstalledHybridCLR())
        {
            Debug.LogWarning("CheckHybridCLRInstall, HybridCLR未安装, 准备执行以下步骤");
            HybridCLRInstall();
            HybridCLRGeneralAll();
        }   
    }

    public static bool HasInstalledHybridCLR()
    {
        return Controller.HasInstalledHybridCLR();
    }


    /// <summary>
    /// 安装HybridCLR
    /// </summary>
    public static void HybridCLRInstall()
    {
        Debug.Log("1.HybridCLRInstall, 开始安装HybridCLR");
        Controller.InstallDefaultHybridCLR();
    }

    /// <summary>
    /// 编辑所有dll
    /// </summary>
    public static void HybridCLRGeneralAll()
    {
        Debug.Log("2.HybridCLRGeneralAll, HybridCLR生成Dll");
        HybridCLR.Editor.Commands.PrebuildCommand.GenerateAll();
    }
}

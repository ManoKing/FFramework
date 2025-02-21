using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using UnityEditor.Build.Reporting;


namespace Flower.UnityObfuscator
{
    internal class ProcessObfuscator : IPostBuildPlayerScriptDLLs
    {
        public int callbackOrder { get { return 1000; } }
        private static void DoObfuscate(string[] assemblyDllPath, string uselessCodeLibAssemblyPath, int randomSeed, bool switchNameObfuscate, bool switchCodeInject,
            ObfuscateType nameObfuscateType, ObfuscateType codeInjectObfuscateType, ObfuscateNameType obfuscateNameType,
            int garbageMethodMultiplePerClass, int insertMethodCountPerMethod)
        {
            CodeObfuscator.Obfuscate(assemblyDllPath, uselessCodeLibAssemblyPath, randomSeed, switchNameObfuscate, switchCodeInject,
                nameObfuscateType, codeInjectObfuscateType, obfuscateNameType, garbageMethodMultiplePerClass, insertMethodCountPerMethod);
        }
        private static void DoObfuscateByConfig(string[] assemblyPath)
        {
            ObfuscatorConfig obfuscatorConfig = AssetDatabase.LoadAssetAtPath<ObfuscatorConfig>(Const.ConfigAssetPath);

            if (obfuscatorConfig == null)
            {
                Debug.Log(Const.ConfigAssetPath + "不存在");
                return;
            }

            if (!obfuscatorConfig.enableCodeObfuscator)
                return;

            string uselessCodeLibAssemblyPath = obfuscatorConfig.uselessCodeLibPath;

            int randomSeed = obfuscatorConfig.randomSeed;
            if (obfuscatorConfig.useTimeSpan)
                randomSeed = (int)DateTime.Now.Ticks;

            bool enableNameObfuscate = obfuscatorConfig.enableNameObfuscate;
            bool enableCodeInject = obfuscatorConfig.enableCodeInject;
            ObfuscateType nameObfuscateType = obfuscatorConfig.nameObfuscateType;
            ObfuscateType codeInjectObfuscateType = obfuscatorConfig.codeInjectType;
            ObfuscateNameType obfuscateNameType = obfuscatorConfig.obfuscateNameType;
            int garbageMethodMultiplePerClass = obfuscatorConfig.GarbageMethodMultiplePerClass;
            int insertMethodCountPerMethod = obfuscatorConfig.InsertMethodCountPerMethod;

            DoObfuscate(assemblyPath, uselessCodeLibAssemblyPath, randomSeed, enableNameObfuscate, enableCodeInject, nameObfuscateType, codeInjectObfuscateType, obfuscateNameType, garbageMethodMultiplePerClass, insertMethodCountPerMethod);
        }
        //测试混淆（不需要Build，直接输出）
        public static void TestObfuscate()
        {
            ObfuscatorConfig obfuscatorConfig = AssetDatabase.LoadAssetAtPath<ObfuscatorConfig>(Const.ConfigAssetPath);
            var targetPath = obfuscatorConfig.testOutputPath;
            string[] obfuscateDllPaths = new string[obfuscatorConfig.obfuscateDllPaths.Length];
            obfuscatorConfig.obfuscateDllPaths.CopyTo(obfuscateDllPaths, 0);
            if (obfuscateDllPaths == null)
            {
                Debug.LogError("目标DLL路径为空");
                return;
            }
            if (!Directory.Exists(obfuscatorConfig.testOutputPath))
            {
                Directory.CreateDirectory(obfuscatorConfig.testOutputPath);
            }
            for (var i = 0; i < obfuscateDllPaths.Length; i++)
            {
                obfuscateDllPaths[i] = $"Library/ScriptAssemblies/{obfuscateDllPaths[i]}.dll";
            }


            FileInfo[] fileInfos = new FileInfo[obfuscateDllPaths.Length];

            for (int i = 0; i < obfuscateDllPaths.Length; i++)
            {
                string path = obfuscateDllPaths[i];
                //有可能记录的是相对路径，这里如果找不到对应路径就找相对路径
                path = File.Exists(path) ? path : (Application.dataPath.Substring(0, Application.dataPath.Length - 6) + path);
                bool exists = File.Exists(path);

                if (!exists)
                {
                    Debug.Log(string.Format("找不到混淆目标文件:{0}", path));
                    return;
                }
                FileInfo fileInfo = new FileInfo(path);
                string oringalMDBPath = fileInfo.FullName + ".mdb";
                string oringalPDBPath = Path.ChangeExtension(fileInfo.FullName, ".pdb");
                if (!File.Exists(oringalMDBPath) && !File.Exists(oringalPDBPath))
                {
                    Debug.Log(string.Format("找不到该MDB文件:{0}", oringalMDBPath));
                    return;
                }

                fileInfos[i] = fileInfo;
            }
            for (var i = 0; i < fileInfos.Length; i++)
            {
                obfuscateDllPaths[i] = CopyDll(fileInfos[i], targetPath);
            }
            DoObfuscateByConfig(obfuscateDllPaths);
        }
        static void CopyTo(string s, string t)
        {

            if (File.Exists(s))
            {
                if (File.Exists(t))
                    File.Delete(t);
                File.Copy(s, t);
            }
        }
        static string CopyDll(FileInfo f, string d)
        {
            string oringalDllPath = f.FullName;
            string targetDllBPath = Path.Combine(d, f.Name);
            string oringalMDBPath = oringalDllPath + ".mdb";
            string targetMDBPath = Path.Combine(d, Path.GetFileName(oringalMDBPath));
            string oringalPDBPath = Path.ChangeExtension(oringalDllPath, ".pdb");
            string targetPDBPath = Path.Combine(d, Path.GetFileName(oringalPDBPath));
            CopyTo(oringalDllPath, targetDllBPath);
            CopyTo(oringalMDBPath, targetMDBPath);
            CopyTo(oringalPDBPath, targetPDBPath);
            return targetDllBPath;
        }
        public void OnPostBuildPlayerScriptDLLs(BuildReport report)
        {
            ObfuscatorConfig obfuscatorConfig = AssetDatabase.LoadAssetAtPath<ObfuscatorConfig>(Const.ConfigAssetPath);
            CodeObfuscator.IsBuildMode = true;
            var dlls = new List<string>();
            Debug.Log(report);
            foreach (var file in report.GetFiles())
            {
                if (!file.path.EndsWith(".dll"))
                    continue;
                if (obfuscatorConfig.HasDll(file.path))
                {
                    dlls.Add(file.path);
                    Debug.Log("混淆DLL:" + file.path);
                }
            }
            DoObfuscateByConfig(dlls.ToArray());
            CodeObfuscator.IsBuildMode = false;
        }
    }

}

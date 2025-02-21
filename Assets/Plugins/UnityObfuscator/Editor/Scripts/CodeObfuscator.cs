using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using Mono.Cecil.Pdb;
using Mono.Cecil.Mdb;
using Mono.Cecil.Cil;

namespace Flower.UnityObfuscator
{
    internal static class CodeObfuscator
    {
        public static bool IsBuildMode = false;
        public static void Obfuscate(string[] assemblyPath, string uselessCodeLibAssemblyPath, int randomSeed, bool enableNameObfuscate, bool enableCodeInject,
            ObfuscateType nameObfuscateType, ObfuscateType codeInjectObfuscateType, ObfuscateNameType obfuscateNameType, int garbageMethodMultiplePerClass, int insertMethodCountPerMethod)
        {
            if (Application.isPlaying || EditorApplication.isCompiling)
            {
                if (!IsBuildMode)
                    EditorUtility.DisplayDialog("警告!", "运行和编译时不能混淆代码!", "确定");
                return;
            }

            if (assemblyPath.Length <= 0)
            {
                if (!IsBuildMode)
                    EditorUtility.DisplayDialog("警告!", "没有可混淆的Dll", "确定");
                return;
            }

            Debug.Log("Code Obfuscate Start");

            using var resolver = new DefaultAssemblyResolver();
            var set = new HashSet<string>();
            foreach (var path in assemblyPath)
            {
                var dir = Path.GetDirectoryName(path);
                if (set.Add(dir))
                    resolver.AddSearchDirectory(dir);
            }
            foreach (var item in Const.ResolverSearchDirs)
            {
                resolver.AddSearchDirectory(item);
            }

            var readerParameters = new ReaderParameters
            {
                AssemblyResolver = resolver,
                //SymbolReaderProvider = new PdbReaderProvider(),
                ReadSymbols = true,
                ReadWrite = true,
            };


            AssemblyDefinition[] assemblies = new AssemblyDefinition[assemblyPath.Length];
            for (int i = 0; i < assemblyPath.Length; i++)
            {
                var assembly = AssemblyDefinition.ReadAssembly(assemblyPath[i], readerParameters);

                if (assembly == null)
                {
                    Debug.LogError(string.Format("Code Obfuscate Load assembly failed: {0}", assemblyPath[i]));
                    return;
                }

                assemblies[i] = assembly;
            }

            AssemblyDefinition garbageCodeAssmbly = null;
            if (enableCodeInject)
            {
                garbageCodeAssmbly = AssemblyDefinition.ReadAssembly(uselessCodeLibAssemblyPath, readerParameters);

                if (garbageCodeAssmbly == null)
                {
                    Debug.LogError(string.Format("Code Obfuscate Load assembly failed: {0}", uselessCodeLibAssemblyPath));
                    return;
                }
            }

            try
            {
                //初始化组件
                ObfuscatorHelper.Init(randomSeed);
                NameObfuscate.Instance.Init(nameObfuscateType);
                CodeInject.Instance.Init(codeInjectObfuscateType, garbageMethodMultiplePerClass, insertMethodCountPerMethod);
                NameFactory.Instance.Load(obfuscateNameType);
                var garbageType = CodeInject.Instance.GetGarbageType(garbageCodeAssmbly);
                //混淆并注入垃圾代码
                for (int i = 0; i < assemblies.Length; i++)
                {
                    var module = assemblies[i].MainModule;
                    if (enableCodeInject && garbageType != null)
                        CodeInject.Instance.Obfuscate(assemblies[i], garbageType);
                    if (enableNameObfuscate)
                        NameObfuscate.Instance.Obfuscate(assemblies[i]);
                }
                //把每个dll对其他被混淆的dll的引用名字修改为混淆后的名字
                if (enableNameObfuscate)
                {
                    foreach (var assembly in assemblies)
                    {
                        foreach (var item in assembly.MainModule.GetMemberReferences())
                        {
                            try
                            {
                                if (item is FieldReference)
                                {
                                    FieldReference fieldReference = item as FieldReference;
                                    Dictionary<BaseObfuscateItem, string> dic = NameFactory.Instance.GetOld_New_NameDic(NameType.Filed);
                                    FieldObfuscateItem fieldObfuscateItem = new FieldObfuscateItem(fieldReference.DeclaringType.Namespace, fieldReference.DeclaringType.Name, fieldReference.Name);
                                    if (NameFactory.Instance.AlreadyHaveRandomName(NameType.Filed, fieldObfuscateItem))
                                    {
                                        item.Name = NameFactory.Instance.GetRandomName(NameType.Filed, fieldObfuscateItem);
                                    }
                                }
                                else if (item is PropertyReference)
                                {
                                    PropertyReference propertyReference = item as PropertyReference;

                                    PropertyObfuscateItem propertyObfuscateItem = new PropertyObfuscateItem(propertyReference.DeclaringType.Namespace, propertyReference.DeclaringType.Name, propertyReference.Name);

                                    if (NameFactory.Instance.AlreadyHaveRandomName(NameType.Property, propertyObfuscateItem))
                                    {
                                        item.Name = NameFactory.Instance.GetRandomName(NameType.Property, propertyObfuscateItem);
                                    }


                                }
                                else if (item is MethodReference)
                                {
                                    MethodReference methodReference = item as MethodReference;

                                    MethodObfuscateItem methodObfuscateItem = new MethodObfuscateItem(methodReference.DeclaringType.Namespace, methodReference.DeclaringType.Name, methodReference.Name);

                                    if (NameFactory.Instance.AlreadyHaveRandomName(NameType.Method, methodObfuscateItem))
                                    {
                                        item.Name = NameFactory.Instance.GetRandomName(NameType.Method, methodObfuscateItem);
                                    }
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }

                        foreach (var item in assembly.MainModule.GetTypeReferences())
                        {
                            try
                            {
                                TypeDefinition typeDefinition = item.Resolve();
                                TypeObfuscateItem typeObfuscateItem = ObfuscateItemFactory.Create(typeDefinition);
                                NamespaceObfuscateItem namespaceObfuscateItem = ObfuscateItemFactory.Create(typeDefinition.Namespace, typeDefinition.Module);
                                if (NameFactory.Instance.AlreadyHaveRandomName(NameType.Class, typeObfuscateItem))
                                    item.Name = NameFactory.Instance.GetRandomName(NameType.Class, typeObfuscateItem);
                                if (NameFactory.Instance.AlreadyHaveRandomName(NameType.Namespace, namespaceObfuscateItem))
                                    item.Namespace = NameFactory.Instance.GetRandomName(NameType.Namespace, namespaceObfuscateItem);
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
                var writerParameters = new WriterParameters()
                {
                    WriteSymbols = true,
                };
                for (int i = 0; i < assemblies.Length; i++)
                {
                    assemblies[i].Write(writerParameters);
                }
                if (!IsBuildMode)
                    EditorUtility.DisplayDialog("完成", "混淆代码完成!", "确定");
                else
                    Debug.Log("代码混淆完成!");

            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("Code Obfuscate failed: {0}", ex));
                if (!IsBuildMode)
                    EditorUtility.DisplayDialog("错误", $"混淆代码发生错误! \n{ex}!", "确定");
            }
            finally
            {
                for (int i = 0; i < assemblies.Length; i++)
                {
                    AssemblyDefinition assemblie = assemblies[i];
                    if (assemblie.MainModule.SymbolReader != null)
                    {
                        assemblie.MainModule.SymbolReader.Dispose();
                    }
                    assemblie.Dispose();
                    assemblies[i] = null;
                }
                if (garbageCodeAssmbly != null && garbageCodeAssmbly.MainModule.SymbolReader != null)
                {
                    garbageCodeAssmbly.MainModule.SymbolReader.Dispose();
                    garbageCodeAssmbly.Dispose();
                    garbageCodeAssmbly = null;
                }
                //输出 名字-混淆后名字 的map
                NameFactory.Instance.OutputNameMap(Const.NameMapPath);
                GC.Collect();
            }


        }

    }
}



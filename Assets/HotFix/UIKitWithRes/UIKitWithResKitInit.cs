using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace QFramework
{
    public class UIKitWithResKitInit
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            UIKit.Config.PanelLoaderPool = new ResKitPanelLoaderPool();
        }
    }
    public class ResKitPanelLoaderPool : AbstractPanelLoaderPool
    {
        public class ResKitPanelLoader : IPanelLoader
        {
            private string loadName;
            public GameObject LoadPanelPrefab(PanelSearchKeys panelSearchKeys)
            {
                if (panelSearchKeys.AssetBundleName.IsNotNullAndEmpty())
                {
                    loadName = panelSearchKeys.GameObjName;
                }

                if (panelSearchKeys.PanelType.IsNotNull() && panelSearchKeys.GameObjName.IsNullOrEmpty())
                {
                    loadName =panelSearchKeys.PanelType.Name;
                }
                //Debug.LogWarning(loadName);
                var obj = AddressableManager.Instance.LoadAsset<GameObject>(loadName);
                return obj;
            }

            public void Unload()
            {
                AddressableManager.Instance.UnloadAsset(loadName);
            }
        }

        protected override IPanelLoader CreatePanelLoader()
        {
            return new ResKitPanelLoader();
        }
    }
}
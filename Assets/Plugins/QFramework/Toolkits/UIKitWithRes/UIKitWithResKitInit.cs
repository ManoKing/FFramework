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

            public GameObject LoadPanelPrefab(PanelSearchKeys panelSearchKeys)
            {
                string loadName = string.Empty;
                if (panelSearchKeys.AssetBundleName.IsNotNullAndEmpty())
                {
                    loadName = panelSearchKeys.GameObjName;
                }

                if (panelSearchKeys.PanelType.IsNotNull() && panelSearchKeys.GameObjName.IsNullOrEmpty())
                {
                    loadName =panelSearchKeys.PanelType.Name;
                }
                Debug.LogWarning(loadName);
                var op = Addressables.LoadAssetAsync<GameObject>(loadName);
                var obj = op.WaitForCompletion();
                return obj;
            }

            public void Unload()
            {
                
            }
        }

        protected override IPanelLoader CreatePanelLoader()
        {
            return new ResKitPanelLoader();
        }
    }
}
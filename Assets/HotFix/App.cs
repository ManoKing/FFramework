using QFramework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HotFix
{
    public class App: MonoBehaviour
    {
        private void Start()
        {
            UIKitWithResKitInit.Init();
            Debug.Log("Enter the hot update interface");
            UIKit.OpenPanel<GameLobby>(UILevel.Common, null, "GameLobby", "GameLobby");
        }

        //static async void LoadScene()
        //{
        //    var handler = await Addressables.LoadSceneAsync("Main").Task;
        //    handler.ActivateAsync();
        //}
    }
}

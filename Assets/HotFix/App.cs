using QFramework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HotFix
{
    public class App
    {
        public static int Main()
        {
            LoadScene();
            // MicrophoneManager.RequestUserPermission();
            return 0;
        }

        static async void LoadScene()
        {
            var handler = await Addressables.LoadSceneAsync("Main").Task;
            handler.ActivateAsync();
            UIKit.OpenPanel<GameLobby>(UILevel.Common, null, "GameLobby", "GameLobby");
        }

       
    }
}

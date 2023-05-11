public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	// ExtensionKit.dll
	// SingletonKit.dll
	// UIKit.dll
	// Unity.Addressables.dll
	// Unity.ResourceManager.dll
	// UnityEngine.CoreModule.dll
	// mscorlib.dll
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// QFramework.Singleton<object>
	// System.Action<UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// System.Collections.Generic.Dictionary<object,UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>
	// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>
	// }}

	public void RefMethods()
	{
		// bool QFramework.CSharpObjectExtension.IsNotNull<object>(object)
		// object QFramework.UIKit.GetPanel<object>()
		// object QFramework.UIKit.OpenPanel<object>(QFramework.UILevel,QFramework.IUIData,string,string)
		// UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object> UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<object>(object)
		// object UnityEngine.Component.GetComponent<object>()
	}
}
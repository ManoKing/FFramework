using UnityEngine;

public class ExcelDataInit
{
    public static bool isInit;
    public static void Init()
    {
        if(isInit)
        {
            return;
        }
        isInit=true;
		AddressableManager.Instance.LoadSystemAsset<Localization_English>("Localization_English").SetDic();
AddressableManager.Instance.LoadSystemAsset<Localization_Russian>("Localization_Russian").SetDic();

        //Resources.UnloadUnusedAssets();
    }
}
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
		 Resources.Load<Localization_English>("DataConfig/Localization_English").SetDic();
 Resources.Load<Localization_Russian>("DataConfig/Localization_Russian").SetDic();

        Resources.UnloadUnusedAssets();
    }
}
using UnityEngine;
using UnityEngine.UI;
public class Test : MonoBehaviour, INotification
{
    // 动态多语言示例
    public Text textCom;

    private void Start()
    {
        // 动态绑定语言
        Refresh();
        LocalizationMgr.Instance.RefreshHander += Refresh;
    }

    public void Refresh()
    {
        textCom.text = LocalizationMgr.Instance.GetWordByReflection(100002);
    }
}
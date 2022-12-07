using UnityEngine;
using UnityEngine.UI;
public class TextNotification : MonoBehaviour, INotification
{
    public int textId;
    private Text textCom;

    void Start()
    {
        textCom = gameObject.GetComponent<Text>();
        Refresh();
        LocalizationMgr.Instance.RefreshHander += Refresh;
    }
    public void Refresh()
    {
        // textCom.text = LocalizationMgr.Instance.GetWordByReflection(textId);
        textCom.text = LocalizationMgr.Instance.GetWordByDirect(textId);
    }
    void OnDestroy()
    {
        LocalizationMgr.Instance.RefreshHander -= Refresh;
    }
}
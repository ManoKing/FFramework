using UnityEngine;
using UnityEngine.UI;
public class SpriteNotification : MonoBehaviour, INotification
{
    public string atlasName;
    private Image imageCom;

    void Start()
    {
        imageCom = gameObject.GetComponent<Image>();
        Refresh();
        LocalizationMgr.Instance.RefreshHander += Refresh;
    }
    public void Refresh()
    {
        var appLanguage = PlayerPrefs.GetInt("AppLanguage");
        var nameSprite = Rename(imageCom.sprite.name) + "_" +((SystemLanguage)appLanguage).ToString();
        //Debug.LogError(nameSprite);
        imageCom.SetSprite(atlasName, nameSprite);
        imageCom.SetNativeSize();
    }

    public string Rename(string name)
    {
        int index = name.LastIndexOf("_");
        return name.Substring(0, index);
    }

    void OnDestroy()
    {
        LocalizationMgr.Instance.RefreshHander -= Refresh;
    }
}
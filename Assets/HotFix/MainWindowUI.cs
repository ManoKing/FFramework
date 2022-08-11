using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class MainWindowUI : MonoBehaviour
{
    public Button LoadPrefab;

    private void Start()
    {
        LoadPrefab.onClick.AddListener(async () =>
        {
            var picture = await Addressables.LoadAssetAsync<GameObject>("Image").Task;
            var obj = Instantiate(picture, this.transform, false);
            obj.transform.position = new Vector3(300, 300, 0);
            obj.transform.GetChild(0).GetComponent<Text>().text = "Hallo Choosme";
        });
    }
}

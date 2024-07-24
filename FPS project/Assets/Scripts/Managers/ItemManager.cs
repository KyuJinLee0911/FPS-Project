using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public List<ItemData> activatedItem = new List<ItemData>();
    public GameObject itemInfoPrefab;
    public Transform itemInfoParent;

    private void LoadItemData()
    {
        var loadedJson = Resources.Load<TextAsset>("JSON/ItemDropTable");
    }
}

public class ItemDropData
{
    string name;
    int type;
    float rate;
}

public class ItemDropDatas
{
    List<ItemDropData> dropDatas;
}
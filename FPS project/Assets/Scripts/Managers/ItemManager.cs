using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public List<ItemData> activatedItem = new List<ItemData>();
    public GameObject itemInfoPrefab;
    public Transform itemInfoParent;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    private static ItemManager instance;
    public ItemData[] itemDatas;
    public GameObject itemInfoPrefab;
    public Transform itemInfoParent;

    public static ItemManager Instance
    {
        get
        {
            if(instance == null) return null;

            return instance;
        }
    }

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);
    }

    public ItemData GetItemData(string itemName)
    {
        if(itemDatas.Length == 0) return null;

        foreach(var item in itemDatas)
        {
            if(item.itemName == itemName)
                return item;
            else
                continue;
        }

        Debug.LogError("No Item Data Exist");
        return null;
    }

    public void AddItemToPlayer(ItemData itemData)
    {
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.itemList.Add(itemData);
        AddItemUI(itemData);
    }

    public void AddItemUI(ItemData itemData)
    {
        GameObject itemInfoUIObj = Instantiate(itemInfoPrefab, itemInfoParent);
        ItemInfoUI itemInfoUI = itemInfoUIObj.GetComponent<ItemInfoUI>();
        itemInfoUI.itemNameTxt.text = itemData.itemNameString;
        itemInfoUI.itemDescriptionText.text = itemData.description;
        itemInfoUI.itemImage.sprite = itemData.itemImage;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    protected ItemData itemData;

    public void Init(string itemName)
    {
        this.itemData = ItemManager.Instance.GetItemData(itemName);
    }

    public virtual void DoItem() { }

    public ItemData GetItemData()
    {
        return itemData;
    }

    private void Start()
    {
        Init(this.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        DoItem();
        ItemManager.Instance.AddItemToPlayer(itemData);
        Debug.Log($"Add {itemData.itemName} to player");
        Destroy(gameObject);
    }
}

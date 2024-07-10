using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    protected ItemData itemData;
    [SerializeField] private GameObject itemInfoWorldSpaceUI;
    public GameObject worldSpaceUI { get => itemInfoWorldSpaceUI; }
    public bool canInteract { get; set; }
    public void Init(string itemName)
    {
        this.itemData = ItemManager.Instance.GetItemData(itemName);
    }

    public virtual void DoItem() { }

    public void DoInteraction()
    {

    }

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
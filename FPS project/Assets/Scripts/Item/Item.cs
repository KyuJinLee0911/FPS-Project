using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour, IInteractable
{
    public bool isInfinite { get => false; }
    [SerializeField] protected ItemData itemData;
    public ItemData ItemData { get => itemData; }
    [SerializeField] protected GameObject itemInfoWorldSpaceUI;
    public GameObject worldSpaceUI { get => itemInfoWorldSpaceUI; }
    public bool canInteract { get; set; }
    public GameObject itemPrefab;

    public CanvasType canvasType;
    public Text itemNameTxt;
    public Text itemDescriptionText;
    public Image itemImage = null;
    public Item item;

    public virtual void SetDescription()
    {
        if (canvasType == CanvasType.CT_SCREENSPACE) return;
        Debug.Log($"{item.ItemData}");
        itemNameTxt.text = item.ItemData.itemName;
        itemDescriptionText.text = item.ItemData.itemDescription;
        if (item.ItemData.itemImage != null)
            itemImage.sprite = item.ItemData.itemImage;
    }

    public virtual void DoItem() { }

    public virtual void DoInteraction()
    {
        DoItem();
        Instantiate(itemPrefab, GameManager.Instance._item.transform);
        GameManager.Instance._item.activatedItem.Add(itemData);
        SetDescription();
        Debug.Log($"item {itemData.itemKey} activated");
        Destroy(gameObject);
    }
}
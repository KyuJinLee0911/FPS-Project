using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour, IInteractable
{
    public bool isInfinite { get => false; }
    public ItemData itemData;
    [SerializeField] protected GameObject itemInfoWorldSpaceUI;
    public GameObject worldSpaceUI { get => itemInfoWorldSpaceUI; }
    public bool canInteract { get; set; }

    public CanvasType canvasType;
    public Text itemNameTxt;
    public Text itemDescriptionText;
    public Image itemImage = null;
    public Item item;

    public virtual void SetDescription()
    {
        if (canvasType == CanvasType.CT_SCREENSPACE) return;
        Debug.Log($"{item.itemData}");
        itemNameTxt.text = item.itemData.itemName;
        itemDescriptionText.text = item.itemData.itemDescription;
        if (item.itemData.itemImage != null)
            itemImage.sprite = item.itemData.itemImage;
    }

    public virtual void DoItem() { }

    public virtual void DoInteraction()
    {
        DoItem();
        if(itemData.itemType == ItemType.IT_ITEM || itemData.itemType == ItemType.IT_WEAPON)
            GameManager.Instance._item.activatedItem.Add(this);

        transform.SetParent(transform, GameManager.Instance._item.transform);
        gameObject.SetActive(false);
    }
}
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

    private void Awake()
    {
        if (itemData.itemRarity != ItemRarity.IR_NULL)
            itemData.Weight = (int)itemData.itemRarity;
    }

    public virtual void SetDescription()
    {
        if (canvasType == CanvasType.CT_SCREENSPACE) return;
        Debug.Log($"{itemData}");
        itemNameTxt.text = itemData.itemName;
        itemDescriptionText.text = itemData.itemDescription;
        if (itemData.itemImage != null)
            itemImage.sprite = itemData.itemImage;
    }

    public virtual void DoItem() { GameManager.Instance._item.activatedItem.Add(this); }
    public virtual void RemoveItem() { }

    public virtual void DoInteraction()
    {
        DoItem();
        if (itemData.itemType == ItemType.IT_ITEM || itemData.itemType == ItemType.IT_WEAPON)
            GameManager.Instance._item.activatedItem.Add(this);

        transform.SetParent(transform, GameManager.Instance._item.transform);
        gameObject.SetActive(false);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ItemRarity
{
    IR_NORMAL,
    IR_RARE,
    IR_SUPERIOR,
    IR_EPIC,
    IR_LEGENDARY
}

public enum ItemType
{
    IT_WEAPON,
    IT_ITEM
}

[CreateAssetMenu(fileName = "Item", menuName = "Item/Make new item data", order = 2)]
public class ItemData : ScriptableObject
{
    public ItemType itemType;
    // 스크립트에서 아이템을 판별하기 위해 쓰는 string. 해당 아이템의 item을 상속한 스크립트와 동일함
    public string itemKey;
    // UI에 표시되는 이름을 위한 string.
    public string itemName;
    public ItemRarity itemRarity;
    [TextArea]
    public string itemDescription;
    public Sprite itemImage;
}

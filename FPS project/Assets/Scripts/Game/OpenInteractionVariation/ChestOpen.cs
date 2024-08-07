using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestOpen : OpenInteraction
{
    public override void DoInteraction()
    {
        base.DoInteraction();

        // 랜덤 아이템 드랍
        GameManager.Instance._item.ChestDropItem(transform);
    }
}

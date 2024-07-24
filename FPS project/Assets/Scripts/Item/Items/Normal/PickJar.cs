using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickJar : Item
{
    public override void DoItem()
    {
        base.DoItem();
        GameManager.Instance.player.hp = GameManager.Instance.player.maxHp;
    }
}

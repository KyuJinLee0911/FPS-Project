using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissB : Item
{
    public override void DoItem()
    {
        base.DoItem();
        GameManager.Instance.player.autoCriticalRate += 0.05f;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarnibalMask : Item
{
    private void Start()
    {
        GameManager.Instance.player.OnPlayerLevelUp += DoItem;
    }

    public override void DoItem()
    {
        base.DoItem();
        GameManager.Instance.player.defence += 0.04f;
    }
}

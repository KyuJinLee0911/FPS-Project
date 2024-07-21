using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleZoneOpenInteraction : OpenInteraction
{
    [SerializeField] private BattleZoneCtrl battleZoneCtrl;
    public override void DoInteraction()
    {
        base.DoInteraction();
        battleZoneCtrl.InitSpawn();
    }
}

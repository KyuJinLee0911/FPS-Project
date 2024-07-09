using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestItem : Item
{
    public override void DoItem()
    {
        base.DoItem();

        Debug.Log($"{this.name}");
    }
}

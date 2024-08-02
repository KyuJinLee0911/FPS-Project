using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : Item
{
    private void OnTriggerEnter(Collider other)
    {
        DoItem();
    }

    public override void DoInteraction()
    {
        DoItem();
    }

    public override void DoItem()
    {
        GameManager.Instance._item.AddBattery();
        Debug.Log(GameManager.Instance._item.batteries);
        transform.parent.gameObject.SetActive(false);
    }
}

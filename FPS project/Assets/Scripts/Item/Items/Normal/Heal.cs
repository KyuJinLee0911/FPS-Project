using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : Item
{
    public float healAmount = 15f;
    public override void DoItem()
    {
        GameManager.Instance.player.GetHp(healAmount);
        transform.parent.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player")) return;

        DoItem();
    }

    public override void DoInteraction()
    {
        // base.DoInteraction();
        DoItem();
    }
}

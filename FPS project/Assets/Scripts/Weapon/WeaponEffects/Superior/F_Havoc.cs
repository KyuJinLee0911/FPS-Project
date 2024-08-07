using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_Havoc : Feature
{
    private void OnEnable()
    {
        DoFeature();
    }

    private void OnDisable()
    {
        RemoveFeature();
    }

    public override void DoFeature()
    {
        GameManager.Instance.player.subSkill.skillDamage *= 1.3f;
    }

    public override void RemoveFeature()
    {
        GameManager.Instance.player.subSkill.skillDamage /= 1.3f;
    }
}

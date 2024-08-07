using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_SurpriseGift : Feature
{
    // Start is called before the first frame update
    void OnEnable()
    {
        GameManager.Instance.player.OnPlayerLevelUp += DoFeature;
        DoFeature();
    }

    private void Start()
    {

    }

    private void OnDisable()
    {
        RemoveFeature();
    }

    public override void DoFeature()
    {
        GameManager.Instance.player.autoCriticalRate += 0.1f;
    }

    public override void RemoveFeature()
    {
        GameManager.Instance.player.autoCriticalRate -= 0.1f;
        GameManager.Instance.player.OnPlayerLevelUp -= DoFeature;
    }
}

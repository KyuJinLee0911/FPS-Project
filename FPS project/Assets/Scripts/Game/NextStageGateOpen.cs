using System.Collections;
using System.Collections.Generic;
using FPS.Control;
using UnityEngine;

public class NextStageGateOpen : OpenInteraction
{
    public override void DoInteraction()
    {
        base.DoInteraction();

        
    }

    public void FinishGateOpen()
    {
        // 다음 스테이지
        GameManager.Instance.ToNextStage();
    }

    public void StopMoving()
    {
        GameManager.Instance.player.GetComponent<PlayerController>().isControlable = false;
    }
}

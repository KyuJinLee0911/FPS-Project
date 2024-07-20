using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDoor : OpenInteraction
{

    public override void DoInteraction()
    {
        GameManager.Instance.BeginPlay();
    }
}

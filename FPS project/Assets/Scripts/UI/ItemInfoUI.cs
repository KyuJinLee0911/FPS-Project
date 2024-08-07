using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CanvasType
{
    CT_WORLDSPACE,
    CT_SCREENSPACE
}

public class ItemInfoUI : MonoBehaviour
{

    private void Update()
    {
        transform.parent.LookAt(GameManager.Instance.player.transform, Vector3.up);
    }

}

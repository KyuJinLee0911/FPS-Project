using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustLocation : MonoBehaviour
{
    private void OnEnable()
    {
        transform.position = GameManager.Instance.player.transform.position;
        transform.rotation = Quaternion.identity;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendStartPosData : MonoBehaviour
{
    public Transform startPos;

    private void Awake()
    {
        Debug.Log("Data set");
        GameManager.Instance.startPos = startPos;
    }
}

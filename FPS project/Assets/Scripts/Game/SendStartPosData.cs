using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendStartPosData : MonoBehaviour
{
    public Transform startPos;
    public Transform endPos;

    private void Awake()
    {
        Debug.Log("Data set");
        GameManager.Instance.startPos = startPos;
        GameManager.Instance.endPos = endPos;
    }
}

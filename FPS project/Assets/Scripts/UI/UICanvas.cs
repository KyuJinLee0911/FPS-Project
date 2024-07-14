using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvas : MonoBehaviour
{
    [SerializeField] GameObject pauseUIObj;
    [SerializeField] GameObject loadingUIObj;

    void Init()
    {
        GameManager.Instance.pauseUIObj = pauseUIObj;
        GameManager.Instance.loadingUIObj = loadingUIObj;
    }
    void Start()
    {
        Init();
    }
}

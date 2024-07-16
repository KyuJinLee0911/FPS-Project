using System.Collections;
using System.Collections.Generic;
using FPS.Control;
using UnityEngine;

public class UICanvas : MonoBehaviour
{
    [SerializeField] GameObject pauseUIObj;
    [SerializeField] GameObject loadingUIObj;
    [SerializeField] GameObject abilityUI;

    void Init()
    {
        GameManager.Instance.pauseUIObj = pauseUIObj;
        GameManager.Instance.loadingUIObj = loadingUIObj;
        GameManager.Instance._class.abilityUI = abilityUI;
    }
    void Start()
    {
        Init();
    }
}

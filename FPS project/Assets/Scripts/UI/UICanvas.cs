using System.Collections;
using System.Collections.Generic;
using FPS.Control;
using UnityEngine;

public class UICanvas : MonoBehaviour
{
    [SerializeField] GameObject pauseUIObj;
    [SerializeField] GameObject loadingUIObj;
    [SerializeField] GameObject abilityUI;
    [SerializeField] GameObject gameOverUI;
    [SerializeField] GameObject uiLevelUpObj;

    void Init()
    {
        GameManager.Instance.pauseUIObj = pauseUIObj;
        GameManager.Instance.loadingUIObj = loadingUIObj;
        GameManager.Instance._class.abilityUI = abilityUI;
        GameManager.Instance.player.uiLevelUpText = uiLevelUpObj;
    }
    void Start()
    {
        Init();
    }
}

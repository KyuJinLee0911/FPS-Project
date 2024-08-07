using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] Button resumeBtn;
    [SerializeField] Button optionBtn;
    [SerializeField] Button restartConfirmBtn;
    [SerializeField] Button backToTitleBtn;
    [SerializeField] GameObject restartConfirmObj;

    private void OnEnable()
    {
        GameManager.Instance.AdjustTimeScale(0);
    }

    void Init()
    {
        resumeBtn.onClick.AddListener(() => GameManager.Instance.ResumeGame());
        restartConfirmBtn.onClick.AddListener(() =>
        {
            if (GameManager.Instance.gameState == GameState.GS_INGAME)
            {
                GameManager.Instance.RestartGame();
            }
            gameObject.SetActive(false);
        });
        backToTitleBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.BackToTitle();
            gameObject.SetActive(false);
        });
    }

    void Start()
    {
        Init();
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.AdjustTimeScale(1);
        restartConfirmObj.SetActive(false);
    }
}

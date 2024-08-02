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
        restartConfirmBtn.onClick.AddListener(() => GameManager.Instance.RestartGame());
        backToTitleBtn.onClick.AddListener(() => GameManager.Instance.BackToTitle());
    }

    void Start()
    {
        Init();
    }

    private void OnDisable()
    {
        GameManager.Instance.AdjustTimeScale(1);
        restartConfirmObj.SetActive(false);
    }
}

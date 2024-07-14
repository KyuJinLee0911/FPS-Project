using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] Button resumeBtn;
    [SerializeField] Button optionBtn;
    [SerializeField] Button restartConfirmBtn;
    [SerializeField] GameObject restartConfirmObj;

    void Init()
    {
        resumeBtn.onClick.AddListener(() => GameManager.Instance.ResumeGame());
        restartConfirmBtn.onClick.AddListener(() => GameManager.Instance.RestartGame());
    }

    void Start()
    {
        Init();
    }

    private void OnDisable()
    {
        restartConfirmObj.SetActive(false);
    }
}

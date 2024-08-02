using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] Button restartBtn;
    [SerializeField] Button toFortressBtn;

    private void OnEnable()
    {
        GameManager.Instance.AdjustTimeScale(0);
        GameManager.Instance.SetCursorState(true, CursorLockMode.None);

        restartBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.RestartGame();
            gameObject.SetActive(false);
        });
        toFortressBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.ToFortress();
            gameObject.SetActive(false);
        });

    }

    private void OnDisable()
    {
        GameManager.Instance.AdjustTimeScale(1);
        GameManager.Instance.SetCursorState(false, CursorLockMode.Locked);
    }
}

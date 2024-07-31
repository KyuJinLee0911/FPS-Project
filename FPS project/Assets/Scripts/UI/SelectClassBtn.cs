using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectClassBtn : MonoBehaviour
{
    [SerializeField] int idx;
    [SerializeField] GameObject ClassInfo;
    [SerializeField] GameObject ClassChange;
    private void OnEnable()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            GameManager.Instance.player.ChangePlayerClass(idx);
        });

        GetComponent<Button>().onClick.AddListener(() =>
        {
            GameManager.Instance.SetCursorState(false, CursorLockMode.Locked);
        });

        GetComponent<Button>().onClick.AddListener(() =>
        {
            ClassInfo.SetActive(false);
        });

        GetComponent<Button>().onClick.AddListener(() =>
        {
            ClassChange.SetActive(false);
        });
    }
}

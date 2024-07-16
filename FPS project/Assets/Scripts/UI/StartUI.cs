using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    public GameObject startBtnObj;
    public GameObject optionBtnObj;
    public GameObject exitBtnObj;
    public GameObject loadingUIObj;
    private Button startBtn;
    private Button optionBtn;
    private Button exitBtn;


    private void Start()
    {
        startBtn = startBtnObj.GetComponent<Button>();
        optionBtn = optionBtnObj.GetComponent<Button>();
        exitBtn = exitBtnObj.GetComponent<Button>();
        GameManager.Instance.loadingUIObj = loadingUIObj;

        startBtn.onClick.AddListener(() => GameManager.Instance.ToFortress());

        exitBtn.onClick.AddListener(() => GameManager.Instance.ExitGame());
    }
}

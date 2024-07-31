using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClassChange : MonoBehaviour
{
    [SerializeField] Button[] btns = new Button[4];
    [SerializeField] PlayerClassData[] classDatas = new PlayerClassData[4];

    private void OnEnable()
    {
        GameManager.Instance.SetCursorState(true, CursorLockMode.None);
        for (int i = 1; i < btns.Length; i++)
        {
            if (!GameManager.Instance._class.unlockedClasses.Contains(classDatas[i])) continue;

            btns[i].interactable = true;
            btns[i].transform.GetChild(0).GetComponent<Image>().color = Color.white;
            btns[i].transform.GetChild(1).GetComponent<Text>().text = classDatas[i].className;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    public Image classImg;
    public Sprite[] classSprites;
    public TextMeshProUGUI classNameTxt;
    public Transform abilityInfoParent;
    public Transform weaponInfoParent;
    public GameObject abilityImgPrefab;
    public GameObject weaponInfoPrefab;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI gradeTxt;

    public Button backToTitleBtn;
    public Button backToFortressBtn;

    private void OnEnable()
    {
        GameManager.Instance._data.SaveUserData();
        GameManager.Instance.AdjustTimeScale(0);
        if (GameManager.Instance.gameState != GameState.GS_RESULT)
            GameManager.Instance.SetGameState(GameState.GS_RESULT);
        GameManager.Instance.SetCursorState(true, CursorLockMode.None);
        SetInfos();
        backToTitleBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.BackToTitle();
            GameManager.Instance.SetCursorState(true, CursorLockMode.None);
            GameManager.Instance.AdjustTimeScale(1);
        });
        backToTitleBtn.onClick.AddListener(() => gameObject.SetActive(false));
        backToFortressBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.ToFortress();
            GameManager.Instance.SetCursorState(false, CursorLockMode.Locked);
            GameManager.Instance.AdjustTimeScale(1);
        });
        backToFortressBtn.onClick.AddListener(() => gameObject.SetActive(false));
    }

    private void OnDisable()
    {


    }

    void SetInfos()
    {
        int classIdx = (int)GameManager.Instance._class.currentClass.classType;
        classImg.sprite = classSprites[classIdx];
        classNameTxt.text = GameManager.Instance._class.currentClass.className;

        for (int i = 0; i < GameManager.Instance._class.activatedAbility.Count; i++)
        {
            GameObject newAbObj = Instantiate(abilityImgPrefab, abilityInfoParent);
            newAbObj.GetComponent<Image>().sprite = GameManager.Instance._class.activatedAbility[i].data.AbilitySprite;
        }

        for (int i = 0; i < GameManager.Instance.playerFighter.currentWeapons.Length; i++)
        {
            if (GameManager.Instance.playerFighter.currentWeapons[i] == null) continue;
            GameObject newWeaponObj = Instantiate(weaponInfoPrefab, weaponInfoParent);
            newWeaponObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GameManager.Instance.playerFighter.currentWeapons[i].itemName;
            newWeaponObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = GameManager.Instance.playerFighter.currentWeapons[i].itemDescription;
        }

        scoreText.text = GameManager.Instance.score.ToString();
        countText.text = GameManager.Instance.ingameKillCount.ToString();
        DecideGrade();
    }

    void DecideGrade()
    {
        int _score = GameManager.Instance.score;
        if (_score < 100)
        {
            gradeTxt.text = "C";
            gradeTxt.color = Color.red;
        }
        else if (_score < 200)
        {
            gradeTxt.text = "B";
            gradeTxt.color = Color.yellow;
        }
        else if (_score < 300)
        {
            gradeTxt.text = "A";
            gradeTxt.color = Color.blue;
        }
        else
        {
            gradeTxt.text = "S";
            gradeTxt.color = Color.green;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] Slider hpBar;
    [SerializeField] Slider expBar;
    [SerializeField] Text hpText;
    [SerializeField] Text expText;
    [SerializeField] Text levelText;
    [SerializeField] Image profileImage;
    [SerializeField] Text mainSkillCoolTimeTxt;
    [SerializeField] Text subSkillCoolTimeTxt;
    [SerializeField] private GameObject mainSkillCoolTimeUIObj;
    [SerializeField] private GameObject subSkillCoolTimeUIObj;
    [SerializeField] Image mainSkillImage;
    [SerializeField] Image subSkillImage;
    Player player;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        GameManager.Instance.hud = this;
        player = GameManager.Instance.player;

        player.mainSkill.skillCoolTimeUI = mainSkillCoolTimeUIObj;
        player.mainSkill.uiSkillImage = mainSkillImage;
        player.subSkill.skillCoolTimeUI = subSkillCoolTimeUIObj;
        player.subSkill.uiSkillImage = subSkillImage;
    }

    private void Update()
    {
        SetHUDData();
    }

    public void SetHUDData()
    {
        hpBar.maxValue = player.maxHp;
        hpBar.value = player.hp;
        hpText.text = $"{Mathf.FloorToInt(player.hp)} / {player.maxHp}";

        expBar.maxValue = player.expToNextLevel;
        expBar.value = player.exp;
        float percentage = Mathf.Floor(player.exp / player.expToNextLevel);
        expText.text = $"{player.exp} / {player.expToNextLevel} ({percentage}%)";

        levelText.text = $"Lv.{player.level}";

        mainSkillCoolTimeTxt.text = player.mainSkill.currentCoolTime.ToString("N1");
        subSkillCoolTimeTxt.text = player.subSkill.currentCoolTime.ToString("N1");
    }

}

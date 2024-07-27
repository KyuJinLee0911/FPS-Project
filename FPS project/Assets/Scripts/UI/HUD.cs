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
    Player player;

    private void Start()
    {
        player = GameManager.Instance.player;
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
    }

}

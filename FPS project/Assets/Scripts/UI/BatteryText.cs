using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BatteryText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    void Update()
    {
        if (GameManager.Instance.gameState != GameState.GS_FORTRESS)
            text.text = GameManager.Instance._item.gainedBatteriesInGame.ToString();
        else
            text.text = GameManager.Instance._item.batteries.ToString();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    public Ability ability;
    public Text nameText;
    public Text descriptionText;
    public Image image;
    private void OnEnable()
    {
        AddAndSetAbility();
    }

    private void AddAndSetAbility()
    {
        GetComponent<Button>().onClick.AddListener(() => ability.DoAbility());
        AbilityData data = null;
        for(int i = 0; i < ability.datas.Length; i++)
        {
            if(GameManager.Instance._class.activatedAbilityDict.ContainsKey(ability.datas[i].Key)) continue;

            data = ability.datas[i];
            break;
        }
        if(data == null) return;
        
        nameText.text = data.AbilityName;
        descriptionText.text = data.AbilityDescription;
        image.sprite = data.AbilitySprite;
    }
}

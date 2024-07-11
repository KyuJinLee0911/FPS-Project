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
        AbilityData data = ability.data;
        nameText.text = data.AbilityName;
        descriptionText.text = data.AbilityDescription;
        image.sprite = data.AbilitySprite;
    }
}

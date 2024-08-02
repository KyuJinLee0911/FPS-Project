using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    public Ability ability;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Image image;
    UnityAction call = null;
    private void OnEnable()
    {
        Debug.Log("Enable");
        AddAndSetAbility();
    }

    private void AddAndSetAbility()
    {
        call = () => ability.DoAbility();

        GetComponent<Button>().onClick.AddListener(call);
        AbilityData data = ability.data;

        if (data == null) return;

        nameText.text = data.AbilityName;
        descriptionText.text = data.AbilityDescription;
        image.sprite = data.AbilitySprite;

        GetComponent<Button>().onClick.AddListener(() =>
        {
            for (int i = 0; i < transform.parent.childCount; i++)
            {
                transform.parent.GetChild(i).gameObject.SetActive(false);
            }
            transform.parent.gameObject.SetActive(false);
        });
    }

    private void OnDisable()
    {
        GetComponent<Button>().onClick.RemoveListener(call);
        nameText.text = "";
        descriptionText.text = "";
        image.sprite = null;
        
    }
}

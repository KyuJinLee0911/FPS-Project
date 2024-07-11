using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityGrade
{
    AG_NORMAL,
    AG_SPECIAL
}

[CreateAssetMenu(fileName = "NewAbilityData", menuName = "Ability/Create new Ability data", order = 3)]
public class AbilityData : ScriptableObject
{
    [SerializeField] private AbilityGrade abilityGrade;
    [SerializeField] private string key;
    [SerializeField] private string abilityName;
    [TextArea]
    [SerializeField] private string abilityDescription;
    [SerializeField] private Sprite abilitySprite;

    public AbilityGrade AbilityGrade { get => abilityGrade; }
    public string Key { get => key; }
    public string AbilityName { get => abilityName; }
    public string AbilityDescription { get => abilityDescription; }
    public Sprite AbilitySprite { get => abilitySprite; }
}

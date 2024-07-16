using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerClassData", menuName = "Player/Create new player class data", order = 1)]
public class PlayerClassData : ScriptableObject
{
    public UnitCode unitCode;
    public ClassType classType;
    public string className;
    public float hp;
    public float defence;
    public float autoCriticalRate;
    public Skill mainSkill;
    public Skill subSkill;
    [SerializeField] private List<Ability> allAbilities;
    public List<Ability> AllAbilities { get => allAbilities; }
}

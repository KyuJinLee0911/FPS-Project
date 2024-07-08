using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    public float coolTime;
    public float skillDamage;
    public float skillRange;
    public Vector3 targetLocation;
    public string skillName;
    public abstract void Initialize();
    public abstract void DoSkill();
}

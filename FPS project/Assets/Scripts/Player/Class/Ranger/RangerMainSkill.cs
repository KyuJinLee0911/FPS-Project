using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangerMainSkill : Skill
{
    Transform root;
    [SerializeField] float stealthTime;
    [SerializeField] float decoyTime;
    [SerializeField] GameObject decoyPrefab;
    GameObject decoy;

    public override void DoSkill()
    {
        if (!IsReady()) return;
        skillCoolTimeUI.SetActive(true);
        // if(Vector3.Distance(transform.position, targetLocation) > skillRange) return;
        currentCoolTime = coolTime;
        Debug.Log(skillName);

        StartCoroutine(Stealth());
    }

    public override void Initialize()
    {
        skillName = "Stealth";
        coolTime = 16;
        currentCoolTime = 0;
        skillRange = 5;
        skillDamage = 55;
        decoy = Instantiate(decoyPrefab, transform);
        decoy.SetActive(false);
        root = transform.root;
    }
    
    IEnumerator Stealth()
    {
        GameManager.Instance.onChangeTarget.Invoke(decoy.transform);
        decoy.SetActive(true);
        decoy.transform.parent = null;
        decoy.transform.position = root.transform.position;

        yield return new WaitForSeconds(decoyTime);

        GameManager.Instance.onChangeTarget.Invoke(root);
        decoy.transform.SetParent(transform);
        decoy.SetActive(false);
    }

}

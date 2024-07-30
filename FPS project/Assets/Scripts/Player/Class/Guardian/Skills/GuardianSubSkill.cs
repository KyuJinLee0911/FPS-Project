using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianSubSkill : Skill
{
    [SerializeField] LayerMask layerMask;
    [SerializeField] float power;
    [SerializeField] GameObject rootObj;

    public override void Initialize()
    {
        // uiSkillImage.sprite = skillSprite;
        skillName = "bash";
        coolTime = 10;
        currentCoolTime = 0;
        skillRange = 5;
        skillDamage = 55;
        rootObj = transform.root.gameObject;
    }

    public override void DoSkill()
    {
        if (!IsReady()) return;
        skillCoolTimeUI.SetActive(true);
        // if(Vector3.Distance(transform.position, targetLocation) > skillRange) return;
        currentCoolTime = coolTime;
        Debug.Log(skillName);
        Bash();
    }

    private void Bash()
    {
        GameManager.Instance.controller.gunAnimator.Play("Bash");
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 3.0f, layerMask))
        {
            if (!hit.transform.gameObject.CompareTag("Enemy")) return;
            Vector3 dirVec = (hit.point - Camera.main.transform.position + Vector3.up).normalized;
            hit.collider.attachedRigidbody.AddForce(dirVec * power, ForceMode.Impulse);
            hit.collider.transform.parent.parent.GetComponent<IDamageable>().TakeDamage(rootObj, skillDamage);
        }
    }
}

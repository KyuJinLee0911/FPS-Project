using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianMainSkill : Skill
{
    [SerializeField] private GameObject bastionPrefab;
    [SerializeField] private GameObject playerBastion;
    public override void DoSkill()
    {
        if (!IsReady()) return;
        currentCoolTime = coolTime;
        skillCoolTimeUI.SetActive(true);
        Debug.Log($"{skillName}, {coolTime}, {skillRange}, {skillDamage}");
        StartCoroutine(ActivateBastion());
    }

    IEnumerator ActivateBastion()
    {
        // 피격 자체가 불가능하도록 hurtBox를 bastion이 활성화되어 있는 동안 끔
        playerBastion.SetActive(true);
        // GameManager.Instance.player.playerHurtBox.enabled = false;
        GameManager.Instance.player.hitbox.enabled = false;

        yield return new WaitForSeconds(3.0f);

        playerBastion.SetActive(false);
        // GameManager.Instance.player.playerHurtBox.enabled = true;
        GameManager.Instance.player.hitbox.enabled = true;
    }

    private void OnDestroy()
    {
        Destroy(playerBastion);
    }

    public override void Initialize()
    {
        // uiSkillImage.sprite = skillSprite;
        skillName = "Bastion";
        coolTime = 16;
        currentCoolTime = 0;
        skillRange = 3;
        skillDamage = 0;
        playerBastion = Instantiate(bastionPrefab, GameManager.Instance.player.transform);
        playerBastion.SetActive(false);
    }
}

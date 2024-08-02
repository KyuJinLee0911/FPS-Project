using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangerMainSkill : Skill
{
    Transform root;
    [SerializeField] float stealthTime;
    [SerializeField] float decoyTime;
    [SerializeField] GameObject decoyPrefab;
    [SerializeField] GameObject decoy;
    IEnumerator coroutine;

    public override void DoSkill()
    {
        if (!IsReady()) return;
        skillCoolTimeUI.SetActive(true);
        // if(Vector3.Distance(transform.position, targetLocation) > skillRange) return;
        currentCoolTime = coolTime;
        Debug.Log(skillName);

        StartCoroutine(coroutine);
    }

    public override void Initialize()
    {
        skillName = "Stealth";
        coolTime = 16;
        currentCoolTime = 0;
        skillRange = 5;
        skillDamage = 55;
        if (decoy == null)
            decoy = Instantiate(decoyPrefab, transform);
        decoy.SetActive(false);
        root = transform.root;
        coroutine = Stealth();
        GameManager.Instance.onSceneChange += OnChangeScene;
    }

    private void OnDestroy()
    {
        Destroy(decoy);
    }

    IEnumerator Stealth()
    {
        Debug.Log($"Start coroutine... {coroutine}");
        GameManager.Instance.onChangeTarget.Invoke(decoy.transform);
        decoy.SetActive(true);
        decoy.transform.parent = null;
        decoy.transform.position = root.transform.position;

        yield return new WaitForSeconds(decoyTime);

        GameManager.Instance.onChangeTarget.Invoke(root);
        decoy.transform.SetParent(transform);
        decoy.SetActive(false);
        coroutine = Stealth();
    }

    public void OnChangeScene()
    {
        if(GameManager.Instance.player == null) return;
        StopCoroutine(coroutine);
        currentCoolTime = 0;
        if (decoy.transform.parent == null)
        {
            GameManager.Instance.onChangeTarget.Invoke(root);
            decoy.transform.SetParent(transform);
            // decoy.SetActive(false);
            Initialize();
        }
        coroutine = Stealth();
    }
}

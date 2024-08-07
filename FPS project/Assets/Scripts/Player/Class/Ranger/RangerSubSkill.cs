using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RangerSubSkill : Skill
{
    Transform root;
    Fighter fighter;
    Transform lHand;
    [SerializeField] GameObject javelinPrefab;
    [SerializeField] Rigidbody javRb;
    [Header("Trajectory")]
    [SerializeField] LineRenderer line;
    [SerializeField] Transform releasePosition;
    [SerializeField][Range(10, 100)] int linePoints = 25;
    [SerializeField][Range(0.01f, 0.25f)] float timeBetweenPoints = 0.1f;
    [SerializeField] float throwPower;
    bool hasThrown = false;

    public GameObject javelin;

    public override void DoSkill()
    {
        if (!IsReady()) return;
        // 왼손 처음 위치로 복귀
        GameManager.Instance.playerFighter.ChangeLGripParent(false);
        skillCoolTimeUI.SetActive(true);
        // if(Vector3.Distance(transform.position, targetLocation) > skillRange) return;
        currentCoolTime = coolTime;
        Debug.Log(skillName);

        StartCoroutine(Javelin());
    }

    private void Update()
    {
        DrawProjection();
    }

    private void OnDestroy()
    {
        Destroy(javelin);
    }

    public override void Initialize()
    {
        skillName = "Javelin";
        coolTime = 16;
        currentCoolTime = 0;
        skillRange = 5;
        skillDamage = 55;
        lHand = GameManager.Instance.controller.leftHandTransform;
        javelin = Instantiate(javelinPrefab, lHand);

        javelin.SetActive(false);
        root = transform.root;
        fighter = root.GetComponent<Fighter>();
        releasePosition = lHand.parent.GetChild(1);
        line = releasePosition.GetComponent<LineRenderer>();
        javRb = javelin.GetComponent<Rigidbody>();
        GameManager.Instance.controller.leftHandTransform.GetComponent<LHandAnimEvent>().Init();
    }

    IEnumerator Javelin()
    {
        if (!javelin.activeSelf)
            javelin.SetActive(true);
        javRb.constraints = RigidbodyConstraints.None;
        GameManager.Instance.controller.leftHandAnimator.Play("ThrowReady");

        Fighter fighter = root.GetComponent<Fighter>();

        yield return new WaitUntil(() => fighter.isSubSkillSet == false);

        GameManager.Instance.controller.leftHandAnimator.Play("Throw");
        line.enabled = false;
    }

    void DrawProjection()
    {
        if (!IsReady()) return;
        if (!fighter.isSubSkillSet) return;

        line.enabled = true;
        line.positionCount = Mathf.CeilToInt(linePoints / timeBetweenPoints) + 1;

        Vector3 startPosition = releasePosition.position;
        Vector3 startVelocity = throwPower * releasePosition.up / javRb.mass;
        int i = 0;
        line.SetPosition(i, startPosition);
        for (float time = 0; time < linePoints; time += timeBetweenPoints)
        {
            i++;
            Vector3 point = startPosition + time * startVelocity;
            point.y = startPosition.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);

            line.SetPosition(i, point);
        }
    }
}

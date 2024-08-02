using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombGuy : Enemy
{
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator anim;
    [SerializeField] float explosionForce, explosionRadious, expUpwardModifier;
    [SerializeField] float bombGuySpeed;
    public HitBox hitBox;
    GameObject playerGameObject;


    void Start()
    {
        InitCreature();
    }

    public override void InitCreature()
    {
        playerGameObject = GameManager.Instance.player.gameObject;
        rb.AddForce(new Vector3(0, 0.5f, 0.5f), ForceMode.Impulse);
        hp = 40;
        maxHp = 40;
        defence = 0.0f;
        level = 0;
        exp = 0;
        autoCriticalRate = 0;
        autoCriticalMagnification = 0;
        isDead = false;
        isMoving = true;
        hpBar.maxValue = maxHp;
        hpBar.value = hp;

        GameManager.Instance.onChangeTarget.AddListener(ChangeTarget);
    }

    public override void Die(GameObject instigator)
    {
        if(isDead) return;
        isDead = true;
        GoBoom();
    }

    public override void TakeDamage(GameObject instigator, float damage)
    {
        base.TakeDamage(instigator, damage);
    }

    // Update is called once per frame
    void Update()
    {
        hpBar.value = hp;
        anim.SetBool("isRunning", isMoving);

        if (isMoving && playerGameObject != null)
        {
            transform.LookAt(playerGameObject.transform);
            transform.position = Vector3.MoveTowards(transform.position, playerGameObject.transform.position, bombGuySpeed * Time.deltaTime);
        }

        float distance = Vector3.Distance(transform.position, playerGameObject.transform.position);

        if (distance <= explosionRadious)
        {
            GoBoom();
        }
    }

    void GoBoom()
    {
        isMoving = false;
        anim.SetTrigger("GoBoom");
    }

    public void OnExplosionEnded()
    {
        HitBox playerHitBox = GameManager.Instance.player.hitbox;
        if(playerHitBox.enabled == false) return;

        playerGameObject.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadious, expUpwardModifier);
        if (Vector3.Distance(transform.position, playerGameObject.transform.position) < explosionRadious && hitBox.enabled == true)
        {
            playerGameObject.GetComponent<IDamageable>().TakeDamage(gameObject, 10);
        }
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleZoneCtrl : MonoBehaviour
{
    [SerializeField] private Animator enterGateAnim;
    [SerializeField] private Animator exitGateAnim;
    [SerializeField] private int enemyCount;
    [SerializeField] private int spawnedEnemyCount;
    [SerializeField] private bool hasPlayerEnteredBattleZone = false;
    [SerializeField] private Slider totalHpBar;
    [SerializeField] private GameObject hpBarObj;
    [SerializeField] private int enemySpawnAmount = 7;

    private Queue<Enemy> enemySpawnQueue = new Queue<Enemy>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        hasPlayerEnteredBattleZone = true;
        SetActiveHPBar(true);
        enterGateAnim.Play("Close");
    }

    public void InitSpawn()
    {
        for (int i = 0; i < enemySpawnAmount; i++)
        {
            SpawnNewEnemy();
        }
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        // int childCount = transform.childCount;
        int childCount = 8;
        for(int i = 0; i < childCount; i++)
        {
            Enemy enemy = transform.GetChild(i).GetComponent<Enemy>();
            EnqueueEnemy(enemy);
            CalculateTotalHp(enemy);
        }
        GameManager.Instance.battleZoneCtrl = this;
        enemyCount = enemySpawnQueue.Count;
        
        totalHpBar.value = totalHpBar.maxValue;
        SetActiveHPBar(false);
    }

    private void CalculateTotalHp(Enemy enemy)
    {
        totalHpBar.maxValue += enemy.hp;
    }

    private void EnqueueEnemy(Enemy enemy)
    {
        enemySpawnQueue.Enqueue(enemy);
        enemy.gameObject.SetActive(false);
    }

    private void SetActiveHPBar(bool isActive)
    {
        hpBarObj.SetActive(isActive);
    }

    public void SubtractEnemyCount(Enemy enemy)
    {
        enemyCount--;
        totalHpBar.value -= enemy.maxHp;
        spawnedEnemyCount--;

        if (spawnedEnemyCount <= enemySpawnAmount && enemySpawnQueue.Count > 0)
            SpawnNewEnemy();


        if (enemyCount == 0)
            OpenZoneExitGate();
    }

    void SpawnNewEnemy()
    {
        Enemy newEnemy = enemySpawnQueue.Dequeue();
        newEnemy.gameObject.SetActive(true);
        spawnedEnemyCount++;
    }

    void OpenZoneExitGate()
    {
        exitGateAnim.Play("Open");
        SetActiveHPBar(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Spawner : MonoBehaviour
{
    public int count;
    public float spawnDistance;
    public float scaleFactor;
    public List<GameObject> monsters;
    public BoxCollider spawnBox;

    private void Start()
    {
        StartCoroutine(SpawnMonsters());
    }
    IEnumerator SpawnMonsters()
    {
        float distance;
        List<float> weights = new List<float>();
        foreach (var mob in monsters)
        {
            Enemy enemy = mob.GetComponent<Enemy>();
            weights.Add((float)enemy.enemyType);
        }
        List<GameObject> spawnedMonsters = new List<GameObject>();
        for (int i = 0; i < count; i++)
        {
            float posX = Random.Range(transform.position.x - spawnBox.size.x / 2, transform.position.x + spawnBox.size.x / 2);
            float posZ = Random.Range(transform.position.z - spawnBox.size.z / 2, transform.position.z + spawnBox.size.z / 2);

            int idx = GameManager.Instance._item.PickRandomIndex(weights);
            GameObject newEnemy = Instantiate(monsters[idx], transform);
            newEnemy.transform.position = new Vector3(posX, transform.position.y, posZ);
            newEnemy.transform.localScale = Vector3.one * scaleFactor;
            spawnedMonsters.Add(newEnemy);
            newEnemy.SetActive(false);
        }
        yield return new WaitUntil(() =>
        {
            distance = Vector3.Distance(GameManager.Instance.player.transform.position, transform.position);
            Debug.Log(distance);
            return distance <= spawnDistance;
        });

        for(int i = 0; i < count; i++)
        {
            spawnedMonsters[i].SetActive(true);
        }
    }
}

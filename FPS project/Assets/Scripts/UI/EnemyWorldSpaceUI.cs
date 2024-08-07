using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWorldSpaceUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(GameManager.Instance.player.transform.position);
    }
}

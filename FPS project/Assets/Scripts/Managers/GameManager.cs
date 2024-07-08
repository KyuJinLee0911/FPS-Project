using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public PlayerClassData[] playerClassDatas;

    void Awake()
    {
        if(instance == null) instance = this;
        else
        {
            if(instance != this)
                Destroy(gameObject);
        }
    }

    void Start()
    {
        

    }
    public static GameManager Instance
    {
        get
        {
            if(instance == null)
            {
                return null;
            }
            return instance;
        }
    }

}

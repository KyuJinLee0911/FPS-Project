using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }

    public ClassManager _class;
    public DataManager _data;
    public ItemManager _item;
    public ObjectPool _pool;
    public Player player;

    public void Init()
    {
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;
        _class.Init();
        _data.Init();
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.Initialize();
    }

    void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            if (instance != this)
                Destroy(gameObject);
        }
    }

    void Start()
    {
        Init();   
    }
    
}

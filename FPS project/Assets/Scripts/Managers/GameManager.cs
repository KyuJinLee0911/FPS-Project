using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using FPS.Control;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public GameObject pauseUIObj;
    public GameObject loadingUIObj;

    public void Init()
    {
        SetCursorState(false, CursorLockMode.Locked);
        _class.Init();
        _data.Init();
    }

    void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            if (instance != this)
                Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Init();   
    }

    public void AdjustTimeScale(float scale)
    {
        Time.timeScale = scale;
    }

    public void PauseGame()
    {
        player.GetComponent<PlayerController>().isControlable = false;
        pauseUIObj.SetActive(true);
        AdjustTimeScale(0);
        SetCursorState(true, CursorLockMode.None);
    }

    public void ResumeGame()
    {
        player.GetComponent<PlayerController>().isControlable = true;
        pauseUIObj.SetActive(false);
        AdjustTimeScale(1);
        SetCursorState(false, CursorLockMode.Locked);
    }

    public void SetCursorState(bool isCursorVisible, CursorLockMode lockMode)
    {
        Cursor.visible = isCursorVisible;
        Cursor.lockState = lockMode;
    }

    public void RestartGame()
    {
        AdjustTimeScale(1);
        _data.DeleteData();
        StartCoroutine(LoadSceneAsyncWithLoadingUI("TestScene"));
    }

    void OnPause()
    {
        if(pauseUIObj.activeInHierarchy)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    IEnumerator LoadSceneAsyncWithLoadingUI(string sceneName)
    {
        loadingUIObj.SetActive(true);
        Slider loadingGauge = loadingUIObj.transform.GetComponentInChildren<Slider>();
        Text loadingText = loadingUIObj.transform.GetComponentInChildren<Text>();

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);

        while(!async.isDone)
        {
            // 로딩 진행 상태 확인 코드
            loadingGauge.value = async.progress;
            loadingText.text = $"Loading... ({(async.progress * 100).ToString("N1")}%)";
            
            yield return null;
        }
        
        Init();
        loadingUIObj.SetActive(false);
        
    }
    
}

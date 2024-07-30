using System;
using System.Collections;
using FPS.Control;
using UnityEngine;
using UnityEngine.Events;
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
    public AchievementManager _achivement;
    public Player player;
    public Fighter playerFighter;
    public PlayerController controller;
    public GameObject pauseUIObj;
    public GameObject loadingUIObj;
    public GameObject playerPrefab;
    public string[] stageNames;
    public int currentStageIndex = 0;
    public Transform startPos;
    public Transform endPos;
    public BattleZoneCtrl battleZoneCtrl;
    public UnityEvent<Transform> onChangeTarget;

    public void Init()
    {

        _class.Init();
        _data.Init();
        _achivement.Init();
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

        Init();
    }

    void Start()
    {

    }

    #region About Pause and Resume
    public void AdjustTimeScale(float scale)
    {
        Time.timeScale = scale;
    }

    public void PauseGame()
    {
        controller.isControlable = false;
        pauseUIObj.SetActive(true);
        AdjustTimeScale(0);
        SetCursorState(true, CursorLockMode.None);
    }

    public void ResumeGame()
    {
        controller.isControlable = true;
        pauseUIObj.SetActive(false);
        AdjustTimeScale(1);
        SetCursorState(false, CursorLockMode.Locked);
    }

    void OnPause()
    {
        if (pauseUIObj == null) return;
        if (pauseUIObj.activeInHierarchy)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    #endregion

    public void SetCursorState(bool isCursorVisible, CursorLockMode lockMode)
    {
        Cursor.visible = isCursorVisible;
        Cursor.lockState = lockMode;
    }

    public void RestartGame()
    {
        AdjustTimeScale(1);
        _data.DeleteIngameData();
        currentStageIndex = 1;
        StartCoroutine(LoadSceneAsyncWithLoadingUI(stageNames[1]));
    }

    #region Change Stages
    IEnumerator LoadSceneAsyncWithLoadingUI(string sceneName)
    {
        loadingUIObj.SetActive(true);
        Slider loadingGauge = loadingUIObj.transform.GetComponentInChildren<Slider>();
        Text loadingText = loadingUIObj.transform.GetComponentInChildren<Text>();

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);

        while (!async.isDone)
        {
            // 로딩 진행 상태 확인 코드
            loadingGauge.value = async.progress;
            loadingText.text = $"Loading... ({(async.progress * 100).ToString("N1")}%)";

            yield return null;
        }

        Init();
        loadingUIObj.SetActive(false);
    }

    // 요새(거점)으로 가는 함수
    // 커서 끄고, 요새 스테이지 불러오기
    public void ToFortress()
    {
        SetCursorState(false, CursorLockMode.Locked);
        // currentStageIndex--;
        // if (currentStageIndex < 0)
        //     currentStageIndex = stageNames.Length - 1;
        if (currentStageIndex == 3)
        {
            _data.DeleteIngameData();
            player.level = 1;
            player.Initialize();
        }
        currentStageIndex = 4;

        StartCoroutine(LoadSceneAsyncWithLoadingUI(stageNames[currentStageIndex]));
    }

    // 요새에서 스테이지 1로 가는 함수
    // 요새에서 문과 상호작용하면 호출
    public void BeginPlay()
    {
        ToNextStage();
    }

    // 0 = 시작화면
    // 1 = 스테이지 1
    // 2 = 스테이지 2
    // 3 = 스테이지 3 (보스)
    // 4 = 거점
    // 거점에서 스테이지 1, 스테이지 1에서 2, 2에서 3, 3에서 거점으로 복귀하는 함수
    public void ToNextStage()
    {
        // 이미 움직일 수 없음 = 다음 스테이지로 이미 이동 중
        // 중복 입력을 통해 스테이지를 건너뛰는 것을 막기 위함
        if (!controller.isControlable)
        {
            Debug.Log("Already moving");
            return;
        }

        // 다음스테이지로 가기 전에 현재 데이터 저장
        controller.isControlable = false;
        _data.SaveIngameData();

        currentStageIndex++;

        if (currentStageIndex >= stageNames.Length)
        {
            currentStageIndex = 1;
        }
        StartCoroutine(LoadSceneAsyncWithLoadingUI(stageNames[currentStageIndex]));

        // 컷씬 재생
        StartCoroutine(PlayCutScene());
    }
    #endregion

    IEnumerator PlayCutScene()
    {
        controller.isControlable = false;
        Debug.Log("Cut Scene Playing...");
        yield return new WaitForSeconds(2);
        controller.isControlable = true;
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

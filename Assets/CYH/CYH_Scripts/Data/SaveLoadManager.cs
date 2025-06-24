using System;
using System.Collections;
using System.IO;
using UnityEngine;

/// <summary>
/// JSON 형식으로 게임 데이터를 저장하고 불러오는 매니저 클래스입니다.
/// </summary>
public class SaveLoadManager : Singleton<SaveLoadManager>
{
    public GameData GameData;
    public const string FileName = "SaveFile";
    public string DataPath => Path.Combine(Application.dataPath, $"CYH/CYH_SaveFiles/{FileName}");

    [field: SerializeField] public int ElapsedSeconds { get; private set; } // 게임종료 후 총 경과 시간(초) - 테스트용

    [Header("총 경과 시간")]
    [SerializeField] private int _elapsedMinutes;   // 게임종료 후 총 경과 시간(분)
    public int ElapsedMinutes { get { return _elapsedMinutes; } }

    private Coroutine _autoSaveCoroutine;
    [Header("저장 간격")]
    [SerializeField] private float autoSaveInterval = 1f;
    private WaitForSeconds _wait;

    [Header("일반 스테이지")]
    // 일반스테이지 판별용 테스트 변수
    public bool isNormalStage = false;


    protected override void Awake()
    {
        base.Awake();

        // 게임 시작 시 저장된 모든 게임데이터 불러오기
        LoadData();
    }

    private void Start()
    {
        _wait = new WaitForSeconds(autoSaveInterval);
        
        // 일반스테이지 -> 1초마다 게임데이터 저장
        if (isNormalStage)
        {
            StartAutoSave();
        }
    }

    private void Update()
    {
        // 테스트용
        if (Input.GetKeyDown(KeyCode.M))
        {
            SaveData();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            LoadData();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            DeleteData();
        }
    }

    #region SaveCoroutine
    // 1초마다 SaveData 호출
    private IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            SaveData();
            yield return _wait;
        }
    }

    public void StartAutoSave()
    {
        if (_autoSaveCoroutine == null)
            _autoSaveCoroutine = StartCoroutine(AutoSaveRoutine());
    }

    public void StopAutoSave()
    {
        if (_autoSaveCoroutine != null)
        {
            StopCoroutine(_autoSaveCoroutine);
            _autoSaveCoroutine = null;
        }
    }
    #endregion

    #region Save, Load, Delete Exist
    /// <summary>
    /// 지정된 경로에 데이터를 JSON 형식으로 저장합니다.
    /// </summary>
    public void SaveData()
    {
        // 저장 경로 유무 체크
        if (Directory.Exists(Path.GetDirectoryName($"{DataPath}")) == false)
        {
            Directory.CreateDirectory(Path.GetDirectoryName($"{DataPath}"));
        }

        GameData.SavedTime = DateTime.Now;  // 저장 시간 = 현재 시간

        string json = JsonUtility.ToJson(GameData, true);
        File.WriteAllText($"{DataPath}", json);
        Debug.Log("SaveData");
        Debug.Log($"마지막 저장 시간: {GameData.SavedTime}");
    }

    /// <summary>
    /// 지정된 경로에서 JSON 형식의 데이터를 불러와 객체로 반환합니다.
    /// 최종 게임에서는 Awake에서 호출될 예정입니다.
    /// </summary>
    public bool LoadData()
    {
        // 데이터 존재 여부 체크
        if (!ExistData())
            return false;

        string json = File.ReadAllText($"{DataPath}");
        GameData = JsonUtility.FromJson<GameData>(json);
        Debug.Log("LoadData");

        // 테스트용
        ElapsedTime();
        _elapsedMinutes = 0;
        ElapsedSeconds = 0;

        return true;
    }

    /// <summary>
    /// 지정된 경로에 있는 저장 파일을 삭제합니다.
    /// </summary>
    public bool DeleteData()
    {
        // 초기화용 Json파일 백업 상황도 고려

        // 데이터 존재 여부 체크
        if (!ExistData())
            return false;

        // 파일 삭제
        File.Delete($"{DataPath}");
        Debug.Log("DeleteData");

        return true;
    }

    /// <summary>
    /// 지정된 경로에 저장 파일이 존재하는지 확인합니다.
    /// </summary>
    public bool ExistData()
    {
        // 저장 경로 유무 체크
        if (Directory.Exists(Path.GetDirectoryName($"{DataPath}")) == false)
            return false;

        return File.Exists($"{DataPath}");
    }

    /// <summary>
    /// 저장된 시간과 현재 시간의 차이를 계산해 경과 시간을 분 단위로 저장합니다.
    /// 현재는 테스트 용도로 초 단위를 기준으로 사용합니다.
    /// </summary>
    public void ElapsedTime()
    {
        DateTime gameStartTime = DateTime.Now;
        TimeSpan elapsedTime = gameStartTime - GameData.SavedTime;

        // 초 단위는 버리고 분 단위만 추출
        //_elapsedMinutes = (int)elapsedTime.TotalMinutes;
        //Debug.Log($"게임 시작 시간: {gameStartTime}");
        //Debug.Log($"게임 종료 후 경과 시간: {ElapsedMinutes}분");

        // 테스트용 초 단위 체크
        ElapsedSeconds = (int)elapsedTime.TotalSeconds;
        Debug.Log($"게임 시작 시간: {gameStartTime}");
        Debug.Log($"게임 종료 후 경과 시간: {ElapsedSeconds}초");
    }
    #endregion

    private void OnEnable()
    {
        GameEvents.OnSaveRequested += SaveData;
    }

    private void OnDisable()
    {
        GameEvents.OnSaveRequested -= SaveData;
    }
}
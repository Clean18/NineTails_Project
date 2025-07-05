using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : Singleton<SceneChangeManager>
{
    private AsyncOperation _asyncLoad;

    private float _loadStartTime;                           // 로딩 시작 시각 
    private float _minLoadingTime;                          // 최소 로드 시간
    private string _targetSceneName;                        // 로드할 씬
    // 현재 씬 번호
    [SerializeField] private int _currentSceneIndex;
    public int CurrentSceneIndex
    {
        get => _currentSceneIndex;
        set
        {
            //Debug.Log($"현재 씬 인덱스{_currentSceneIndex} = {value}");
            _currentSceneIndex = Mathf.Clamp(value, 2, _stageInfo.Count);
        }
    }
    [SerializeField] private int _nextSceneIndex;
    public int NextSceneIndex
    {
        get => _nextSceneIndex;
        set
        {
            _nextSceneIndex = Mathf.Clamp(value, 3, _stageInfo.Count); ;
        }
    }
    
    /// <summary>
    /// 스테이지 리스트, 딕셔너리 Key값
    /// </summary>
    [SerializeField] public List<string> _stageInfo;
    /// <summary>
    /// 딕셔너리 Value값
    /// </summary>
    [SerializeField] public List<string> GameSceneValues;
    // 스테이지 정보 딕셔너리
    private Dictionary<string, string> _gameSceneDict;

    /// <summary>
    /// 현재 씬 정보가 스토리 씬의 몇번째 index인지 반환하는 함수
    /// <br/> Key가 있으면 플레이어 오브젝트 비활성화, 없으면 활성화
    /// <br/> ex) Table[SceneName] == DataParser Index
    /// </summary>
    private Dictionary<string, int> _dialogSceneDic;

    protected override void Awake()
    {
        base.Awake();

        _gameSceneDict = LoadCsvToDictionary("Csvs/GameScenes");
        _stageInfo = LoadCsvToList("Csvs/StageInfo");
        GameSceneValues = _gameSceneDict.Values.ToList();
    }

    private void Start()
    {
        // GameStart 씬 로드
        //LoadSceneAsync(_gameSceneDict[_stageInfo[0]]);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log($"입력한 키 : {Input.inputString}");
            SceneChangeManager.Instance.LoadSceneAsync("DialogScene");
            //SceneChangeManager.Instance.LoadSceneAsync("Stage1-3");
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log($"입력한 키 : {Input.inputString}");
            //SceneChangeManager.Instance.LoadSceneAsync("Stage1-1_CYH_Prototype");
            SceneChangeManager.Instance.LoadSceneAsync("Stage1-3");
        }
    }

    /// <summary>
    /// 현재 씬의 다음 씬 로드 (SceneChangeManager.Instance.LoadNextScene())
    /// </summary>
    public void LoadNextScene()
    {
        // 다음 인덱스 계산
        int nextIndex = PlayerController.Instance.GetPlayerSceneIndex() + 1;

        // 유효 범위 체크
        if (_stageInfo != null && nextIndex < _stageInfo.Count)
        {
            // 씬 정보로 씬 이름 가져오기
            string nextScene = _gameSceneDict[_stageInfo[nextIndex]];
            PlayerController.Instance.SetPlayerSceneIndex(nextIndex);

            LoadSceneAsync(nextScene); // 사용중
        }
        else
        {
            Debug.LogWarning("더 이상 로드할 씬 없음");
        }
    }

    public void LoadPrevScene()
    {
        // 다음 인덱스 계산
        int nextIndex = PlayerController.Instance.GetPlayerSceneIndex() - 1;

        // 유효 범위 체크
        if (_stageInfo != null && nextIndex < _stageInfo.Count)
        {
            // 씬 정보로 씬 이름 가져오기
            string nextScene = _gameSceneDict[_stageInfo[nextIndex]];
            PlayerController.Instance.SetPlayerSceneIndex(nextIndex);

            LoadSceneAsync(nextScene); // 사용중
        }
        else
        {
            Debug.LogWarning("더 이상 로드할 씬 없음");
        }
    }

    /// <summary>
    /// 다른 씬 로드 (스테이지 정보 ex.Stage1-1_Start)
    /// </summary>
    public void LoadNextScene(string stageInfo)
    {
        // stageInfo 씬 로드
        LoadSceneAsync(_gameSceneDict[stageInfo]);
        // 현재 씬 번호 -> 이동할 씬 번호로 변경                     
        CurrentSceneIndex = _stageInfo.IndexOf(stageInfo);
    }

    /// <summary>
    /// 씬을 비동기로 로드 (일반씬 -> 일반씬)
    /// </summary>
    public void LoadSceneAsync(string sceneName)
    {
        // 현재 로딩 중인 씬이 있는지 체크
        if (_asyncLoad != null && !_asyncLoad.isDone)
            return;

        StartCoroutine(LoadSceneCoroutine(sceneName)); // 사용중
    }

    /// <summary>
    /// 씬을 비동기로 로드 (일반씬 -> 로딩씬 -> 목표씬)
    /// </summary>
    public void LoadSceneWithLoading(string loadingSceneName, string targetSceneName)
    {
        // 현재 로딩 중인 씬이 있는지 체크
        if (_asyncLoad != null && !_asyncLoad.isDone) return;

        _targetSceneName = targetSceneName;
        StartCoroutine(LoadTwoSceneCoroutine(loadingSceneName));
    }

    /// <summary>
    /// 씬을 비동기로 로드 (일반씬 -> 로딩씬 -> 목표씬) (최소 로딩시간 보장)
    /// </summary>
    public void LoadSceneWithLoading(string loadingSceneName, string targetSceneName, float minLoadingTime)
    {
        // 현재 로딩 중인 씬이 있는지 체크
        if (_asyncLoad != null && !_asyncLoad.isDone) return;

        _targetSceneName = targetSceneName;
        _minLoadingTime = minLoadingTime;
        StartCoroutine(LoadTwoSceneCoroutineWithMinTime(loadingSceneName));
    }

    public void LoadFirstScene()
    {
        StartCoroutine(LoadFirstSceneRoutine());
    }

    /// <summary>
    /// 현재 씬의 다음 씬 로드 (스테이지 인덱스(=순서))
    /// </summary>
    public void LoadNextScene(int sceneIndex)
    {
        // 다음 인덱스 계산
        if (sceneIndex <= 2) sceneIndex = 2;
        int nextIndex = sceneIndex + 1;

        // 유효 범위 체크
        if (_stageInfo != null && nextIndex < _stageInfo.Count)
        {
            // 씬 정보로 씬 이름 가져오기
            string nextScene = _gameSceneDict[_stageInfo[sceneIndex]];
            if (nextScene == "GameStart" || nextScene == "Loading" || nextScene == "GameStartScene" || nextScene == "LoadingScene_v1")
            {
                Debug.Log("게임시작씬 or 로딩씬 index라 프롤로그 씬으로 변경");
                sceneIndex = 2;
                nextIndex = 3;
                nextScene = _gameSceneDict[_stageInfo[sceneIndex]];
            }
            Debug.Log($"현재 씬 : {sceneIndex}");
            CurrentSceneIndex = sceneIndex;
            LoadSceneAsync(nextScene);
            NextSceneIndex = nextIndex;
        }
        else
        {
            Debug.LogWarning("더 이상 로드할 씬 없음");
        }
    }

    #region Coroutine

    /// <summary>
    /// 데이터매니저 초기화 > 플레이어 초기화 > 씬이동 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadFirstSceneRoutine()
    {
        // TODO : 일단은 로딩씬으로 이동 후
        Debug.LogWarning("게임 시작 로딩씬으로 이동");

        yield return StartCoroutine(LoadSceneCoroutine(_gameSceneDict["Loading"]));

        // 데이터매니저 csv 파싱 후
        if (!DataManager.IsDataInit)
        {
            // 데이터 매니저 초기화
            Debug.LogWarning("데이터매니저 초기화 중...");
            yield return StartCoroutine(DataManager.Instance.LoadDatas());
            Debug.LogWarning("데이터매니저 초기화 완료");
        }

        // 플레이어 초기화 후 비활성화
        GameManager.Instance.PlayerInit();

        Debug.LogWarning("플레이어 초기화 중...");
        yield return StartCoroutine(PlayerController.Instance.PlayerInitRoutine());
        Debug.LogWarning("플레이어 초기화 완료");

        // 인덱스로 씬 변경
        LoadNextScene(PlayerController.Instance.GetPlayerSceneIndex());
    }

    /// <summary>
    /// 일반 -> 로딩씬 -> 목표씬 순차 코루틴
    /// </summary>
    private IEnumerator LoadTwoSceneCoroutine(string loadingSceneName)
    {
        // 로딩씬 바로 로드
        yield return StartCoroutine(LoadSceneCoroutine(loadingSceneName));
        // 목표씬 바로 로드
        yield return StartCoroutine(LoadSceneCoroutine(_targetSceneName));
    }

    /// <summary>
    /// 일반 -> 로딩씬 -> 목표씬 순차 코루틴 (최소 로드시간 보장)
    /// </summary>
    private IEnumerator LoadTwoSceneCoroutineWithMinTime(string loadingSceneName)
    {
        // 로딩씬 바로 로드
        yield return StartCoroutine(LoadSceneCoroutine(loadingSceneName)); // LoadingScene
        // 목표씬은 최소 로드시간 보장 후 로드
        yield return StartCoroutine(LoadingLoadSceneCoroutine(_targetSceneName, _minLoadingTime));
    }

    /// <summary>
    /// 씬 로딩 코루틴
    /// </summary>
    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // 로딩 시작 시작 체크
        _loadStartTime = Time.time;
        _asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        //// 씬 로딩 완료되었을 때 바로 로드
        //_asyncLoad.allowSceneActivation = true;

        // 로딩 진행 중
        while (!_asyncLoad.isDone)
        {
            // 진행도(%)
            float loadProgress = Mathf.Clamp01(_asyncLoad.progress / 0.9f);
            float progressPercent = loadProgress * 100f;
            Debug.Log($"로딩 진행도 : {progressPercent:F0}%");
            yield return null;
        }

        //TODO: DialogScene인 경우 Player 초기화 스킵 처리

        // 로딩에 걸린 시간
        float elapsed = Time.time - _loadStartTime;
        Debug.Log($"[{sceneName}] 로딩 완료: {elapsed:F1}초 소요");

        // 씬 로딩 완료되었을 때 바로 로드
        _asyncLoad.allowSceneActivation = true;
    }

    /// <summary>
    /// 씬 로딩 코루틴 (최소 로드시간 보장)
    /// </summary>
    private IEnumerator LoadingLoadSceneCoroutine(string sceneName, float minTime)
    {
        _loadStartTime = Time.time;
        _asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // 씬 로딩 완료되었을 때 대기
        _asyncLoad.allowSceneActivation = false;

        // 로딩 진행도가 90% 미만 || 경과시간이 최소보장시간 미만일 때 대기
        while (_asyncLoad.progress < 0.9f || Time.time - _loadStartTime < minTime)
        {
            float loadProgress = Mathf.Clamp01(_asyncLoad.progress / 0.9f);
            float progressPercent = loadProgress * 100f;
            float elapsed = Time.time - _loadStartTime;
            //Debug.Log($"[최소 로드 시간 보장] [{sceneName}] 로딩 진행도: {progressPercent:F0}%, 소요 시간: {elapsed:F1}/{minTime:F1}초");
            yield return null;
        }

        //TODO: DialogScene인 경우 Player 초기화 스킵 처리

        Debug.Log($"[{sceneName}] 로딩 완료(최소 {minTime:F1}초 보장) 총 소요 시간: {(Time.time - _loadStartTime):F1}초");
        _asyncLoad.allowSceneActivation = true;
        while (!_asyncLoad.isDone) yield return null;
    }
    #endregion

    #region LoadCsv

    /// <summary>
    /// Resources 폴더의 TextAsset -> Dictionary 로드
    /// </summary>
    private Dictionary<string, string> LoadCsvToDictionary(string resourcePath)
    {
        var dict = new Dictionary<string, string>();

        TextAsset asset = Resources.Load<TextAsset>(resourcePath);
        if (asset == null)
        {
            Debug.LogError($"CSV 로드 실패: Resources/{resourcePath}.csv");
            return dict;
        }

        var lines = asset.text
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Skip(1)
            .ToArray();

        foreach (var line in lines)
        {
            var cols = line.Split(',');
            if (cols.Length < 2)
                continue;

            string key = cols[0].Trim();
            string value = cols[1].Trim();

            if (!dict.ContainsKey(key))
                dict.Add(key, value);
        }

        return dict;
    }

    /// <summary>
    /// Resources 폴더의 TextAsset -> List 로드
    /// </summary>
    private List<string> LoadCsvToList(string resourcePath)
    {
        var list = new List<string>();
        TextAsset asset = Resources.Load<TextAsset>(resourcePath);
        if (asset == null)
        {
            Debug.LogError($"CSV 로드 실패: Resources/{resourcePath}.csv");
            return list;
        }

        var lines = asset.text
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Skip(1)
            .ToArray();

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (!string.IsNullOrEmpty(trimmed))
                list.Add(trimmed);
        }

        return list;
    }
    #endregion
}
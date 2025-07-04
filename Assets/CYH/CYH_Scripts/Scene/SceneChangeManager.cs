using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : Singleton<SceneChangeManager>
{
    // 1. Dictionary<TextAsset이름, 씬 이름>
    // 2. 각 씬을 딕셔너리 인덱스 순서대로 로드
    // 3. 로드한 씬이 대화씬이라면 DialogParser DialogName에 씬 이름 넣어주기

    private AsyncOperation _asyncLoad;

    private float _loadStartTime;                       // 로딩 시작 시각 
    private float _minLoadingTime;                      // 최소 로드 시간
    private string _targetSceneName;                    // 로드할 씬

    [SerializeField] private int SceneNum = 0;          // 현재 씬 번호                     

    private Dictionary<string, string> _gameSceneDict;  // 스테이지 정보 딕셔너리

    [SerializeField] private List<string> _stageInfo;   // 스테이지 리스트


    protected override void Awake()
    {
        base.Awake();

        _gameSceneDict = LoadCsvToDictionary("Csvs/GameScenes");
        _stageInfo = LoadCsvToList("Csvs/StageInfo");
    }

    void Update()
    {
        // 테스트

        // 일반씬 -> 로딩씬 -> 목표씬
        if (Input.GetKeyDown(KeyCode.Alpha0))
            SceneChangeManager.Instance.LoadSceneWithLoading("LoadingScene", "BattleScene");

        // 일반씬 -> 로딩씬 -> 목표씬 (최소 로드시간 보장)
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SceneChangeManager.Instance.LoadSceneWithLoading("LoadingScene", "BattleScene", 5f);

        // 씬 -> 씬
        if (Input.GetKeyDown(KeyCode.Alpha2))
            SceneChangeManager.Instance.LoadSceneAsync("BattleScene");

        // 씬 -> 씬
        if (Input.GetKeyDown(KeyCode.Alpha3))
            SceneChangeManager.Instance.LoadSceneAsync("BossScene");

        // 씬 -> 씬
        if (Input.GetKeyDown(KeyCode.Alpha4))
            SceneChangeManager.Instance.LoadNextScene();
    }

    /// <summary>
    /// 현재 씬의 다음 씬 로드
    /// </summary>
    public void LoadNextScene()
    {
        // 현재 씬 이름
        string currentSceneName = SceneManager.GetActiveScene().name;

        // 현재 씬의 인덱스
        SceneNum = _stageInfo.IndexOf(currentSceneName);
        Debug.Log($"currentScene : {currentSceneName} : {SceneNum}번째 씬");

        // 다음 씬 인덱스
        int nextIndex = SceneNum + 1;
        Debug.Log($"nextIndex : {nextIndex}");

        if (SceneNum >= 0 && nextIndex < _stageInfo.Count)
        {
            // 리스트에서 바로 다음 씬 이름 가져오기
            string nextSceneName = _gameSceneDict[_stageInfo[nextIndex]];
            Debug.Log($"nextSceneName : {nextSceneName}");

            // 씬 로드
            LoadSceneAsync(nextSceneName);
            Debug.Log($"nextSceneName : {nextSceneName} : {nextIndex}번째 씬 로드 완료");
            //LoadFileName();
        }
        else
        {
            Debug.LogWarning("로드 불가");
        }
    }

    /// <summary>
    /// 현재 씬의 다음 씬 로드 (씬 이름)
    /// </summary>
    public void LoadNextScene(string sceneName)
    {
        LoadSceneAsync(sceneName);
    }

    /// <summary>
    /// DialogParser DialogName에 파일 이름 넣기
    /// </summary>
    public void LoadFileName()
    {
        // 현재 씬이 대화씬인지 체크
        if (SceneManager.GetActiveScene().name != "DialogScene")
            return;

        // 딕셔너리의 key -> 리스트로 변환

        // DialogName에 들어갈 파일 이름
        string dialogFileName = _stageInfo[SceneNum];

        // 씬 내 DialogParser 컴포넌트 찾아서 세팅
        var parser = FindObjectOfType<DialogParser>();
        if (parser != null)
        {
            //parser.DialogName = dialogFileName;
            Debug.Log($"DialogName 변경: {dialogFileName}");
        }
        else
        {
            Debug.LogWarning("현재 씬에 DialogParser 없음");
        }
    }

    /// <summary>
    /// 씬을 비동기로 로드 (일반씬 -> 일반씬)
    /// </summary>
    public void LoadSceneAsync(string sceneName)
    {
        // 현재 로딩 중인 씬이 있는지 체크
        if (_asyncLoad != null && !_asyncLoad.isDone)
            return;

        StartCoroutine(LoadSceneCoroutine(sceneName));
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
        StartCoroutine(LoadTwoSceneCoroutineWithMinTime());
    }

    #region Coroutine

    /// <summary>
    /// 일반 -> 로딩씬 -> 목표씬 순차 코루틴
    /// </summary>
    private IEnumerator LoadTwoSceneCoroutine(string loadingSceneName)
    {
        // 1) 로딩씬 바로 로드
        yield return StartCoroutine(LoadSceneCoroutine(loadingSceneName));
        // 2) 목표씬 바로 로드
        yield return StartCoroutine(LoadSceneCoroutine(_targetSceneName));
    }

    /// <summary>
    /// 일반 -> 로딩씬 -> 목표씬 순차 코루틴 (최소 로드시간 보장)
    /// </summary>
    private IEnumerator LoadTwoSceneCoroutineWithMinTime()
    {
        // 1) 로딩씬 바로 로드
        yield return StartCoroutine(LoadSceneCoroutine("LoadingScene"));

        // 2) 목표씬은 최소 로드시간 보장 후 로드
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

        // 씬 로딩 완료되었을 때 바로 로드
        _asyncLoad.allowSceneActivation = true;

        // 로딩 진행 중
        while (!_asyncLoad.isDone)
        {
            // 진행도(%)
            float loadProgress = Mathf.Clamp01(_asyncLoad.progress / 0.9f);
            float progressPercent = loadProgress * 100f;
            Debug.Log($"로딩 진행도 : {progressPercent:F0}%");
            yield return null;
        }

        // 로딩에 걸린 시간
        float elapsed = Time.time - _loadStartTime;
        Debug.Log($"[{sceneName}] 로딩 완료: {elapsed:F1}초 소요");
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

        // 로딩 진행도가 90% 이상 || 경과시간이 최소보장시간 이상일 때까지 대기
        while (_asyncLoad.progress < 0.9f || Time.time - _loadStartTime < minTime)
        {
            float loadProgress = Mathf.Clamp01(_asyncLoad.progress / 0.9f);
            float progressPercent = loadProgress * 100f;
            float elapsed = Time.time - _loadStartTime;
            Debug.Log($"[최소 로드 시간 보장] [{sceneName}] 로딩 진행도: {progressPercent:F0}%, 소요 시간: {elapsed:F1}/{minTime:F1}초");
            yield return null;
        }

        _asyncLoad.allowSceneActivation = true;
        while (!_asyncLoad.isDone) yield return null;

        Debug.Log($"[{sceneName}] 로딩 완료(최소 {minTime:F1}초 보장) 총 소요 시간: {(Time.time - _loadStartTime):F1}초");
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
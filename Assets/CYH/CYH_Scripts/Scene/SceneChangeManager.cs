using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : Singleton<SceneChangeManager>
{
    private AsyncOperation _asyncLoad;

    private float _loadStartTime;                           // 로딩 시작 시각 
    private float _minLoadingTime;                          // 최소 로드 시간
    private string _targetSceneName;                        // 로드할 씬

    public int _currentSceneIndex = 0;                      // 현재 씬 번호                     
    public List<string> _stageInfo;                         // 스테이지 리스트
    private Dictionary<string, string> _gameSceneDict;      // 스테이지 정보 딕셔너리


    protected override void Awake()
    {
        base.Awake();

        _gameSceneDict = LoadCsvToDictionary("Csvs/GameScenes");
        _stageInfo = LoadCsvToList("Csvs/StageInfo");
    }

    private void Start()
    {
        // GameStart 씬 로드
        LoadSceneAsync(_gameSceneDict[_stageInfo[0]]);
    }

    /// <summary>
    /// 현재 씬의 다음 씬 로드 (SceneChangeManager.Instance.LoadNextScene())
    /// </summary>
    public void LoadNextScene()
    {
        // 다음 인덱스 계산
        int nextIndex = _currentSceneIndex + 1;

        // 유효 범위 체크
        if (_stageInfo != null && nextIndex < _stageInfo.Count)
        {
            // 씬 정보로 씬 이름 가져오기
            string nextScene = _gameSceneDict[_stageInfo[nextIndex]];
            LoadSceneAsync(nextScene);
            _currentSceneIndex = nextIndex;
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
        _currentSceneIndex = _stageInfo.IndexOf(stageInfo);
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
        // 로딩씬 바로 로드
        yield return StartCoroutine(LoadSceneCoroutine(loadingSceneName));
        // 목표씬 바로 로드
        yield return StartCoroutine(LoadSceneCoroutine(_targetSceneName));
    }

    /// <summary>
    /// 일반 -> 로딩씬 -> 목표씬 순차 코루틴 (최소 로드시간 보장)
    /// </summary>
    private IEnumerator LoadTwoSceneCoroutineWithMinTime()
    {
        // 로딩씬 바로 로드
        yield return StartCoroutine(LoadSceneCoroutine("LoadingScene"));
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

        //TODO: DialogScene인 경우 Player 초기화 스킵 처리

        // 로딩에 걸린 시간
        float elapsed = Time.time - _loadStartTime;
        Debug.Log($"[{_stageInfo[_currentSceneIndex]} / {sceneName}] 로딩 완료: {elapsed:F1}초 소요");
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
            Debug.Log($"[최소 로드 시간 보장] [{sceneName}] 로딩 진행도: {progressPercent:F0}%, 소요 시간: {elapsed:F1}/{minTime:F1}초");
            yield return null;
        }

        //TODO: DialogScene인 경우 Player 초기화 스킵 처리

        _asyncLoad.allowSceneActivation = true;
        while (!_asyncLoad.isDone) yield return null;

        Debug.Log($"[{_stageInfo[_currentSceneIndex]} / {sceneName}] 로딩 완료(최소 {minTime:F1}초 보장) 총 소요 시간: {(Time.time - _loadStartTime):F1}초");
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
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : Singleton<SceneChangeManager>
{
    private AsyncOperation _asyncLoad;

    private float _loadStartTime;                   // 로딩 시작 시각 
    private float _minLoadingTime;                  // 최소 로드 시간
    private string _targetSceneName;                // 로드할 목표씬

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
}
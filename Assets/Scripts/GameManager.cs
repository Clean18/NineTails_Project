using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    // 플레이어가 오토로 돌아갈때는 몬스터의 정보를 알아야함 > 몬스터를 추격하고 공격하기 위해
    // 즉, 싱글톤이든 static이든 오브젝트풀이랑 몬스터들의 정보를 플레이어에서 접근할 수 있던가 해야함
    public GameObject PlayerPrefab;
    public PlayerController Player;
	public Spawner Spawner;

    public static bool IsCheat = false;
    public static bool IsImmortal = false;

    [SerializeField] private AudioMixer _audioMixer;

    private const string BGM_PARAM = "BGM Volume";
    private const string SFX_PARAM = "SFX Volume";

    private const string BGM_PREF = "BGM VolumePref";
    private const string SFX_PREF = "SFX VolumePref";

    [SerializeField] private LoadingFade _loadingFade;

    protected override void Awake()
    {
        base.Awake();

        SetBGMVolume(PlayerPrefs.GetFloat(BGM_PREF, 1f));
        SetSFXVolume(PlayerPrefs.GetFloat(SFX_PREF, 1f));
    }

    void OnEnable()
    {
        // 씬 로딩 후 자동 호출될 메서드 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬 로드시 실행되는 이벤트
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("씬 로드");
        UIManager.Instance.SceneUIList.Clear();

        // 플레이어 생성
        if (DataManager.IsDataInit) StartCoroutine(SceneInitRoutine());
        //StartCoroutine(SceneInitRoutine());

        if (Player != null && !Player.Equals(null))
        {
            Debug.Log("씬로드 데이터 세이브");
            PlayerController.Instance.SaveData();
        }
        _loadingFade.FadeStart();
    }

    IEnumerator SceneInitRoutine()
    {
        // 주석하기
        //if (!DataManager.IsDataInit)
        //{
        //    // 데이터 매니저 초기화
        //    Debug.LogWarning("데이터매니저 초기화 중...");
        //    yield return StartCoroutine(DataManager.Instance.LoadDatas());
        //    Debug.LogWarning("데이터매니저 초기화 완료");
        //}

        if (PlayerController.Instance == null) PlayerInit();

        Debug.LogWarning("플레이어 초기화 중...");
        yield return StartCoroutine(Player.PlayerInitRoutine());
        Debug.LogWarning("플레이어 초기화 완료");

        // 플레이어 사망상태면 풀피로 회복
        if (Player.GetHp() <= 0 || Player.GetIsDead())
        {
            Player.TakeHeal(Player.GetMaxHp());
        }

        // 씬 이동될 때 치트모드가 아니면 무적 해제
        if (!IsCheat) IsImmortal = false;

        // TODO : 씬에 따라 플레이어 활성화 비활성화 > 나중에 크레딧씬 추가되면 추가필요
        //플레이어 비활성화(CYH)
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "GameStartScene" || currentSceneName == "DialogScene" || currentSceneName == "LoadingScene_v1" || currentSceneName == "EndingCreditScene")
        {
            Player.gameObject.SetActive(false);
            Debug.Log(Player.gameObject.activeSelf == false ? "플레이어 비활성화 상태" : "플레이어 활성화 상태");
        }
        Debug.LogWarning("씬 전환 초기화 완료");
    }

    public void PlayerInit()
    {
        var go = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
        var player = go.GetComponent<PlayerController>();
        Player = player;
    }

    public void OnStartBtn() => SceneChangeManager.Instance.LoadFirstScene();

    public void SetBGMVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        _audioMixer.SetFloat(BGM_PARAM, dB);
        PlayerPrefs.SetFloat(BGM_PREF, value);
    }

    public void SetSFXVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        _audioMixer.SetFloat(SFX_PARAM, dB);
        PlayerPrefs.SetFloat(SFX_PREF, value);
    }

    public float GetSavedBGMVolume() => PlayerPrefs.GetFloat(BGM_PREF, 1f);
    public float GetSavedSFXVolume() => PlayerPrefs.GetFloat(SFX_PREF, 1f);
}

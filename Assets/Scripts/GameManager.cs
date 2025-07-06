using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    // 플레이어가 오토로 돌아갈때는 몬스터의 정보를 알아야함 > 몬스터를 추격하고 공격하기 위해
    // 즉, 싱글톤이든 static이든 오브젝트풀이랑 몬스터들의 정보를 플레이어에서 접근할 수 있던가 해야함
    public GameObject PlayerPrefab;
    public PlayerController Player;
	public Spawner Spawner;

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

        // 데이터매니저 초기화 되어있으면 == 스타트씬 이후
        // 플레이어 생성
        if (DataManager.IsDataInit) StartCoroutine(SceneInitRoutine());

        if (Player != null && !Player.Equals(null))
        {
            Debug.Log("씬로드 데이터 세이브");
            PlayerController.Instance.SaveData();
        }
        //StartCoroutine(SceneInitRoutine());
    }

    IEnumerator SceneInitRoutine()
    {
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
        Player.IsImmortal = false;
        Debug.LogWarning("플레이어 초기화 완료");

        // TODO : 씬에 따라 플레이어 활성화 비활성화
        //플레이어 비활성화(CYH)
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "GameStartScene" || currentSceneName == "DialogScene" || currentSceneName == "LoadingScene_v1")
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            player.gameObject.SetActive(false);
            Debug.Log(player.gameObject.activeSelf == false ? "플레이어 비활성화 상태" : "플레이어 활성화 상태");
        }

        Debug.LogWarning("씬 전환 초기화 완료");
    }

    public void PlayerInit()
    {
        var go = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
        var player = go.GetComponent<PlayerController>();
        Player = player;
    }

    public void OnStartBtn()
    {
        Debug.Log("게임시작 버튼 클릭 > 씬 전환");
        //SceneChangeManager.Instance.LoadSceneWithLoading("Loading", "Stage1-1_Battle", 5f);

        SceneChangeManager.Instance.LoadFirstScene();
    }
}

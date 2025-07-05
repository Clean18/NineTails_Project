using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    // 플레이어가 오토로 돌아갈때는 몬스터의 정보를 알아야함 > 몬스터를 추격하고 공격하기 위해
    // 즉, 싱글톤이든 static이든 오브젝트풀이랑 몬스터들의 정보를 플레이어에서 접근할 수 있던가 해야함
    public GameObject PlayerPrefab;
    public PlayerController PlayerController;
	public Spawner Spawner;

	public Dictionary<string, SkillData> SkillDic;

    void Start()
	{
		SkillDic = new()
		{
			["Fireball"] = Resources.Load<Fireball>("Skills/Fireball"),
		};
        foreach (var skill in SkillDic.Values)
        {
            skill.IsCooldown = false;
        }
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

        StartCoroutine(SceneInitRoutine());
    }

    IEnumerator SceneInitRoutine()
    {
        if (!DataManager.IsDataInit)
        {
            // 데이터 매니저 초기화
            Debug.LogWarning("데이터매니저 초기화 중...");
            yield return StartCoroutine(DataManager.Instance.LoadDatas());
            Debug.LogWarning("데이터매니저 초기화 완료");
        }

        var go = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
        var player = go.GetComponent<PlayerController>();

        Debug.LogWarning("플레이어 초기화 중...");
        yield return StartCoroutine(player.PlayerInitRoutine());
        Debug.LogWarning("플레이어 초기화 완료");

        Debug.LogWarning("씬 전환 초기화 완료");
    }

    public SkillData GetSkill(string skillName) => SkillDic.TryGetValue(skillName, out SkillData skill) ? skill : null;
}

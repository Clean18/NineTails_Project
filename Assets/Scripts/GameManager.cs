using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
public enum StatType
{
    Attack,
    Defense,
    Hp,
    Speed,
    Cost
}

public class GameManager : Singleton<GameManager>
{
    // 플레이어가 오토로 돌아갈때는 몬스터의 정보를 알아야함 > 몬스터를 추격하고 공격하기 위해
    // 즉, 싱글톤이든 static이든 오브젝트풀이랑 몬스터들의 정보를 플레이어에서 접근할 수 있던가 해야함
    public GameObject PlayerPrefab;
    public PlayerController PlayerController;
	public Spawner Spawner;

	public Dictionary<string, SkillData> SkillDic;
    public Dictionary<StatType, Dictionary<int, long>> StatDic = new();

    protected override void Awake()
    {
        base.Awake();

        // 레벨별 스탯 수치
        StartCoroutine(StatInit());
    }

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

        Debug.Log("플레이어 오브젝트 생성");
        var go = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
        go.GetComponent<PlayerController>().PlayerInit();
    }


    public SkillData GetSkill(string skillName) => SkillDic.TryGetValue(skillName, out SkillData skill) ? skill : null;

    IEnumerator StatInit()
    {
        // CSV 다운로드
        string csvString = "https://docs.google.com/spreadsheets/d/1gRFa0xZI2dQDW37blA48rheCbATOGygO/gviz/tq?tqx=out:csv&sheet=Character_StatLevel";
        UnityWebRequest csvData = UnityWebRequest.Get(csvString);
        yield return csvData.SendWebRequest();

        if (csvData.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("CSV 다운로드 실패");
            // TODO : 게임종료
            yield break;
        }

        // 딕셔너리 초기화
        StatDic = new()
        {
            [StatType.Attack] = new(),   // 공격력
            [StatType.Hp] = new(),       // 체력
            [StatType.Defense] = new(),  // 방어력
            [StatType.Cost] = new(),     // 레벨업 비용
            [StatType.Speed] = new(),    // 이동속도
        };

        string csv = csvData.downloadHandler.text;
        string[] lines = csv.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] cells = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"); // 정규식

            int statLevel = int.Parse(Clean(cells[0]));
            long attack = long.Parse(Clean(cells[1]));
            long hp = long.Parse(Clean(cells[2]));
            long defense = long.Parse(Clean(cells[3]));
            long levelupCost = long.Parse(Clean(cells[4]));
            long speed = 0;
            if (long.TryParse(Clean(cells[5]), out long result)) speed = result;

            StatDic[StatType.Attack][statLevel] = attack;
            StatDic[StatType.Hp][statLevel] = hp;
            StatDic[StatType.Defense][statLevel] = defense;
            StatDic[StatType.Cost][statLevel] = levelupCost;
            StatDic[StatType.Speed][statLevel] = speed;

            //Debug.Log("===============");
            //Debug.Log($"{statLevel} 레벨");
            //Debug.Log($"공격력 : {attack}");
            //Debug.Log($"체력 : {hp}");
            //Debug.Log($"방어력 : {defense}");
            //Debug.Log($"비용 : {levelupCost}");
            //Debug.Log($"스피드 : {speed}");
            //Debug.Log("===============");
        }
    }
    string Clean(string s) => s.Trim().Trim('"').Replace(",", ""); // " , 제거
}

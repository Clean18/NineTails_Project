using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
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

	public PlayerController PlayerController;
	public Spawner Spawner;

	public Dictionary<string, SkillData> SkillDic;
    public Dictionary<StatType, Dictionary<int, long>> StatDic;

	void Start()
	{
		SkillDic = new()
		{
			["Fireball"] = Resources.Load<Fireball>("Skills/Fireball"),
		};

		// 스킬을 사용한다는건 오브젝트풀 사용
		// 오브젝트풀에서 사용할 오브젝트를 가져오고 플레이어의 스탯 * 스킬의 Damage계수 = 총대미지

        foreach (var skill in SkillDic.Values)
        {
            skill.IsCooldown = false;
        }

        // 레벨별 스탯 수치
        StartCoroutine(StatInit());

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

            Debug.Log($"----------\n{statLevel} 레벨");
            Debug.Log($"공격력 : {attack}");
            Debug.Log($"체력 : {hp}");
            Debug.Log($"방어력 : {defense}");
            Debug.Log($"비용 : {levelupCost}");
            Debug.Log($"스피드 : {speed}\n----------");
        }
    }
    string Clean(string s) => s.Trim().Trim('"').Replace(",", ""); // " , 제거
}

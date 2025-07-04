using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

#region Data Enum
public enum StatDataType
{
	Attack,     // 공격력
	Defense,    // 방어력
	Hp,         // 체력
	Speed,      // 이동속도
	Cost        // 레벨업 비용
}
/// <summary>
/// 장비 등급
/// </summary>
public enum GradeType
{
	Normal,     // 평범
	Common,     // 고급
	Uncommon,   // 진귀
	Rare,       // 설화
}
/// <summary>
/// 강화 정보 구조체
/// <br/>* string Grade : 장비 등급
/// <br/>* int Level : 강화 단계
/// <br/>* float Attack : 공격력 % 수치
/// <br/>* float CooldownReduction : 스킬 쿨타임 감소
/// <br/>* float ReduceDamage : 받는 피해 감소 관통 수치
/// <br/>* int WarmthCost : 강화에 필요한 재화개수
/// </summary>
[System.Serializable]
public struct UpgradeInfo
{
	public string Grade;            // 장비의 등급
	public int Level;               // 강화 단계    
	public float Attack;            // (플레이어) 공격력% 수치
	public float CooldownReduction; // (플레이어) 스킬 쿨타임 감소
	public float ReduceDamage;      // (몬스터) 받는 피해 감소 관통 수치
	public long WarmthCost;         // 강화에 필요한 재화개수
	public int IncreaseDamageLevel; // 가하는 피해 증가 레벨

	public UpgradeInfo(string grade = "N", int level = 1, float attack = 0.01f, float cooldown = 0f, float reduceDamage = 0f, long warmthCost = 1, int increaseDamageLevel = 0)
	{
		Grade = grade;
		Level = level;
		Attack = attack;
		CooldownReduction = cooldown;
		ReduceDamage = reduceDamage;
		WarmthCost = warmthCost;
		IncreaseDamageLevel = increaseDamageLevel;
	}
}
/// <summary>
/// 승급 정보 구조체
/// <br/>* string CurrentGrade : 현재 등급
/// <br/>* string UpgradeGrade : 다음 등급
/// <br/>* long WarmthCost : 승급 비용
/// <br/>* float SuccessRate : 승급 확률
/// </summary>
[System.Serializable]
public struct PromotionInfo
{
	public string CurrentGrade;    // 현재 장비의 등급
	public string UpgradeGrade;    // 승급 장비 등급   
	public long WarmthCost;         // 승급에 필요한 재화개수
	public float SuccessRate;      // 승급 성공 확률

	public PromotionInfo(string currentGrade = "N", string upgradeGrade = "R", long warmthCost = 325, float successRate = 1f)
	{
		CurrentGrade = currentGrade;
		UpgradeGrade = upgradeGrade;
		WarmthCost = warmthCost;
		SuccessRate = successRate;
	}
}
/// <summary>
/// 미션 정보 구조체
/// </summary>
[System.Serializable]
public struct MissionInfo
{
    public string Id;          // 미션 Id
	public string Stage;       // 현재 씬(스테이지)
	public int TimeLimit;      // 시간제한
	public int Count;          // 킬 조건
	public string NextScene;   // 다음 씬
    public int WarmthReward;   // 온정 보상
    public int SpritReward;    // 영기 보상
    public int SkillPoint;     // 스킬 포인트
}
#endregion

/// <summary>
/// CSV 데이터를 가지고 있을 매니저
/// </summary>
public class DataManager : Singleton<DataManager>
{
    public static bool isInit { get; private set; }

	/// <summary>
	/// 플레이어 스탯의 레벨별 수치 데이터
	/// <br/> ex) Table[스탯타입][레벨] == 스탯값
	/// </summary>
	public Dictionary<StatDataType, Dictionary<int, long>> StatDataTable = new();
	/// <summary>
	/// 플레이어 스탯 레벨별 성장 비용 데이터
	/// <br/> ex) Table[스탯타입][레벨] == 성장 비용
	/// </summary>
	public Dictionary<StatDataType, Dictionary<int, long>> StatCostTable = new();
	/// <summary>
	/// 장비 등급별 필요한 성장 비용 데이터
	/// <br/> ex) Table[등급][레벨] == 성장 비용
	/// </summary>
	public Dictionary<GradeType, Dictionary<int, long>> EquipmentUpgradeCostTable = new();
	/// <summary>
	/// 장비 등급별 장비 스탯
	/// <br/> ex) Table[등급][레벨] == 장비 스탯
	/// </summary>
	public Dictionary<GradeType, Dictionary<int, UpgradeInfo>> EquipmentDataTable = new();
	/// <summary>
	/// 장비 등급별 각성 비용 ex) Table[등급] == 비용
	/// </summary>
	public Dictionary<GradeType, PromotionInfo> EquipmentUpgradeTable = new();
	/// <summary>
	/// 스테이지별 미션 정보 테이블
	/// </summary>
	public Dictionary<string, MissionInfo> MissionTable = new();
    /// <summary>
    /// 일반 스킬 레벨별 영기 비용 ex) Table[레벨] == 비용
    /// </summary>
    public Dictionary<int, int> NormalSkillCostTable = new();
    /// <summary>
    /// 궁극기 스킬 레벨별 영기 비용 ex) Table[레벨] == 비용
    /// </summary>
    public Dictionary<int, int> UltSkillCostTable = new();

    protected override void Awake()
    {
        base.Awake();

        isInit = false;
    }

    public IEnumerator LoadDatas()
	{
        if (isInit) yield break;

		yield return StartCoroutine(StatDataInit());
		yield return StartCoroutine(EquipmentUpgradeCostInit());
		yield return StartCoroutine(EquipmentDataInit());
		yield return StartCoroutine(EquipmentUpgradeInit());
		yield return StartCoroutine(MissionDataInit());
		yield return StartCoroutine(SkillCostInit());

		Debug.Log("모든 데이터 테이블 초기화 완료");
        isInit = true;

    }

	string Clean(string s) => s.Trim().Trim('"').Replace(",", ""); // " , 제거

	IEnumerator StatDataInit()
	{
		// CSV 다운로드
		string csvString = "https://docs.google.com/spreadsheets/d/1gRFa0xZI2dQDW37blA48rheCbATOGygO/gviz/tq?tqx=out:csv&sheet=Character_StatLevel";
		UnityWebRequest csvData = UnityWebRequest.Get(csvString);
		yield return csvData.SendWebRequest();

		if (csvData.result != UnityWebRequest.Result.Success)
		{
			Debug.Log("CSV 다운로드 실패");
            DownloadFailed();
            yield break;
		}

		// 딕셔너리 초기화
		StatDataTable = new()
		{
			[StatDataType.Attack] = new(),   // 공격력
			[StatDataType.Hp] = new(),       // 체력
			[StatDataType.Defense] = new(),  // 방어력
			[StatDataType.Speed] = new(),    // 이동속도
		};

		StatCostTable = new()
		{
			[StatDataType.Attack] = new(),   // 공격력
			[StatDataType.Hp] = new(),       // 체력
			[StatDataType.Defense] = new(),  // 방어력
			[StatDataType.Speed] = new(),    // 이동속도
		};

		string csv = csvData.downloadHandler.text;
		string[] lines = csv.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

		for (int i = 1; i < lines.Length; i++)
		{
			string line = lines[i];
			string[] cells = line.Split(',');

			int statLevel = int.Parse(Clean(cells[0]));
			long attack = long.Parse(Clean(cells[1]));
			long hp = long.Parse(Clean(cells[2]));
			long defense = long.Parse(Clean(cells[3]));
			long levelupCost = long.Parse(Clean(cells[4]));
			long speed = 0;
			if (long.TryParse(Clean(cells[5]), out long result)) speed = result;

			// 성장 데이터 저장
			StatDataTable[StatDataType.Attack][statLevel] = attack;
			StatDataTable[StatDataType.Hp][statLevel] = hp;
			StatDataTable[StatDataType.Defense][statLevel] = defense;
			//StatDataTable[StatDataType.Cost][statLevel] = levelupCost;
			StatDataTable[StatDataType.Speed][statLevel] = speed;

			// 성장 비용 저장
			StatCostTable[StatDataType.Attack][statLevel] = levelupCost;
            StatCostTable[StatDataType.Hp][statLevel] = levelupCost;
            StatCostTable[StatDataType.Defense][statLevel] = levelupCost;
            StatCostTable[StatDataType.Speed][statLevel] = levelupCost;

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
	IEnumerator EquipmentUpgradeCostInit()
	{
		// CSV 다운로드
		string csvString = "https://docs.google.com/spreadsheets/d/1s4FQBQfvX_Dl3fvUFV9DDI5LKe2pRy6RSE-X0X8cNlQ/gviz/tq?tqx=out:csv&sheet=";

		List<GradeType> gradeList = new()
		{
			GradeType.Normal,
			GradeType.Common,
			GradeType.Uncommon,
			GradeType.Rare,
		};

		// 딕셔너리 초기화
		EquipmentUpgradeCostTable = new()
		{
			[GradeType.Normal] = new(),     // 평범
			[GradeType.Common] = new(),     // 고급
			[GradeType.Uncommon] = new(),   // 진귀
			[GradeType.Rare] = new(),       // 설화
		};

		foreach (var grade in gradeList)
		{
			UnityWebRequest csvData = UnityWebRequest.Get(csvString + grade.ToString());
			yield return csvData.SendWebRequest();

			if (csvData.result != UnityWebRequest.Result.Success)
			{
				Debug.Log("CSV 다운로드 실패");
                DownloadFailed();
                yield break;
			}

			string csv = csvData.downloadHandler.text;
			string[] lines = csv.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

			for (int i = 1; i < lines.Length; i++)
			{
				string line = lines[i];
				string[] cells = line.Split(','); // 정규식

				int level = int.Parse(Clean(cells[0]));
				long cost = long.Parse(Clean(cells[1]));

				EquipmentUpgradeCostTable[grade][level] = cost;

				//Debug.Log("===============");
				//Debug.Log($"{grade} 등급");
				//Debug.Log($"레벨 : {level} / 비용 : {cost}");
				//Debug.Log("===============");
			}
		}
	}
	IEnumerator EquipmentDataInit()
	{
		string csvString = "https://docs.google.com/spreadsheets/d/17pNOTI-66c9Q0yRHWgzWHDjiiiZwNyZoFPjT9kQzlh4/gviz/tq?tqx=out:csv&sheet=Upgrade";
		UnityWebRequest csvData = UnityWebRequest.Get(csvString);
		yield return csvData.SendWebRequest();

		if (csvData.result != UnityWebRequest.Result.Success)
		{
			Debug.Log("CSV 다운로드 실패");
            DownloadFailed();
            yield break;
		}

		EquipmentDataTable = new()
		{
			[GradeType.Normal] = new(),     // 평범
			[GradeType.Common] = new(),     // 고급
			[GradeType.Uncommon] = new(),   // 진귀
			[GradeType.Rare] = new(),       // 설화
		};

		string csv = csvData.downloadHandler.text;
		string[] lines = csv.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

		for (int i = 1; i < lines.Length; i++)
		{
			string line = lines[i];
			string[] cells = line.Split(',');

			string grade = Clean(cells[0]);
			int level = int.Parse(Clean(cells[1]));
			float attack = float.Parse(Clean(cells[2]));
			float cooldown = float.Parse(Clean(cells[3]));
			float reduceDamage = float.Parse(Clean(cells[4]));
			long warmthCost = long.Parse(Clean(cells[5]));

			UpgradeInfo info = new UpgradeInfo
			{
				Grade = grade,
				Level = level,
				Attack = attack,
				CooldownReduction = cooldown,
				ReduceDamage = reduceDamage,
				WarmthCost = warmthCost,
			};

			switch (grade)
			{
				case "N": EquipmentDataTable[GradeType.Normal][level] = info; break;
				case "R": EquipmentDataTable[GradeType.Common][level] = info; break;
				case "SR": EquipmentDataTable[GradeType.Uncommon][level] = info; break;
			}

			//Debug.Log($"{grade} 등급");
			//Debug.Log($"레벨 : {level}");
			//Debug.Log($"공격력 증가 : {attack * 100}%");
			//Debug.Log($"쿨타임 감소 : {cooldown * 100}%");
			//Debug.Log($"댐감 무시 : {reduceDamage * 100}%");
			//Debug.Log($"성장 비용 : {warmthCost}");
		}
	}
	IEnumerator MissionDataInit()
	{
		// CSV 다운로드
		string csvString = "https://docs.google.com/spreadsheets/d/1n7AH55p6OCQZMm6MolTxhY2X7k8kQXoIDH2qoGv4RIc/export?format=csv&gid=929060478";
		UnityWebRequest csvData = UnityWebRequest.Get(csvString);
		yield return csvData.SendWebRequest();

		if (csvData.result != UnityWebRequest.Result.Success)
		{
			Debug.Log("CSV 다운로드 실패");
            DownloadFailed();
            yield break;
		}

		// 딕셔너리 초기화
		MissionTable = new();

		string csv = csvData.downloadHandler.text;
		string[] lines = csv.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

		for (int i = 1; i < lines.Length; i++)
		{
			string line = lines[i];
			string[] cells = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

            string id = Clean(cells[0]);
			string stage = Clean(cells[1]);
			int timeLimit = int.Parse(Clean(cells[2]));
			int count = int.Parse(Clean(cells[3]));
			string nextScene = Clean(cells[4]);
            int warmthReward = int.Parse(Clean(cells[5]));
            int spritReward = int.Parse(Clean(cells[6]));
            int skillPoint = int.Parse(Clean(cells[7]));

            MissionInfo info = new MissionInfo
            {
                Id = id,
                Stage = stage,
                TimeLimit = timeLimit,
                Count = count,
                NextScene = nextScene,
                WarmthReward = warmthReward,
                SpritReward = spritReward,
                SkillPoint = skillPoint
            };
            MissionTable[stage] = info;
        }
	}
	IEnumerator EquipmentUpgradeInit()
	{
		string csvString = "https://docs.google.com/spreadsheets/d/17pNOTI-66c9Q0yRHWgzWHDjiiiZwNyZoFPjT9kQzlh4/gviz/tq?tqx=out:csv&sheet=Promotion";
		UnityWebRequest csvData = UnityWebRequest.Get(csvString);
		yield return csvData.SendWebRequest();

		if (csvData.result != UnityWebRequest.Result.Success)
		{
			Debug.Log("CSV 다운로드 실패");
            DownloadFailed();
            yield break;
		}

		EquipmentUpgradeTable = new()
		{
			[GradeType.Normal] = new(),     // 평범
			[GradeType.Common] = new(),     // 고급
			[GradeType.Uncommon] = new(),   // 진귀
			[GradeType.Rare] = new(),       // 설화
		};

		string csv = csvData.downloadHandler.text;
		string[] lines = csv.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

		for (int i = 1; i < lines.Length; i++)
		{
			string line = lines[i];
			string[] cells = line.Split(',');

			string currentGrade = Clean(cells[0]);
			string nextGrade = Clean(cells[1]);
			long warmthCost = long.Parse(Clean(cells[2]));
			float successRate = float.Parse(Clean(cells[3]));

			PromotionInfo info = new PromotionInfo
			{
				CurrentGrade = currentGrade,
				UpgradeGrade = nextGrade,
				WarmthCost = warmthCost,
				SuccessRate = successRate,
			};

			switch (currentGrade)
			{
				case "N": EquipmentUpgradeTable[GradeType.Normal] = info; break;
				case "R": EquipmentUpgradeTable[GradeType.Common] = info; break;
				case "SR": EquipmentUpgradeTable[GradeType.Uncommon] = info; break;
			}

			//Debug.Log($"{currentGrade} 등급 -> {nextGrade} 등급");
			//Debug.Log($"승급 비용 : {warmthCost}");
			//Debug.Log($"승급 확률 : {successRate * 100}%");
		}
	}
    IEnumerator SkillCostInit()
    {
        // CSV 다운로드
        string csvString = "https://docs.google.com/spreadsheets/d/1DrO4aB5Gmi2taIy4tMzLPItZq4yxLkfufWEaGDU-d4Q/gviz/tq?tqx=out:csv&sheet=SkillUpgrade";
        UnityWebRequest csvData = UnityWebRequest.Get(csvString);
        yield return csvData.SendWebRequest();

        if (csvData.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("CSV 다운로드 실패");
            DownloadFailed();
            yield break;
        }

        // 딕셔너리 초기화
        NormalSkillCostTable = new();

        UltSkillCostTable = new();

        string csv = csvData.downloadHandler.text;
        string[] lines = csv.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] cells = line.Split(',');

            int skilllevel = int.Parse(Clean(cells[0]));
            int normalCost = int.Parse(Clean(cells[1]));
            int ultCost = int.Parse(Clean(cells[2]));

            NormalSkillCostTable[skilllevel] = normalCost;
            UltSkillCostTable[skilllevel] = ultCost;

            Debug.Log("===============");
            Debug.Log($"{skilllevel} 레벨");
            Debug.Log($"노말 스킬 비용 : {normalCost}");
            Debug.Log($"궁극기 비용 : {ultCost}");
            Debug.Log("===============");
        }
    }

    void DownloadFailed()
    {
        Debug.Log("다운로드 실패");
        // TODO : 게임종료할지 어떻게할지
    }

	#region Table Get 함수
	/// <summary>
	/// 플레이어 스탯의 레벨별 수치 데이터
	/// <br/> ex) Table[스탯타입][레벨] == 스탯값
	/// <br/> -1 로 예외처리
	/// </summary>
	public long GetStatData(StatDataType statType, int level) => StatDataTable.TryGetValue(statType, out var data) && data.TryGetValue(level, out long result) ? result : -1;
  /// <summary>
	/// 플레이어 스탯의 레벨별 성장비용
	/// <br/> ex) Table[스탯타입][레벨] == 성장비용
	/// <br/> long.MaxValue 로 예외처리
	/// </summary>
    public long GetStatCost(StatDataType statType, int level) => StatCostTable.TryGetValue(statType, out var cost) && cost.TryGetValue(level, out long result) ? result : long.MaxValue;
	/// <summary>
	/// 장비 등급별 필요한 성장 비용 데이터
	/// <br/> ex) Table[등급][레벨] == 성장 비용
	/// <br/> -1 로 예외처리
	/// </summary>
	public long GetEquipmentUpgradeCost(GradeType gradeType, int level) => EquipmentUpgradeCostTable.TryGetValue(gradeType, out var data) && data.TryGetValue(level, out long result) ? result : -1;
	/// <summary>
	/// 장비 등급별 장비 스탯
	/// <br/> ex) Table[등급][레벨] == 장비 스탯
	/// </summary>
	public UpgradeInfo GetEquipmentUpgradeInfo(GradeType gradeType, int level) => EquipmentDataTable.TryGetValue(gradeType, out var data) && data.TryGetValue(level, out var result) ? result : new UpgradeInfo();
	/// <summary>
	/// 장비 등급별 각성 비용 ex) Table[등급] == 비용
	/// </summary>
	public PromotionInfo GetEquipmentPromotionInfo(GradeType currentGrade) => EquipmentUpgradeTable.TryGetValue(currentGrade, out var result) ? result : new PromotionInfo();
    /// <summary>
    /// 노말 스킬 레벨별 강화 비용 ex) Table[레벨] == 비용
    /// </summary>
    /// <returns></returns>
    public int GetNormalSkillCost(int level) => NormalSkillCostTable.TryGetValue(level, out int result) ? result : int.MaxValue;
    /// <summary>
    /// 노말 스킬 레벨별 강화 비용 ex) Table[레벨] == 비용
    /// </summary>
    /// <returns></returns>
    public int GetUltSkillCost(int level) => UltSkillCostTable.TryGetValue(level, out int result) ? result : int.MaxValue;
    #endregion
}

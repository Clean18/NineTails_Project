using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public enum StatDataType
{
    Attack,     // 공격력
    Defense,    // 방어력
    Hp,         // 체력
    Speed,      // 이동속도
    Cost        // 레벨업 비용
}
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
    public int WarmthCost;          // 강화에 필요한 재화개수
}
/// <summary>
/// 승급 정보 구조체
/// </summary>
[System.Serializable]
public struct PromotionInfo
{
    public string CurrentGrade;    // 현재 장비의 등급
    public string UpgradeGrade;    // 승급 장비 등급   
    public int WarmthCost;         // 승급에 필요한 재화개수
    public float SuccessRate;      // 승급 성공 확률
}

/// <summary>
/// 미션 정보 구조체
/// </summary>
[System.Serializable]
public struct MissionInfo
{
    public string Stage;       // 현재 씬(스테이지)
    public int TimeLimit;      // 시간제한
    public int Count;          // 킬 조건
    public string NextScene;   // 다음 씬
}

/// <summary>
/// CSV 데이터를 가지고 있을 매니저
/// </summary>
public class DataManager : Singleton<DataManager>
{
    /// <summary>
    /// 플레이어 스탯의 레벨별 수치 데이터
    /// <br/> ex) Table[스탯타입][레벨] == 스탯값
    /// </summary>
    public Dictionary<StatDataType, Dictionary<int, long>> StatDataTable = new();
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
    public List<PromotionInfo> EquipmentUpgradeTable = new();
    // TODO : 스킬 테이블 > 스크립터블 오브젝트로 두고 계산식 사용
    // TODO : 몬스터 테이블

    /// <summary>
    /// 스테이지별 미션 정보 테이블
    /// </summary>
    public Dictionary<string, MissionInfo> MissionTable = new();
    protected override void Awake()
    {
        base.Awake();

        // 데이터테이블 초기화
        StartCoroutine(LoadDatas());
    }

    IEnumerator LoadDatas()
    {
        yield return StartCoroutine(StatDataInit());
        yield return StartCoroutine(EquipmentUpgradeCostInit());
        yield return StartCoroutine(EquipmentDataInit());
        yield return StartCoroutine(EquipmentUpgradeInit());
        yield return StartCoroutine(MissionDataInit());
        Debug.Log("모든 데이터 테이블 초기화 완료");
    }

    IEnumerator StatDataInit()
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
        StatDataTable = new()
        {
            [StatDataType.Attack] = new(),   // 공격력
            [StatDataType.Hp] = new(),       // 체력
            [StatDataType.Defense] = new(),  // 방어력
            [StatDataType.Cost] = new(),     // 레벨업 비용
            [StatDataType.Speed] = new(),    // 이동속도
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

            StatDataTable[StatDataType.Attack][statLevel] = attack;
            StatDataTable[StatDataType.Hp][statLevel] = hp;
            StatDataTable[StatDataType.Defense][statLevel] = defense;
            StatDataTable[StatDataType.Cost][statLevel] = levelupCost;
            StatDataTable[StatDataType.Speed][statLevel] = speed;

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
                // TODO : 게임종료
                yield break;
            }

            string csv = csvData.downloadHandler.text;
            string[] lines = csv.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 3; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] cells = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"); // 정규식

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
        string csvString = "https://docs.google.com/spreadsheets/d/17pNOTI-66c9Q0yRHWgzWHDjiiiZwNyZoFPjT9kQzlh4/export?format=csv&gid=0";
        UnityWebRequest csvData = UnityWebRequest.Get(csvString);
        yield return csvData.SendWebRequest();

        if (csvData.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("CSV 다운로드 실패");
            // TODO : 게임종료
            yield break;
        }

        EquipmentUpgradeCostTable = new()
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
            string[] cells = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

            string grade = Clean(cells[0]);
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

            string stage = Clean(cells[0]);
            int timeLimit = int.Parse(Clean(cells[1]));
            int count = int.Parse(Clean(cells[2]));
            string nextScene = Clean(cells[3]);
        }
    }

    IEnumerator EquipmentUpgradeInit()
    {
        yield return null;
    }
}

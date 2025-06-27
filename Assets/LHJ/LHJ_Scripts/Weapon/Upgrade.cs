using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 강화 정보 구조체
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
[System.Serializable]
public class UpgradeTable : DataTableParser<UpgradeInfo>
{
    public UpgradeTable(Func<string[], UpgradeInfo> Parse) : base(Parse)
    {
    }
}

public class Upgrade : MonoBehaviour
{
    public UpgradeTable upgradeTable;
    [SerializeField] private string currentGrade;      // 현재 장비 등급
    [SerializeField] private int currentLevel;         // 현재 장비 강화
    [SerializeField] private float currentAttack;      // 현재 장비 공격력% 수치
    [SerializeField] private float CooldownReduction;  // 현재 쿨타임 감소 수치
    [SerializeField] private float ReduceDamage;       // 방어력 감소 수치
    [SerializeField] private int warmth;               // (Test용) 가지고 있는 재화
    private float ssrDamageBonus = 0.05f; // SSR 기본 데미지 증가 수치
    private int baseSSRCost = 150; // 테이블 SR 재화 이후 임시로 넣어둔 재화 
    public Main mainUI;  // Main 변수 선언 

    // 장비등급, 강화단계 읽기 전용 프로퍼티
    public string CurrentGrade => currentGrade;
    public int CurrentLevel => currentLevel;
    public int Warmth => warmth;    // Main UI(온정) 연결용 테스트 프로퍼티

    void Start()
    {
        // 강화 데이터 다운로드 루틴 실행
        StartCoroutine(DownloadRoutine());
    }

    //CSV 다운로드용 스프레드시트 URL
    public const string UpgradeTableURL = "https://docs.google.com/spreadsheets/d/17pNOTI-66c9Q0yRHWgzWHDjiiiZwNyZoFPjT9kQzlh4/export?format=csv&gid=0";
    IEnumerator DownloadRoutine()
    {
        UnityWebRequest request = UnityWebRequest.Get(UpgradeTableURL);
        yield return request.SendWebRequest();

        string csv = request.downloadHandler.text;
        upgradeTable.Parse = words =>
        {
            UpgradeInfo info;
            info.Grade = words[0];                          // 등급
            info.Level = int.Parse(words[1]);               // 강화 단계
            info.Attack = float.Parse(words[2]);            // 공격력% 수치  
            info.CooldownReduction = float.Parse(words[3]); // 쿨타임 감소
            info.ReduceDamage = float.Parse(words[4]);      // 방어력 관통
            info.WarmthCost = int.Parse(words[5]);          // 강화 비용
            return info;
        };
        upgradeTable.Load(csv);
    }
    /// <summary>
    /// 강화 테이블에서 현재 등급과 레벨에 맞는 다음 정보를 찾아 강화
    /// 재화가 충분할 경우 강화 성공 처리 후 현재 공격력과 강화 레벨을 갱신
    /// </summary>
   
    // 장비의 등급, 강화, 남은 재화 수치를 Promotion에 ref하여 보낼 함수
    public void TryPromoteWith(Promotion promotion)
    {
        promotion.TryPromote(ref currentGrade, ref currentLevel, ref warmth);
        mainUI.PlayerStatUI();
    }
    public void TryEnhance()
    {
        // SSR 등급은 무한히 강화가 되는 구조
        if (currentGrade == "SSR")
        {
            if (warmth < baseSSRCost)
            {
                Debug.Log("재화가 부족하여 강화를 할 수 없습니다.");
                return;
            }
            currentLevel += 1;
            warmth -= baseSSRCost;
            float bonusPerLevel = 0.02f; // 매 강화 시 +2%
            ssrDamageBonus += bonusPerLevel;
            Debug.Log($"강화 성공! 현재 등급: {currentGrade}등급, 강화 단계: {currentLevel}강" + $"공격력 증가율: {currentAttack * 100}%" + $"스킬 쿨타임 감소: {CooldownReduction * 100}%" + $"방어력 관통 수치: {ReduceDamage * 100}%"+ $"누적 피해 증가: { ssrDamageBonus}%");

            baseSSRCost++;          // 강화에 들어가는 재화가 1개씩 증가
            return;
        }
        // 강화 테이블에서 다음 강화 정보찾기
        List<UpgradeInfo> upgradeList = upgradeTable.Values;
        UpgradeInfo nextInfo = default;
        bool found = false;

        for (int i = 0; i < upgradeList.Count; i++)
        {
            UpgradeInfo info = upgradeList[i];

            // 현재 등급과 다음 강화 단계 조건이 일치하는지
            if (info.Grade == currentGrade && info.Level == currentLevel + 1)
            {
                nextInfo = info;
                found = true;
                break;
            }
        }

        // 다음 강화 정보가 없을경우
        if (!found)
        {
            Debug.Log("더 이상 강화할 수 없습니다.");
            return;
        }

        // 재화 확인
        if (warmth < nextInfo.WarmthCost)
        {
            Debug.Log("재화가");
            return;
        }
        else
        {
            // 강화 성공 처리
            warmth -= nextInfo.WarmthCost;
            currentLevel += 1;
            currentAttack = nextInfo.Attack;
            CooldownReduction = nextInfo.CooldownReduction;
            ReduceDamage = nextInfo.ReduceDamage;

            Debug.Log($"강화 성공! 현재 등급: {currentGrade}등급, 강화 단계: {currentLevel}강" + $"공격력 증가율: {currentAttack * 100}%" +  $"스킬 쿨타임 감소: {CooldownReduction * 100}%" +  $"방어력 관통 수치: {ReduceDamage * 100}%");
        }
        mainUI.PlayerStatUI();
    }
}

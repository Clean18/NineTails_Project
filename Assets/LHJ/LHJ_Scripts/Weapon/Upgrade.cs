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
    public int Grade;       // 장비의 등급
    public int Level;       // 강화 단계    
    public int Attack;      // 공격력 수치
    public int WarmthCost;  // 강화에 필요한 재화개수
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
    [SerializeField] private int currentGrade;      // 현재 장비 등급
    [SerializeField] private int currentLevel;      // 현재 장비 강화
    [SerializeField] private int currentAttack;     // 현재 장비 공격력 수치
    [SerializeField] private int warmth;            // (Test용) 가지고 있는 재화
    void Start()
    {
        // 강화 데이터 다운로드 루틴 실행
        StartCoroutine(DownloadRoutine());
    }

    //CSV 다운로드용 스프레드시트 URL
    public const string UpgradeTableURL = "https://docs.google.com/spreadsheets/d/17pNOTI-66c9Q0yRHWgzWHDjiiiZwNyZoFPjT9kQzlh4/export?format=csv&&gid=0";
    IEnumerator DownloadRoutine()
    {
        UnityWebRequest request = UnityWebRequest.Get(UpgradeTableURL);
        yield return request.SendWebRequest();

        string csv = request.downloadHandler.text;
        upgradeTable.Parse = words =>
        {
            UpgradeInfo info;
            info.Grade = int.Parse(words[0]);       // 등급
            info.Level = int.Parse(words[1]);       // 강화 단계
            info.Attack = int.Parse(words[2]);      // 공격력  
            info.WarmthCost = int.Parse(words[3]);  // 강화 비용
            return info;
        };
        upgradeTable.Load(csv);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TryEnhance();
        }
    }
    /// <summary>
    /// 강화 테이블에서 현재 등급과 레벨에 맞는 다음 정보를 찾아 강화
    /// 재화가 충분할 경우 강화 성공 처리 후 현재 공격력과 강화 레벨을 갱신
    /// </summary>
    public void TryEnhance()
    {
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
            Debug.Log("강화 성공! 현재 등급: " + currentGrade + " , 강화 단계: " + currentLevel + " , 공격력: " + currentAttack);
        }

        // 현재 강화 단계가 최대로 도달되었을때
        if (currentLevel >= 5)
        {
            Debug.Log("최대 강화 도달");
        }
    }
}

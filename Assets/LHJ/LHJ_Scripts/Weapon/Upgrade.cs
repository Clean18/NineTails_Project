using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 강화에 필요한 데이터
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
    // TODO: 강화로직 구현
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 승급 정보 구조체
/// </summary>
public struct PromotionInfo
{
    public int CurrentGrade;       // 현재 장비의 등급
    public int UpgradeGrade;       // 승급 장비 등급   
    public int RequirementLev;     // 승급에 필요한 레벨 요구치
    public int WarmthCost;         // 승급에 필요한 재화개수
}
public class Promotion : MonoBehaviour
{
    public DataTableParser<PromotionInfo> promotionTable;
    void Start()
    {
        // 승급 데이터 다운로드 루틴 실행
        StartCoroutine(DownloadRoutine());
    }

    //CSV 다운로드용 스프레드시트 URL
    public const string PromotionTableURL = "https://docs.google.com/spreadsheets/d/17pNOTI-66c9Q0yRHWgzWHDjiiiZwNyZoFPjT9kQzlh4/export?format=csv&gid=749900094";
    IEnumerator DownloadRoutine()
    {
       UnityWebRequest request = UnityWebRequest.Get(PromotionTableURL);
       yield return request.SendWebRequest();

        string csv = request.downloadHandler.text;
        promotionTable.Parse = words =>
        {
            PromotionInfo info;
            info.CurrentGrade = int.Parse(words[0]);       // 현재 등급
            info.UpgradeGrade = int.Parse(words[1]);       // 승급한 후 장비 등급
            info.RequirementLev = int.Parse(words[2]);     // 승급에 필요한 레벨
            info.WarmthCost = int.Parse(words[3]);         // 승급 비용
            return info;
        };
        promotionTable.Load(csv);
    }
}

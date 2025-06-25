using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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
[System.Serializable]
public class PromotionTable : DataTableParser<PromotionInfo>
{
    public PromotionTable(Func<string[], PromotionInfo> Parse) : base(Parse)
    {
    }
}
public class Promotion : MonoBehaviour
{
    public PromotionTable promotionTable;
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
            info.CurrentGrade = words[0];                  // 현재 등급
            info.UpgradeGrade = words[1];                  // 승급한 후 장비 등급
            info.WarmthCost = int.Parse(words[2]);         // 승급 비용
            info.SuccessRate = float.Parse(words[3]);      // 성공 확률
            return info;
        };
        promotionTable.Load(csv);
    }

    /// <summary>
    /// 현재 장비의 등급이 50레벨에 도달했을 때 승급을 시도하는 함수
    /// 재화가 충분하고 확률 체크에 성공하면 승급 처리
    /// 실패 시에도 재화는 차감
    /// </summary>
    public void TryPromote(ref string currentGrade, ref int currentLevel, ref int warmth)
    {
        if (currentLevel < 50)
        {
            Debug.Log("아직 승급할 수 없습니다. (레벨 50 필요)");
            return;
        }

        // 승급 테이블에서 다음 승급 정보찾기
        List<PromotionInfo> list = promotionTable.Values;
        PromotionInfo nextPromotion = default;

        for (int i = 0; i < list.Count; i++)
        {
            PromotionInfo info = list[i];
            // 현재 등급과 승급 조건이 일치하는지 확인
            if (info.CurrentGrade == currentGrade)
            {
                nextPromotion = info;
                break;
            }
        }

        // 승급에 필요한 재화
        if (warmth < nextPromotion.WarmthCost)
        {
            Debug.Log("승급 재화 부족");
            return;
        }

        // 승급 확률 처리
        float roll = UnityEngine.Random.value;
        if (roll <= nextPromotion.SuccessRate)  //성공률이 20퍼보다 낮을때 강화 성공
        {
            warmth -= nextPromotion.WarmthCost;
            currentGrade = nextPromotion.UpgradeGrade;
            currentLevel = 1;

            Debug.Log($"승급 성공! 등급: {currentGrade}, 강화 초기화됨");
        }
        else // 반대인 경우
        {
            warmth -= nextPromotion.WarmthCost;
            Debug.Log($"승급 실패...");
        }
    }
}

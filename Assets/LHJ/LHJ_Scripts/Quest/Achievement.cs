using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public struct AchievementInfo
{
    public string Type;          // 업적타입
    public string Id;            // 업적고유 ID
    public string Name;          // 업적이름
    public string Purpose;       // 조건(스테이지)
    public int WarmthReward;     // 온정보상
    public int SpritReward;      // 영기보상
}

[System.Serializable]
public class AchievementTable : DataTableParser<AchievementInfo>
{
    public AchievementTable(Func<string[], AchievementInfo> Parse) : base(Parse) 
    { 
    }
}
public class Achievement : MonoBehaviour
{
    public AchievementTable achievementTable;

    void Start()
    {
        // 업적 테이블 다운로드 루틴 실행
        StartCoroutine(DownloadRoutine());
    }

    // 업적 CSV 스프레드 시트 URL
    private const string AchievementURL = "https://docs.google.com/spreadsheets/d/1n7AH55p6OCQZMm6MolTxhY2X7k8kQXoIDH2qoGv4RIc/export?format=csv&gid=0";
    IEnumerator DownloadRoutine()
    {
        UnityWebRequest request = UnityWebRequest.Get(AchievementURL);
        yield return request.SendWebRequest();

        string csv = request.downloadHandler.text;

        achievementTable.Parse = words =>
        {
            AchievementInfo info;
            info.Type = words[0];                      // 업적 타입
            info.Id = words[1];                        // 업적 고유 ID
            info.Name = words[2];                      // 업적 이름
            info.Purpose = words[3];                   // 목적
            info.WarmthReward = int.Parse(words[4]);   // 온정 보상
            info.SpritReward = int.Parse(words[5]);    // 영기 보상
            return info;
        };
        achievementTable.Load(csv);
    }

}

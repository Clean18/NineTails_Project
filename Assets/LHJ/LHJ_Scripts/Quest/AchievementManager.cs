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
    public string Scene;       // 조건(스테이지)
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
public class AchievementManager : Singleton<AchievementManager>
{
    public AchievementTable achievementTable;

    void Start()
    {
        // 업적 테이블 다운로드 루틴 실행
        StartCoroutine(DownloadRoutine());
    }

    // 업적 CSV 스프레드 시트 URL
    private const string AchievementURL = "https://docs.google.com/spreadsheets/d/1n7AH55p6OCQZMm6MolTxhY2X7k8kQXoIDH2qoGv4RIc/export?format=csv&gid=0";

    private HashSet<string> achievedIds = new();                // 업적 중복 방지
    private Dictionary<string, int> killCount = new();          // 업적별 조건 달성 여부 확인
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
            info.Scene = words[3];                   // 목적
            info.WarmthReward = int.Parse(words[4]);   // 온정 보상
            info.SpritReward = int.Parse(words[5]);    // 영기 보상
            return info;
        };
        achievementTable.Load(csv);
    }
    
    // 스테이지별 킬 업적 조건 검사 함수
    public void KillCount(string currentStageId)
    {
        foreach (var achievement in achievementTable.Values)
        {
            if (achievement.Type != "Kill") continue;               // Type에서 Kill이 아닐경우 무시하고 계속 진행
            if (achievement.Scene != currentStageId) continue;      // 현재 Scene이름과  업적씬이 일치하지않으면 무시하고 계속 진행
            if (achievedIds.Contains(achievement.Id)) continue;     // 달성된 업적이면 무시하고 계속 진행

            // 해당 업적 ID 초기화
            if (!killCount.ContainsKey(achievement.Id))
                killCount[achievement.Id] = 0;

            // 해당 업적의 킬 카운트를 1 증가
            killCount[achievement.Id]++;

            // 업적 달성 (Test로 5킬 적용)
            if (killCount[achievement.Id] >= 5) 
            {
                achievedIds.Add(achievement.Id);    // 업적 달성 처리
                Debug.Log($"[업적 달성] {achievement.Name} - {killCount[achievement.Id]}킬");
                Reward(achievement);        // 보상 지급
            }
        }
    }
    private void Reward(AchievementInfo achievementInfo)
    {
        Debug.Log($"[보상] 온정 +{achievementInfo.WarmthReward}, 영기 +{achievementInfo.SpritReward}");
    }
}

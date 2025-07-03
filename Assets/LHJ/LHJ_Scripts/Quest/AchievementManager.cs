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
    public string Scene;         // 스테이지
    public int Purpose;          // 업적 달성 조건
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
public enum PromotionAchievementType
{
    Fail = 1,
    NtoR = 2,
    RtoSR = 3,
    SRtoSSR = 4
}
public class AchievementManager : Singleton<AchievementManager>
{
    public AchievementTable achievementTable;
    private int totalDeathCount = 0;

    void Start()
    {
        // 업적 테이블 다운로드 루틴 실행
        StartCoroutine(DownloadRoutine());
    }

    // 업적 CSV 스프레드 시트 URL
    private const string AchievementURL = "https://docs.google.com/spreadsheets/d/1n7AH55p6OCQZMm6MolTxhY2X7k8kQXoIDH2qoGv4RIc/export?format=csv&gid=0";

    public HashSet<string> AchievedIds = new();                // 업적 중복 방지
    public Dictionary<string, int> KillCountDic = new();       // 업적별 조건 달성 여부 확인
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
            info.Scene = words[3];                     // 스테이지
            info.Purpose = int.Parse(words[4]);        // 업적 달성 조건
            info.WarmthReward = int.Parse(words[5]);   // 온정 보상
            info.SpritReward = int.Parse(words[6]);    // 영기 보상
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
            if (AchievedIds.Contains(achievement.Id)) continue;     // 달성된 업적이면 무시하고 계속 진행

            // 해당 업적 ID 초기화
            if (!KillCountDic.ContainsKey(achievement.Id))
                KillCountDic[achievement.Id] = 0;

            // 해당 업적의 킬 카운트를 1 증가
            KillCountDic[achievement.Id]++;

            // 업적 달성 (Test로 5킬 적용)
            if (KillCountDic[achievement.Id] >= achievement.Purpose) 
            {
                AchievedIds.Add(achievement.Id);    // 업적 달성 처리
                Debug.Log($"[업적 달성] {achievement.Name} - {KillCountDic[achievement.Id]}킬");
                Reward(achievement);        // 보상 지급
            }
        }
    }

    // 스테이지 클리어 업적 함수
    public void CheckStageClear(string sceneName)
    {
        foreach (var achievement in achievementTable.Values)
        {
            if (achievement.Type != "StageClear") continue;           // Type에서 StageClear가 아닐경우 무시하고 계속 진행
            if (achievement.Scene != sceneName) continue;             // 현재 Scene이름과 업적에 있는 Scene이 일치하지않으면 무시하고 계속 진행
            if (AchievedIds.Contains(achievement.Id)) continue;       // 이미 달성된 업적이면 계속진행

            AchievedIds.Add(achievement.Id);                          // 업적 달성
            Debug.Log($"[업적 달성] {achievement.Name} - 스테이지 클리어");

            Reward(achievement);                                      // 보상 지급
        }
    }

    // 재화 업적 함수
    public void CheckCurrencyAchievements()
    {
        foreach (var achievement in achievementTable.Values)
        {
            if (achievement.Type != "Currency") continue;       // Type에서 Currnecy가 아닐경우 무시하고 계속진행
            if (AchievedIds.Contains(achievement.Id)) continue; // 이미 달성된 업적이면 무시하고 진행

            bool isAchieved = false;    // 업적 달성여부

            // 온정 업적 체크: 보상이 온정이고 플레이어 보유 온정이 조건보다 높거나 같을때 
            if (achievement.WarmthReward > 0 &&
                PlayerController.Instance.GetCost(CostType.Warmth) >= achievement.Purpose)
                isAchieved = true;

            // 영기 업적 체크: 보상이 영기이고 플레이어 보유 영기가 조건보다 높거나 같을때 
            if (achievement.SpritReward > 0 &&
                PlayerController.Instance.GetCost(CostType.SpiritEnergy) >= achievement.Purpose)
                isAchieved = true;

            if (isAchieved) // 조건이 하나라도 만족할때
            {
                AchievedIds.Add(achievement.Id);    // 해당 업적 달성
                Debug.Log($"[업적 달성] {achievement.Name} - 재화 조건 달성");
                Reward(achievement);
            }
        }
    }
    // 전투력 업적 조건을 확인하고 달성 여부를 판단하는 함수
    public void CheckPowerAchievements()
    {
        foreach (var achievement in achievementTable.Values)
        {
            if (achievement.Type != "Power") continue;               // Type에서 Power가 아닐경우 무시하고 계속진행
            if (AchievedIds.Contains(achievement.Id)) continue;      // 이미 달성된 업적이면 무시하고 진행

            // 전투력 업적과 플레이어 전투력 비교
            if (PlayerController.Instance.GetPower() >= achievement.Purpose)
            {
                AchievedIds.Add(achievement.Id);     // 해당 업적 달성    
                Debug.Log($"[업적 달성] {achievement.Name} - 전투력 조건 달성");
                Reward(achievement);                 // 보상 지급
            }
        }
    }
    public void CheckDeathAchievements()
    {
        totalDeathCount++;  // 죽음 1회 누적

        foreach (var achievement in achievementTable.Values)
        {
            if (achievement.Type != "Death") continue;              // Type에서 Death가 아닐경우 무시하고 계속 진행
            if (AchievedIds.Contains(achievement.Id)) continue;     // 이미 달성된 업적이면 무시하고 진행

            if (totalDeathCount >= achievement.Purpose)
            {
                AchievedIds.Add(achievement.Id);    // 해당 업적 달성 
                Debug.Log($"[업적 달성] {achievement.Name} - 누적 {totalDeathCount}회 사망");
                Reward(achievement);                // 보상 지급
            }
        }
    }
    // 장비 강화 업적 조건을 확인하고 달성 여부를 판단하는 함수
    public void CheckEnhancementAchievements(int currentEnhancementLevel)
    {
        foreach (var achievement in achievementTable.Values)
        {
            if (achievement.Type != "Enhancement") continue;               // Type에서 Enhancement가 아닐 경우 무시
            if (AchievedIds.Contains(achievement.Id)) continue;            // 이미 달성된 업적이면 무시

            if (currentEnhancementLevel >= achievement.Purpose)
            {
                AchievedIds.Add(achievement.Id);                           // 업적 달성 처리
                Debug.Log($"[업적 달성] {achievement.Name} - 강화 레벨 {currentEnhancementLevel} 도달");
                Reward(achievement);                                       // 보상 지급
            }
        }
    }

    // 승급 업적 조건
    public void CheckPromotionAchievements(string currentGrade, string nextGrade, bool isSuccess)
    {
        foreach (var achievement in achievementTable.Values)
        {
            if (achievement.Type != "Promotion") continue;          // Type에서 Promotion이 아닐경우 무시
            if (AchievedIds.Contains(achievement.Id)) continue;     // 이미 달성된 업적이면 무시

            var purpose = (PromotionAchievementType)achievement.Purpose;    // 엄적 목표

            if (isSuccess)
            {       
                // 승급 성공시   N->R에서 현재 등급이 N이고 다음단계가R일때
                if (purpose == PromotionAchievementType.NtoR && currentGrade == "N" && nextGrade == "R" ||
                    purpose == PromotionAchievementType.RtoSR && currentGrade == "R" && nextGrade == "SR" ||
                    purpose == PromotionAchievementType.SRtoSSR && currentGrade == "SR" && nextGrade == "SSR")
                {
                    AchievedIds.Add(achievement.Id);    // 업적 달성 처리
                    Debug.Log($"[업적 달성] {achievement.Name} - 승급 성공: {currentGrade} → {nextGrade}");
                    Reward(achievement);  // 보상 획득
                }
            }
            else
            {
                // 승급 실패시
                if (purpose == PromotionAchievementType.Fail)
                {
                    AchievedIds.Add(achievement.Id);    // 업적 달성 처리
                    Debug.Log($"[업적 달성] {achievement.Name} - 승급 실패");
                    Reward(achievement);    // 보상 획득
                }
            }
        }
    }

    private void Reward(AchievementInfo achievementInfo)
    {
        Debug.Log($"[보상] 온정 +{achievementInfo.WarmthReward}, 영기 +{achievementInfo.SpritReward}");
        PlayerController.Instance.AddCost(CostType.Warmth, achievementInfo.WarmthReward);
        PlayerController.Instance.AddCost(CostType.SpiritEnergy, achievementInfo.SpritReward);
        // TODO : 업적 정보 넘겨서 클리어 체크
    }
}

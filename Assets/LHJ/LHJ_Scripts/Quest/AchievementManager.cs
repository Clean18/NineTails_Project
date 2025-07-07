using System.Collections.Generic;
using UnityEngine;

public enum PromotionAchievementType
{
    Fail = 1,
    NtoR = 2,
    RtoSR = 3,
    SRtoSSR = 4
}
public class AchievementManager : Singleton<AchievementManager>
{
    public Dictionary<string, bool> AchievedIds = new();       // 업적 중복 방지
    public Dictionary<string, int> KillCountDic = new();       // 업적별 조건 달성 여부 확인
    public Dictionary<string, bool> RewardDic = new();         // 보상 획득 여부 확인
    private int totalDeathCount = 0;

    // 스테이지별 킬 업적 조건 검사 함수
    public void KillCount(string currentStageId)
    {
        foreach (var achievement in DataManager.Instance.AchievementTable.Values)
        {
            if (achievement.Type != "Kill") continue;               // Type에서 Kill이 아닐경우 무시하고 계속 진행
            if (achievement.Scene != currentStageId) continue;      // 현재 Scene이름과  업적씬이 일치하지않으면 무시하고 계속 진행
            if (AchievedIds.ContainsKey(achievement.Id)) continue;  // 달성된 업적이면 무시하고 계속 진행

            // 해당 업적 ID 초기화
            if (!KillCountDic.ContainsKey(achievement.Id))
                KillCountDic[achievement.Id] = 0;

            // 해당 업적의 킬 카운트를 1 증가
            KillCountDic[achievement.Id]++;

            // 업적 달성 (Test로 5킬 적용)
            if (KillCountDic[achievement.Id] >= achievement.Purpose) 
            {
                AchievedIds[achievement.Id] = true;
                Debug.Log($"[업적 달성] {achievement.Name} - {KillCountDic[achievement.Id]}킬");
            }
        }
    }

    // 스테이지 클리어 업적 함수
    public void CheckStageClear(string sceneName)
    {
        foreach (var achievement in DataManager.Instance.AchievementTable.Values)
        {
            if (achievement.Type != "StageClear") continue;           // Type에서 StageClear가 아닐경우 무시하고 계속 진행
            if (achievement.Scene != sceneName) continue;             // 현재 Scene이름과 업적에 있는 Scene이 일치하지않으면 무시하고 계속 진행
            if (AchievedIds.ContainsKey(achievement.Id)) continue;    // 이미 달성된 업적이면 계속진행

            AchievedIds[achievement.Id] = true;
            Debug.Log($"[업적 달성] {achievement.Name} - 스테이지 클리어");
        }
    }

    // 재화 업적 함수
    public void CheckCurrencyAchievements()
    {
        foreach (var achievement in DataManager.Instance.AchievementTable.Values)
        {
            if (achievement.Type != "Currency") continue;       // Type에서 Currnecy가 아닐경우 무시하고 계속진행
            if (AchievedIds.ContainsKey(achievement.Id)) continue; // 이미 달성된 업적이면 무시하고 진행

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
                AchievedIds[achievement.Id] = true;
                Debug.Log($"[업적 달성] {achievement.Name} - 재화 조건 달성");
            }
        }
    }
    // 전투력 업적 조건을 확인하고 달성 여부를 판단하는 함수
    public void CheckPowerAchievements()
    {
        foreach (var achievement in DataManager.Instance.AchievementTable.Values)
        {
            if (achievement.Type != "Power") continue;               // Type에서 Power가 아닐경우 무시하고 계속진행
            if (AchievedIds.ContainsKey(achievement.Id)) continue;   // 이미 달성된 업적이면 무시하고 진행

            // 전투력 업적과 플레이어 전투력 비교
            if (PlayerController.Instance.GetPower() >= achievement.Purpose)
            {
                AchievedIds[achievement.Id] = true;
                Debug.Log($"[업적 달성] {achievement.Name} - 전투력 조건 달성");
            }
        }
    }
    public void CheckDeathAchievements()
    {
        totalDeathCount++;  // 죽음 1회 누적
        foreach (var achievement in DataManager.Instance.AchievementTable.Values)
        {
            if (achievement.Type != "Death") continue;              // Type에서 Death가 아닐경우 무시하고 계속 진행
            if (AchievedIds.ContainsKey(achievement.Id)) continue;     // 이미 달성된 업적이면 무시하고 진행

            if (totalDeathCount >= achievement.Purpose)
            {
                AchievedIds[achievement.Id] = true;
                Debug.Log($"[업적 달성] {achievement.Name} - 누적 {totalDeathCount}회 사망");
            }
        }
    }
    // 장비 강화 업적 조건을 확인하고 달성 여부를 판단하는 함수
    public void CheckEnhancementAchievements(int currentEnhancementLevel)
    {
        foreach (var achievement in DataManager.Instance.AchievementTable.Values)
        {
            if (achievement.Type != "Enhancement") continue;               // Type에서 Enhancement가 아닐 경우 무시
            if (AchievedIds.ContainsKey(achievement.Id)) continue;         // 이미 달성된 업적이면 무시

            if (currentEnhancementLevel >= achievement.Purpose)
            {
                AchievedIds[achievement.Id] = true;
                Debug.Log($"[업적 달성] {achievement.Name} - 강화 레벨 {currentEnhancementLevel} 도달");
            }
        }
    }

    // 승급 업적 조건
    public void CheckPromotionAchievements(string currentGrade, string nextGrade, bool isSuccess)
    {
        foreach (var achievement in DataManager.Instance.AchievementTable.Values)
        {
            if (achievement.Type != "Promotion") continue;          // Type에서 Promotion이 아닐경우 무시
            if (AchievedIds.ContainsKey(achievement.Id)) continue;  // 이미 달성된 업적이면 무시

            var purpose = (PromotionAchievementType)(int)achievement.Purpose;    // 엄적 목표

            if (isSuccess)
            {       
                // 승급 성공시   N->R에서 현재 등급이 N이고 다음단계가R일때
                if (purpose == PromotionAchievementType.NtoR && currentGrade == "N" && nextGrade == "R" ||
                    purpose == PromotionAchievementType.RtoSR && currentGrade == "R" && nextGrade == "SR" ||
                    purpose == PromotionAchievementType.SRtoSSR && currentGrade == "SR" && nextGrade == "SSR")
                {
                    AchievedIds[achievement.Id] = true;
                    Debug.Log($"[업적 달성] {achievement.Name} - 승급 성공: {currentGrade} → {nextGrade}");
                }
            }
            else
            {
                // 승급 실패시
                if (purpose == PromotionAchievementType.Fail)
                {
                    AchievedIds[achievement.Id] = true;
                    Debug.Log($"[업적 달성] {achievement.Name} - 승급 실패");
                }
            }
        }
    }

    public void CheckBossAchievements(string scene)
    {
        foreach (var achievement in DataManager.Instance.AchievementTable.Values)
        {
            if (achievement.Type != "Boss") continue;   // Type이 보스가 아니면 무시
            if (AchievedIds.ContainsKey(achievement.Id)) continue; // 이미 달성한 업적이면 무시
            if (achievement.Scene != scene) continue;   // 업적에 해당하는씬이 아니면 무시

            float purpose = achievement.Purpose;

            if (PlayerController.Instance.GetPower() < purpose)     // 권장 전투력 보다 전투력이 낮을때
            {
                AchievedIds[achievement.Id] = true;
                Debug.Log($"[업적 달성] {achievement.Name} - 권장 전투력 미만 클리어 (보유 전투력: {PlayerController.Instance.GetPower()}, 조건: {purpose})");
            }
     
            else if (purpose > 0f) // 체력 퍼센트 조건
            {
                float hpPercent = (float)PlayerController.Instance.GetHp() / PlayerController.Instance.GetMaxHp();

                if (hpPercent < purpose)    // 클리어 당시 플레이어 체력이 목적퍼센트보다 미만일때
                {
                    AchievedIds[achievement.Id] = true;
                    Debug.Log($"[업적 달성] {achievement.Name} - 체력 조건 충족 현재: {hpPercent}, 조건: {purpose})");
                }
            }
            else if (purpose == 0f) // 피격되지 않고 클리어
            {
                AchievedIds[achievement.Id] = true;
                Debug.Log($"[업적 달성] {achievement.Name} - 노히트 클리어");
            }
        }
    }
    public void Reward(AchievementInfo achievementInfo)
    {
        // 클리어 한 업적이면 보상 지급 X
        if (AchievedIds.TryGetValue(achievementInfo.Id, out bool isClear) && !isClear)
        {
            Debug.Log($"[보상] 온정 +{achievementInfo.WarmthReward}, 영기 +{achievementInfo.SpritReward}");
            PlayerController.Instance.AddCost(CostType.Warmth, achievementInfo.WarmthReward);
            PlayerController.Instance.AddCost(CostType.SpiritEnergy, achievementInfo.SpritReward);
            AchievedIds[achievementInfo.Id] = true;
            RewardDic[achievementInfo.Id] = true;
        }
    }
    public bool IsAchieved(string achievementId)
    {
        return AchievedIds.TryGetValue(achievementId, out var value) && value;
    }
    public bool IsRewarded(string id)
    {
        return RewardDic.TryGetValue(id, out bool rewarded) && rewarded;
    }
    private void Start()
    {
        Debug.Log($"[디버그] 세이브된 총 사망 횟수: {totalDeathCount}");
    }
}

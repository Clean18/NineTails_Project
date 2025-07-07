using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SaveAchievementData
{
	public string Id;
	public bool IsClear;
	public int CurrentCondition;
    public bool IsRewarded;

    public SaveAchievementData(string id, bool isClear, int current, bool isRewarded)
    {
        Id = id;
        IsClear = isClear;
        CurrentCondition = current;
        IsRewarded = isRewarded;
    }
}

[System.Serializable]
public struct SaveMissionData
{
	public string Id;
	public bool IsClear;
	public int CurrentCondition;

    public SaveMissionData(string id, bool isClear, int current)
    {
        Id = id;
        IsClear = isClear;
        CurrentCondition = current;
    }
}

[System.Serializable]
public class PlayerQuest
{
	public void InitQuest(List<SaveAchievementData> saveAchive = null, List<SaveMissionData> saveMission = null)
	{
        if (AchievementManager.Instance == null) return;

        var achievClearTable = AchievementManager.Instance.AchievedIds;
        var achievKillCountTable = AchievementManager.Instance.KillCountDic;
        var achievRewardTable = AchievementManager.Instance.RewardDic;

        if (saveAchive != null)
        {
            // 업적매니저 HashSet에 IsClear가 true면 추가
            // False고 0 이상이면 KillCountDic에 추가
            foreach (var achiev in saveAchive)
            {
                // 클리어 했으면 ClearTable에 추가
                if (achiev.IsClear)
                {
                    achievClearTable[achiev.Id] = true;
                    if (achiev.IsRewarded)
                    {
                        achievRewardTable[achiev.Id] = true;
                    }
                }
                // 클리어 못하고 진행중(0) 이면 KillCountTable에 추가
                else if (!achiev.IsClear && achiev.CurrentCondition > -1)
                {
                    achievKillCountTable.Add(achiev.Id, achiev.CurrentCondition);
                }
            }
        }
        Debug.Log("업적 초기화 완료");

        var missionClearTable = MissionManager.Instance.MissionIds;
        if (saveMission != null)
        {
            foreach (var mission in saveMission)
            {
                // 클리어 했으면 ClearTable에 추가
                if (mission.IsClear)
                {
                    missionClearTable.Add(mission.Id);
                }
            }
        }
        Debug.Log("미션 초기화 완료");
    }

	public List<SaveAchievementData> SaveAchievementData()
	{
        var ClearTable = AchievementManager.Instance.AchievedIds;
        var KillCountTable = AchievementManager.Instance.KillCountDic;
        var RewardDic = AchievementManager.Instance.RewardDic;
        List<SaveAchievementData> list = new();

        foreach (var pair in ClearTable)
        {
            bool isRewarded = RewardDic.ContainsKey(pair.Key) && RewardDic[pair.Key];
            list.Add(new SaveAchievementData(pair.Key, pair.Value, -1, isRewarded));
        }
        foreach (var pair in KillCountTable)
        {
            list.Add(new SaveAchievementData(pair.Key, false, pair.Value, false));
        }

		return list;
	}

	public List<SaveMissionData> SaveMissionData()
	{
        // 미션 아이디랑 체크 필요함
        var ClearTable = MissionManager.Instance.MissionIds;
        List<SaveMissionData> list = new();

        foreach (var clearId in ClearTable)
        {
            list.Add(new SaveMissionData(clearId, true, -1));
        }
        Debug.Log($"저장 : 클리어한 미션 개수 : {ClearTable.Count}");
        return list;
	}
}

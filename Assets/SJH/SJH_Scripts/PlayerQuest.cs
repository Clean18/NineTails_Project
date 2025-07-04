using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct SaveAchievementData
{
	public string Id;
	public bool IsClear;
	public int CurrentCondition;

    public SaveAchievementData(string id, bool isClear, int current)
    {
        Id = id;
        IsClear = isClear;
        CurrentCondition = current;
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
	public void InitQuest(List<SaveAchievementData> saveAchive, List<SaveMissionData> saveMission)
	{
        if (AchievementManager.Instance == null) return;

        // TODO : 업적매니저 HashSet에 IsClear가 true면 추가
        // False고 0 이상이면 KillCountDic에 추가
        var ClearTable = AchievementManager.Instance.AchievedIds;
        var KillCountTable = AchievementManager.Instance.KillCountDic;
        foreach (var achiev in saveAchive)
        {
            // 클리어 했으면 ClearTable에 추가
            if (achiev.IsClear)
            {
                ClearTable.Add(achiev.Id);
            }
            // 클리어 못하고 진행중(0) 이면 KillCountTable에 추가
            else if (!achiev.IsClear && achiev.CurrentCondition > -1)
            {
                KillCountTable.Add(achiev.Id, achiev.CurrentCondition);
            }
        }
        Debug.Log("업적 초기화 완료");

        foreach (var mission in saveMission)
        {
            // 클리어 했으면 ClearTable에 추가
            if (mission.IsClear)
            {
                ClearTable.Add(mission.Id);
            }
            // 클리어 못하고 진행중(0) 이면 KillCountTable에 추가
            else if (!mission.IsClear && mission.CurrentCondition > -1)
            {
                KillCountTable.Add(mission.Id, mission.CurrentCondition);
            }
        }
        Debug.Log("미션 초기화 완료");
    }

	public List<SaveAchievementData> SaveAchievementData()
	{
        var ClearTable = AchievementManager.Instance.AchievedIds;
        var KillCountTable = AchievementManager.Instance.KillCountDic;
        List<SaveAchievementData> list = new();

        foreach (var clearId in ClearTable)
        {
            list.Add(new SaveAchievementData(clearId, true, -1));
        }
        foreach (var pair in KillCountTable)
        {
            list.Add(new SaveAchievementData(pair.Key, false, pair.Value));
        }

		return list;
	}

	public List<SaveMissionData> SaveMissionData()
	{
        // TODO : 미션 아이디랑 체크 필요함
        var ClearTable = MissionManager.Instance.MissionIds;
        List<SaveMissionData> list = new();

        foreach (var clearId in ClearTable)
        {
            list.Add(new SaveMissionData(clearId, true, -1));
        }

        return list;
	}
}

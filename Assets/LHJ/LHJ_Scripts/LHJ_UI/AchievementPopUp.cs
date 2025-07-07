using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class AchievementPopUp : BaseUI
{
    [System.Serializable]
    public class AchievementGroupUI
    {
        public List<string> AchievementIds;     // 여러 업적 ID를 묶은 그룹
        public TextMeshProUGUI nameText;        // 이름 표시 텍스트
        public TextMeshProUGUI descriptionText; // 설명 표시 텍스트
        public Button rewardButton;             // 보상 버튼
        public int currentIndex;                // 현재 진행 중인 업적 위치
    }

    [SerializeField] private List<AchievementGroupUI> achievementGroups;

    private void OnEnable()
    {
        foreach (var group in achievementGroups)
        {
            group.currentIndex = 0;

            for (int i = 0; i < group.AchievementIds.Count; i++)
            {
                string id = group.AchievementIds[i];

                bool isAchieved = AchievementManager.Instance.AchievedIds.ContainsKey(id);
                bool isRewarded = AchievementManager.Instance.IsRewarded(id);

                if (!isAchieved)
                {
                    group.currentIndex = i; // 아직 클리어 안 된 업적
                    break;
                }
                else if (!isRewarded)
                {
                    group.currentIndex = i; // 클리어는 했지만 보상버튼을 안눌렀을때
                    break;
                }

                // 마지막까지 반복되면 모든 업적 완료
                if (i == group.AchievementIds.Count - 1)
                {
                    group.currentIndex = group.AchievementIds.Count;
                }
            }
            UpdateGroupUI(group);
        }
    }

    private void UpdateGroupUI(AchievementGroupUI group)
    {
        if (group.currentIndex >= group.AchievementIds.Count)
        {
            group.nameText.text = "모든 업적을 완료했습니다!";
            group.descriptionText.text = "";
            group.rewardButton.gameObject.SetActive(false);
            return;
        }

        string currentId = group.AchievementIds[group.currentIndex];
        if (!DataManager.Instance.AchievementTable.TryGetValue(currentId, out var info))
        {
            group.rewardButton.gameObject.SetActive(false);
            return;
        }

        group.nameText.text = info.Name;
        group.descriptionText.text = info.Description;

        bool isAchieved = AchievementManager.Instance.AchievedIds.TryGetValue(currentId, out var achieved) && achieved;
        bool isRewarded = AchievementManager.Instance.IsRewarded(currentId);

        group.rewardButton.gameObject.SetActive(isAchieved && !isRewarded);

        group.rewardButton.onClick.RemoveAllListeners();
        group.rewardButton.onClick.AddListener(() =>
        {
            Debug.Log("보상획득 클릭");
            if (!isAchieved || isRewarded) return;

            AchievementManager.Instance.Reward(info);
            group.currentIndex++;
            UpdateGroupUI(group);
        });
    }
}

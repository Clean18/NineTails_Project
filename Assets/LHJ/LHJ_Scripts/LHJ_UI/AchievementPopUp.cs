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

    // 업적 그룹을 인스펙터에서 설정
    [SerializeField] private List<AchievementGroupUI> achievementGroups;
    private void OnEnable()
    {
        foreach (var group in achievementGroups)
        {
            string groupKey = group.AchievementIds[0]; 
            if (AchievementManager.Instance.AchievementGroupIndex.TryGetValue(groupKey, out int savedIndex))
            {
                group.currentIndex = savedIndex;
            }
            else
            {
                group.currentIndex = 0;
            }
            UpdateGroupUI(group);
        }
    }

    // 각 업적 그룹 갱신
    private void UpdateGroupUI(AchievementGroupUI group)
    {
        // 모든 업적을 완료했을 때 버튼 숨김
        if (group.currentIndex >= group.AchievementIds.Count)
        {
            group.nameText.text = "모든 업적을 완료했습니다!";
            group.descriptionText.text = "";
            group.rewardButton.gameObject.SetActive(false);
            return;
        }

        // Id가 테이블에 없을경우
        if (!DataManager.Instance.AchievementTable.TryGetValue(group.AchievementIds[group.currentIndex], out var info))
        {
            group.rewardButton.gameObject.SetActive(false);
            return;
        }
        string groupKey = group.AchievementIds[0];

        // 업적 이름과 설명 UI 표시
        group.nameText.text = info.Name;
        group.descriptionText.text = info.Description;
        bool isAchieved = AchievementManager.Instance.IsAchieved(info.Id);
        bool isRewarded = AchievementManager.Instance.ReceivedReward(info.Id);
        group.rewardButton.gameObject.SetActive(isAchieved && !isRewarded);

        // 보상 버튼 클릭시 초기화 후 재등록
        group.rewardButton.onClick.RemoveAllListeners();
        group.rewardButton.onClick.AddListener(() =>
        {
            Debug.Log("보상획득 클릭");
            if (!AchievementManager.Instance.IsAchieved(info.Id)) return;

            AchievementManager.Instance.Reward(info);
            group.currentIndex++;

            // 현재 진행준인 인덱스 업데이트
            AchievementManager.Instance.AchievementGroupIndex[group.AchievementIds[0]] = group.currentIndex;
            UpdateGroupUI(group);
        });
    }
}

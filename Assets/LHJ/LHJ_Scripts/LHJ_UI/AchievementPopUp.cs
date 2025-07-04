using System.Collections;
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
        public int currentIndex = 0;            // 현재 진행 중인 업적 위치
    }

    // 업적 그룹을 인스펙터에서 설정
    [SerializeField] private List<AchievementGroupUI> achievementGroups;
    private void Start()
    {
        foreach (var group in achievementGroups)
        {
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

        // 업적 이름과 설명 UI 표시
        group.nameText.text = info.Name;
        group.descriptionText.text = info.Description;
        group.rewardButton.gameObject.SetActive(true);

        // 보상 버튼 클릭시 초기화 후 재등록
        group.rewardButton.onClick.RemoveAllListeners();
        group.rewardButton.onClick.AddListener(() =>
        {
            if (!AchievementManager.Instance.IsAchieved(info.Id)) return;

            AchievementManager.Instance.Reward(info);
            group.currentIndex++;   // 다음 업적 인덱스로 이동
            UpdateGroupUI(group);
        });
    }
}

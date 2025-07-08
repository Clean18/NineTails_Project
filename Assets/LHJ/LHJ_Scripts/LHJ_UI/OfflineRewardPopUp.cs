using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OfflineRewardPopUp : BaseUI
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI warmthText;
    [SerializeField] private TextMeshProUGUI spiritText;

    /// <summary>
    /// key = 업적 아이디, value = 스테이지 몬스터 레벨
    /// </summary>
    private Dictionary<string, int> _stageLevelDic = new()
    {
        { "A1", 1 },
        { "A2", 25 },
        { "A3", 60 },
        { "A4", 55 },
        { "A5", 78 },
        { "A6", 108 },
        { "A7", 100 },
        { "A8", 129 },
        { "A9", 147 },
    };

    private void Start()
    {
        GetEvent("Btn_close").Click += data => UIManager.Instance.ClosePopUp();
        OfflineRewardUI();
    }

    private void OfflineRewardUI()
    {
        int elapsedMinutes = SaveLoadManager.Instance.ElapsedMinutes;
        // 시, 분으로 변환
        int hours = elapsedMinutes / 60;
        int minutes = elapsedMinutes % 60;

        // 보상 계산
        string id = "A";
        int stageLevel = 1; // 스테이지 몬스터 레벨
        for (int i = 9; i > 0; i--)
        {
            // 보스 스테이지 예외처리용
            if ((i) % 3 == 0) continue;

            id = "A" + i;
            if (AchievementManager.Instance.AchievedIds.ContainsKey(id))
            {
                stageLevel = _stageLevelDic[id];
                break;
            }
        }
        Debug.Log($"오프라인 보상 {id} : 몬스터 레벨 {stageLevel}");

        long warmth = (long)(90 * System.Math.Pow(1.03, (stageLevel - 1) * minutes));
        long spirit = 0;
        if (stageLevel > 55 && PlayerController.Instance.GetFirstSpiritEnergy()) spirit = (long)(90 * System.Math.Pow(1.03, stageLevel - 55) * minutes);

        // 보상 지급
        PlayerController.Instance.AddCost(CostType.Warmth, warmth);
        PlayerController.Instance.AddCost(CostType.SpiritEnergy, spirit);

        // UI 표시
        timeText.text = $"방치한 시간\n{hours}시간 {minutes}분";
        warmthText.text = $"획득한 온정\n{warmth}";
        spiritText.text = $"획득한 영기\n{spirit}";

        SaveLoadManager.Instance.TimeInit();
    }
}

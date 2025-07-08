using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardPopUp : BaseUI
{
    [SerializeField] private TextMeshProUGUI warmthText;
    [SerializeField] private TextMeshProUGUI spiritText;

    private void Start()
    {
        GetEvent("Btn_close").Click += data =>
        {
            UIManager.Instance.ShowPopUp<AchievementPopUp>();
        };
    }
    public void Init(AchievementInfo achievementInfo)
    {
        warmthText.text = $"온정 +{achievementInfo.WarmthReward}";
        spiritText.text = $"영기 +{achievementInfo.SpritReward}";
    }
}

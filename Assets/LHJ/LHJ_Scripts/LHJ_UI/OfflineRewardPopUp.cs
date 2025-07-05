using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OfflineRewardPopUp : BaseUI
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI warmthText;
    [SerializeField] private TextMeshProUGUI spiritText;
    private void Start()
    {
        GetEvent("Btn_close").Click += data => UIManager.Instance.ClosePopUp();
        OfflineRewardUI();
    }

    private void OfflineRewardUI()
    {
        int elapsedSeconds = SaveLoadManager.Instance.ElapsedSeconds;
        // 시, 분으로 변환
        int hours = elapsedSeconds / 3600;
        int minutes = (elapsedSeconds % 3600) / 60;

        // UI 표시
        timeText.text = $"오프라인 시간: {hours}시간 {minutes}분";
        warmthText.text = $"획득 온정: -";
        spiritText.text = $"획득 영기: -";
    }
}

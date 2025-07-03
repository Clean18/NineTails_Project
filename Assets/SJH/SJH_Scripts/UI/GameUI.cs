using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class GameUI : SceneUI, IUI
{
	public TMP_Text AIStateText;
	public Button ControlModeBtn;

	void Start()
	{
		UIManager.Instance.GameUI = this;
		UIManager.Instance.SceneUIList.Add(this);
        Debug.Log($"GameUI 씬 UI 리스트에 추가 {UIManager.Instance.SceneUIList.Count}");
	}

	void OnEnable() => SubscribeEvent();
	void OnDisable() => UnsubscribeEvent();

	void SubscribeEvent()
	{
		ControlModeBtn?.onClick.AddListener(OnControlModeBtn);
	}

	void UnsubscribeEvent()
	{
        ControlModeBtn?.onClick.RemoveListener(OnControlModeBtn);
    }

	public void ChangeStateText(AIState state)
	{
		switch (state)
		{
			case AIState.Search:
				AIStateText.text = "Search...";
				break;
			case AIState.SkillLoad:
				AIStateText.text = "SkillLoad...";
				break;
			case AIState.Chase:
				AIStateText.text = "Chase...";
				break;
			case AIState.Attack:
				AIStateText.text = "Attack...";
				break;
		}
	}

	public void OnControlModeBtn()
	{
		if (GameManager.Instance.PlayerController == null) return;

		// 플레이어 모드 전환
		var player = GameManager.Instance.PlayerController;

		player.Mode = player.Mode == ControlMode.Auto ? ControlMode.Manual : ControlMode.Auto;

        player.AIInit();

		// Text On/Off
		UpdateAIMode(player.Mode);

        // 플레이어 velocity 초기화
        player.AIStop();
	}

	public void UpdateAIMode(ControlMode mode)
	{
		Debug.Log($"모드 변경 : {mode}");
		AIStateText.gameObject.SetActive(mode == ControlMode.Auto);
	}

	public void UIInit()
	{

	}

	public void OnBossStageBtn()
	{
        // 정보 저장
        // 보스씬 이동
        Debug.Log("보스씬 이동");
        SceneManager.LoadScene("Stage1-3");
	}
}

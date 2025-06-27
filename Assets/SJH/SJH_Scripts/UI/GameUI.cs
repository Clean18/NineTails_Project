using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : SceneUI, IUI
{
	public TMP_Text AIStateText;
	public Button ControlModeBtn;
	public Button Test_BossState;

	void Start()
	{
		UIManager.Instance.GameUI = this;
		UIManager.Instance.SceneUIList.Add(this);
	}

	void OnEnable() => SubscribeEvent();
	void OnDisable() => UnsubscribeEvent();

	void SubscribeEvent()
	{
		ControlModeBtn.onClick.AddListener(OnControlModeBtn);
		Test_BossState.onClick.AddListener(OnBossStageBtn);

	}

	void UnsubscribeEvent()
	{

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

		player.PlayerAI.MonsterSkillCheck();

		// Text On/Off
		UpdateAIMode(player.Mode);

		// 플레이어 velocity 초기화
		player.PlayerView.Move(Vector2.zero, 0);
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
	}
}

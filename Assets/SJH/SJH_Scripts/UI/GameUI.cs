using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameUI : SceneUI
{
    public TMP_Text AIStateText;
    public Button ControlModeBtn;

	void Start()
	{
		UIManager.Instance.GameUI = this;

		// TODO : 임시로 비활성화
		AIStateText.gameObject.SetActive(false);
	}

	void OnEnable() => SubscribeEvent();
	void OnDisable() => UnsubscribeEvent();

	void SubscribeEvent()
	{
		ControlModeBtn.onClick.AddListener(OnControlModeBtn);
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
}

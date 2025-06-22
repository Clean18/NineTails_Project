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
			case AIState.Idle:
				AIStateText.text = "Idle...";
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

		// 플레이어 velocity 초기화
		player.PlayerView.Move(Vector2.zero, 0);
	}

}

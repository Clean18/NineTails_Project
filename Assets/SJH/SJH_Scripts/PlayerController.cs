using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlMode { Manual, Auto }

public class PlayerController : MonoBehaviour
{
    private PlayerModel _playerModel;
	public PlayerView PlayerView;
	private PlayerAI _playerAI;

	public ControlMode Mode = ControlMode.Manual;

	void Awake()
	{
		_playerModel = new PlayerModel();
		PlayerView = GetComponent<PlayerView>();

		_playerAI = new PlayerAI(this, _playerModel, PlayerView);
	}

	void Update()
	{
		// Auto일 때는 입력 제한
		if (Mode == ControlMode.Auto)
		{
			_playerAI.Action();
			return;
		}

		// 수동 컨트롤
		PlayerView.Move(
			new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")), // 이동 방향
			_playerModel.PlayerData.MoveSpeed); // 이동속도
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlMode { Manual, Auto }
public enum AIState { Idle, Chase, Attack }

public class PlayerController : MonoBehaviour
{
    private PlayerModel _playerModel;
	public PlayerView PlayerView;

	// TODO : 게임이 시작되면 시작은 Auto
	private ControlMode _mode;
	public ControlMode Mode {
		get => _mode;
		set
		{
			_mode = value;
			Debug.Log(_mode);
			if (_mode == ControlMode.Manual) UIManager.Instance.GameUI?.AIStateText.gameObject.SetActive(false);
			else UIManager.Instance.GameUI?.AIStateText.gameObject.SetActive(true);
		}
	}

	// AI
	public AIState CurrentState;

	// Manual
	public Vector2 MoveDir { get; private set; }
	public List<KeyCode> SkillKeys = new()
	{
		KeyCode.Mouse0,
		KeyCode.Mouse1,
	};


	void Awake()
	{
		_playerModel = new PlayerModel();
		PlayerView = GetComponent<PlayerView>();

		CurrentState = AIState.Idle;
		Mode = ControlMode.Manual;

		GameManager.Instance.PlayerController = this;
	}

	void Update()
	{
		// Auto일 때는 입력 제한
		if (Mode == ControlMode.Auto) Action();

		// 수동 컨트롤
		else if (Mode == ControlMode.Manual) InputHandler();
	}

	public void Action()
	{
		switch (CurrentState)
		{
			// TODO : 행동방식은 기획에서 받기
			// 임시로 Chase <-> Idle <-> Attack
			case AIState.Idle:
				/* 타겟 탐색
				 * 타겟이 있으면 등록된 스킬 중 사정거리가 유효한 스킬이 있는지 체크
				 * if 스킬이 있음
				 * > Attack 으로 변경
				 * 
				 * else 스킬이 없음
				 * > Chase 으로 변경
				 * 
				 * 타겟이 없으면 유지
				 */
				break;
			case AIState.Chase:
				/* 추격
				 * 현재 맵에 몬스터가 없으면 Idle
				 * 몬스터가 있으면
				 * 공격 가능한지 체크
				 * 
				 */

				break;
			case AIState.Attack:
				/* 공격
				 * 
				 */
				break;
		}
		UIManager.Instance.GameUI?.ChangeStateText(CurrentState);
	}
	public void InputHandler()
	{
		MoveInput();
		// TODO : 사용하는 키 정보 필요 > WASD 이동만 공격은 자동으로
		SkillInput();
		MouseInput();
	}

	void MoveInput()
	{
		MoveDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
		PlayerView.Move(MoveDir, _playerModel.Data.MoveSpeed);
	}

	void SkillInput()
	{

	}

	void MouseInput()
	{
		// 특정 키를 누름
		// GetSkill에서 반환된게 null이 아니면 스킬 사용
		
		foreach (var key in SkillKeys)
		{
			if (Input.GetKeyDown(key))
			{
				var skill = GameManager.Instance.GetSkill("Fireball");
				if (skill != null)
				{
					Debug.Log("키 리스트에 있는 키 스킬사용");
					skill.UseSkill(transform);
				}
			}
		}
	}

}

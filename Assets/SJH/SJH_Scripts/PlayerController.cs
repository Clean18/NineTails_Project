using System.Collections.Generic;
using UnityEngine;

public enum ControlMode { Manual, Auto }
public enum AIState { Idle, Search, Chase, SkillLoad, Attack }

public class PlayerController : MonoBehaviour
{
	private PlayerModel _playerModel;
	public PlayerView PlayerView;
	public PlayerAI PlayerAI;

	// TODO : 게임이 시작되면 시작은 Auto
	private ControlMode _mode;
	public ControlMode Mode
	{
		get => _mode;
		set
		{
			_mode = value;
			// TODO : 자동모드가 되면 항상 Idle에서 시작
			if (CurrentState != AIState.Idle) CurrentState = AIState.Idle;
		}
	}

	// AI
	public AIState CurrentState;
	public Transform TargetMonster;
	public SkillData TargetSkill;

	// Manual
	public Vector2 MoveDir { get; private set; }
	public List<KeyCode> SkillKeys = new()
	{
		KeyCode.Mouse0,
		KeyCode.Mouse1,
	};


	void Awake()
	{
		// TODO : Awake가 아니라 데이터 로드할때 초기화
		_playerModel = new PlayerModel();

		PlayerView = GetComponent<PlayerView>();
		PlayerAI = new PlayerAI(this, PlayerView, _playerModel);

		CurrentState = AIState.Idle;
		Mode = ControlMode.Manual;

		GameManager.Instance.PlayerController = this;
	}

	void Update()
	{
		// Auto일 때는 입력 제한
		if (Mode == ControlMode.Auto) PlayerAI.Action();

		// 수동 컨트롤
		else if (Mode == ControlMode.Manual) InputHandler();
	}

	public void InputHandler()
	{
		MoveInput();
		// TODO : 사용하는 키 정보 필요 > WASD 이동만 공격은 자동으로
		SkillInput();
	}

	void MoveInput()
	{
		MoveDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
		PlayerView.Move(MoveDir, _playerModel.Data.MoveSpeed);
	}

	void SkillInput()
	{
		foreach (var key in SkillKeys)
		{
			if (Input.GetKeyDown(key))
			{
				var skill = GameManager.Instance.GetSkill("Fireball");
				if (skill != null)
				{
					skill.UseSkill(transform);
				}
			}
		}
	}

	[SerializeField] private float _attackDistance = 8;
	[SerializeField] private int _directionCount = 8;
	[SerializeField] private float _sightAngle = 45f;
	[SerializeField] private LayerMask _monsterLayer;

	void OnDrawGizmos()
	{
		for (int i = 0; i < _directionCount; i++)
		{
			float angle = _sightAngle * i;
			Vector3 dir = Quaternion.Euler(0, 0, angle) * transform.up;

			Gizmos.DrawLine(transform.position, transform.position + dir * _attackDistance);
		}
	}
}

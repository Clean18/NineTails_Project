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
	public GameObject TargetMonster;
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
				IdleAction();
				break;
			case AIState.Chase:
				ChaseAction();
				break;
			case AIState.Attack:
				AttackAction();
				break;
		}
		UIManager.Instance.GameUI?.ChangeStateText(CurrentState);
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
		// 특정 키를 누름
		// GetSkill에서 반환된게 null이 아니면 스킬 사용

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

	void IdleAction()
	{
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
		Debug.Log("Idle Action");
		var target = GameManager.Instance.Spawner.FindCloseMonster(transform.position);
		if (target != null)
		{
			Debug.Log("Idle Action : 공격대상 확인 추격 전환");
			TargetMonster = target;
			CurrentState = AIState.Chase;
		}
	}

	void ChaseAction()
	{
		Debug.Log("Chase Action");
		/* 추격
		 * 현재 맵에 몬스터가 없으면 Idle
		 * 몬스터가 있으면
		 * 공격 가능한지 체크
		 */
		if (TargetMonster == null || !TargetMonster.activeSelf)
		{
			Debug.Log("Chase Action : 공격대상 사라짐 대기 전환");
			TargetMonster = null;
			CurrentState = AIState.Idle;
			return;
		}

		float distance = Vector3.Distance(transform.position, TargetMonster.transform.position);

		// 쿨타임이 끝난 스킬 중에서 사거리 내에 있는 것 찾기
		foreach (var skill in GameManager.Instance.SkillDic.Values)
		{
			if (!skill.IsCooldown && distance <= skill.Range)
			{
				Debug.Log($"Chase Action : {skill.SkillName} 장전 공격 전환");
				TargetSkill = skill;
				CurrentState = AIState.Attack;
				return;
			}
		}

		// 아직 사거리 안이 아니면 계속 추격
		Debug.Log("Chase Action : 사거리 멀음 재추격 시작 ");
		Vector2 dir = (TargetMonster.transform.position - transform.position).normalized;
		PlayerView.Move(dir, _playerModel.Data.MoveSpeed);
	}

	void AttackAction()
	{
		Debug.Log("Attack Action");
		if (TargetMonster == null || !TargetMonster.activeSelf)
		{
			Debug.Log("Attack Action : 공격대상 사라짐 대기 전환");
			TargetMonster = null;
			TargetSkill = null;
			CurrentState = AIState.Idle;
			return;
		}

		if (TargetSkill == null || TargetSkill.IsCooldown)
		{
			Debug.Log("Attack Action : 공격대상 사라짐 or 장전스킬 쿨타임");
			CurrentState = AIState.Chase;
			return;
		}

		// 사거리 벗어나면 다시 추격
		float distance = Vector3.Distance(transform.position, TargetMonster.transform.position);
		if (distance > TargetSkill.Range)
		{
			Debug.Log("Attack Action : 공격 스킬 사거리 멀음 추격 전환");
			CurrentState = AIState.Chase;
			return;
		}

		// 스킬 사용
		Debug.Log($"Attack Action : {TargetSkill.SkillName} 스킬 사용");
		TargetSkill.UseSkill(transform, TargetMonster.transform);
		TargetSkill = null;

		// 다음 행동은 상황 따라 다시 판단
		Debug.Log("Attack Action : 공격 스킬 완료 대기 전환");
		CurrentState = AIState.Idle;
	}
}

using UnityEngine;

public class PlayerAI
{
	private PlayerController _controller;
	private PlayerView _view;
	private PlayerModel _model;

	public Transform TargetMonster;
	public SkillData TargetSkill;

	public PlayerAI(PlayerController controller, PlayerView view, PlayerModel model)
	{
		_controller = controller;
		_view = view;
		_model = model;
	}

	public void Action()
	{
		switch (_controller.CurrentState)
		{
			// TODO : 행동방식은 기획에서 받기
			// 임시로 Chase <-> Idle <-> Attack
			case AIState.Idle:
				IdleAction();
				break;
			case AIState.Search:
				break;
			case AIState.Chase:
				ChaseAction();
				break;
			case AIState.SkillLoad:
				break;
			case AIState.Attack:
				AttackAction();
				break;
		}
		UIManager.Instance.GameUI?.ChangeStateText(_controller.CurrentState);

	}

	void IdleAction()
	{
		Debug.Log("Idle Action");
		var target = GameManager.Instance.Spawner.FindCloseMonster(_controller.transform.position);
		if (target != null)
		{
			Debug.Log("Idle Action : 공격대상 확인 추격 전환");
			TargetMonster = target.transform;
			_controller.CurrentState = AIState.Chase;
		}
	}

	void ChaseAction()
	{
		Debug.Log("Chase Action");
		if (TargetMonster == null || !TargetMonster.gameObject.activeSelf)
		{
			Debug.Log("Chase Action : 공격대상 사라짐 대기 전환");
			TargetMonster = null;
			_controller.CurrentState = AIState.Idle;
			return;
		}

		float distance = Vector3.Distance(_controller.transform.position, TargetMonster.transform.position);

		// 쿨타임이 끝난 스킬 중에서 사거리 내에 있는 것 찾기
		foreach (var skill in GameManager.Instance.SkillDic.Values)
		{
			if (!skill.IsCooldown && distance <= skill.Range)
			{
				Debug.Log($"Chase Action : {skill.SkillName} 장전 공격 전환");
				TargetSkill = skill;
				_controller.CurrentState = AIState.Attack;
				return;
			}
		}

		// 아직 사거리 안이 아니면 계속 추격
		Debug.Log("Chase Action : 사거리 멀음 재추격 시작 ");
		Vector2 dir = (TargetMonster.transform.position - _controller.transform.position).normalized;
		_view.Move(dir, _model.Data.MoveSpeed);
	}

	void AttackAction()
	{
		Debug.Log("Attack Action");
		if (TargetMonster == null || !TargetMonster.gameObject.activeSelf)
		{
			Debug.Log("Attack Action : 공격대상 사라짐 대기 전환");
			TargetMonster = null;
			TargetSkill = null;
			_controller.CurrentState = AIState.Idle;
			return;
		}

		if (TargetSkill == null || TargetSkill.IsCooldown)
		{
			Debug.Log("Attack Action : 공격대상 사라짐 or 장전스킬 쿨타임");
			_controller.CurrentState = AIState.Chase;
			return;
		}

		// 사거리 벗어나면 다시 추격
		float distance = Vector3.Distance(_controller.transform.position, TargetMonster.transform.position);
		if (distance > TargetSkill.Range)
		{
			Debug.Log("Attack Action : 공격 스킬 사거리 멀음 추격 전환");
			_controller.CurrentState = AIState.Chase;
			return;
		}

		// 스킬 사용
		Debug.Log($"Attack Action : {TargetSkill.SkillName} 스킬 사용");
		TargetSkill.UseSkill(_controller.transform, TargetMonster.transform);
		TargetSkill = null;

		// 다음 행동은 상황 따라 다시 판단
		Debug.Log("Attack Action : 공격 스킬 완료 대기 전환");
		_controller.CurrentState = AIState.Idle;
	}

	[SerializeField] private float _attackDistance = 8;
	[SerializeField] private int _directionCount = 8;
	[SerializeField] private float _sightAngle = 45f;
	[SerializeField] private LayerMask _monsterLayer;
}

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
			case AIState.Search:
				SearchAction();
				break;
			case AIState.SkillLoad:
				SkillLoad();
				break;
			case AIState.Chase:
				ChaseAction();
				break;
			case AIState.Attack:
				AttackAction();
				break;
		}
		UIManager.Instance.GameUI?.ChangeStateText(_controller.CurrentState);
	}

	void SearchAction()
	{
		Debug.Log("Search Action");

		// 적 탐색
		Collider2D[] monsters = Physics2D.OverlapCircleAll(_controller.transform.position, _controller.SearchDistance, _controller.MonsterLayer);
		// 적이 없으면 대기
		if (monsters.Length == 0) return;

		Dictionary<int, List<Transform>> searchDic = new();
		for (int i = 1; i <= _controller.DirectionCount; i++)
			searchDic[i] = new List<Transform>();

		// 8방향 탐색
		foreach (var collider in monsters)
		{
			Vector2 dir = (Vector2)(collider.transform.position - _controller.transform.position).normalized;
			float angle = Vector2.SignedAngle(Vector2.up, dir);
			if (angle < 0) angle += 360;
			int sector = (int)(angle / _controller.SightAngle) + 1;
			searchDic[sector].Add(collider.transform);
		}

		// 우선순위에 따라 공격 방향 선택
		// 1순위 몬스터수
		// 2순위 원거리 타입 몬스터
		
		// 몬스터가 제일 많은 칸 찾기
		int targetSector = 1;
		int maxCount = 0;
		foreach (var keyValue in searchDic)
		{
			if (keyValue.Value.Count > maxCount)
			{
				maxCount = keyValue.Value.Count;
				targetSector = keyValue.Key;
			}
		}
		Debug.Log($"타겟 칸 : {targetSector} / 몬스터 수 : {maxCount}");

		// TODO : 제일 많은 칸에서 몬스터 구분
		if (searchDic[targetSector].Count == 0) return;

		TargetMonster = null;
		int ran = Random.Range(0, searchDic[targetSector].Count);
		TargetMonster = searchDic[targetSector][ran];

		// 선택된 방향 중 몬스터 타겟 선택
		if (TargetMonster == null) return;
		_controller.CurrentState = AIState.SkillLoad;
	}

	void SkillLoad()
	{
		Debug.Log("SkillLoad Action");

		// 모든 스킬이 쿨타임이면 Search로 이동
		TargetSkill = null;
		float maxCooldown = float.MinValue;
		foreach (var skill in GameManager.Instance.SkillDic.Values)
		{
			// 스킬 쿨타임이 아니고 쿨타임이 가장 긴 스킬 우선
			if (!skill.IsCooldown && skill.Cooldown > maxCooldown)
			{
				maxCooldown = skill.Cooldown;
				TargetSkill = skill;
				Debug.Log($"{TargetSkill.SkillName} 장전");
			}
		}
		if (TargetSkill == null) _controller.CurrentState = AIState.Search;
		// 사용 가능한 스킬을 TargetSkill 에 등록 후 Chase로 변경
		_controller.CurrentState = AIState.Chase;
	}

	void ChaseAction()
	{
		Debug.Log("Chase Action");
		if (MonsterSkillCheck()) return;

		// 공격 스킬의 범위가 공격 대상을 공격할 수 있으면 SKill로
		Vector3 dir = TargetMonster.position - _controller.transform.position;
		float distance = dir.magnitude;
		Debug.Log($"거리 {distance}");
		if (distance <= TargetSkill.Range)
		{
			_view.Stop();
			_controller.CurrentState = AIState.Attack;
			return;
		}

		// 공격할 수 없으면 공격거리까지 이동 후 Skill로
		_view.Move(dir.normalized, _model.Data.MoveSpeed);
	}

	void AttackAction()
	{
		Debug.Log("Attack Action");
		if (MonsterSkillCheck()) return;

		// 사거리 벗어나면 다시 추격
		float distance = (TargetMonster.position - _controller.transform.position).magnitude;
		if (distance > TargetSkill.Range)
		{
			Debug.Log("Attack Action : 공격 스킬 사거리 멀음 추격 전환");
			_controller.CurrentState = AIState.Chase;
			return;
		}

		// 스킬 사용
		Debug.Log($"Attack Action : {TargetSkill.SkillName} 스킬 사용");
		TargetSkill.UseSkill(_controller.transform, TargetMonster.transform);
		TargetMonster = null;
		TargetSkill = null;

		// 다음 행동은 상황 따라 다시 판단
		Debug.Log("Attack Action : 공격 스킬 완료 탐색 전환");
		_controller.CurrentState = AIState.Search;
	}

	/// <summary>
	/// 공격 대상, 공격 대상의 활성화 상태, 공격 스킬, 공격 스킬의 쿨타임을 체크하여 bool 값을 반환하는 함수
	/// </summary>
	/// <returns>true : 공격불가, false : 공격 가능</returns>
	bool MonsterSkillCheck()
	{
		bool result = false;
		if (TargetMonster == null || !TargetMonster.gameObject.activeSelf || TargetSkill == null || TargetSkill.IsCooldown)
		{
			TargetMonster = null;
			TargetSkill = null;
			_controller.CurrentState = AIState.Search;
			result = true;
			return result;
		}
		return result;
	}
}

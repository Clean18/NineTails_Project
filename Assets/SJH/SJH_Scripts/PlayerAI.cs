using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAI
{
	private PlayerController _controller;
	private PlayerView _view;
	private PlayerModel _model;

	private Transform _targetMonster; // 공격할 몬스터
    private ISkill _targetSkill;      // 사용할 스킬

    private Coroutine _searchRoutine;
	private WaitForSeconds _searchDelay;

	[SerializeField] Collider2D[] _monsterTable;

	public PlayerAI(PlayerController controller, PlayerView view, PlayerModel model)
	{
		_controller = controller;
		_view = view;
		_model = model;
		_searchDelay = new WaitForSeconds(0.5f);
        _monsterTable = new Collider2D[400];
    }

    public void AIInit()
    {
        StopSearchRoutine();
        MonsterSkillCheck();
    }

	public void Action()
	{
		if (SkillLogic.IsSkillUsed) return;

		switch (_controller.CurrentState)
		{
			case AIState.Search: SearchAction(); break;
			case AIState.SkillLoad: SkillLoad(); break;
			case AIState.Chase: ChaseAction(); break;
			case AIState.Attack: AttackAction(); break;
		}
		UIManager.Instance.GameUI?.ChangeStateText(_controller.CurrentState);
	}

	void SearchAction()
	{
		if (_searchRoutine == null) _searchRoutine = _controller.StartCoroutine(SearchRoutine());
	}

	void SkillLoad()
	{
		_targetSkill = null;

		List<ISkill> ranSkills = new();
		// 기본공격은 이 리스트에 없어야함
		// -> 모든 스킬이 쿨타임일 때 사용할 예정 
		foreach (var skill in _model.Skill.GetSkillMappingList())
		{
			// 쿨타임이 아닌 스킬 등록
			if (skill != null && !skill.IsCooldown) ranSkills.Add(skill);
		}
		// 쿨타임이 아닌 스킬들 중 랜덤 사용
		if (ranSkills.Count > 0)
		{
			_targetSkill = ranSkills[Random.Range(0, ranSkills.Count)];
		}
		else
		{
			_targetSkill = _model.Skill.DefaultAttack;
		}
		Debug.Log($"{_targetSkill.SkillData.SkillName} 스킬장전");

		// 사용 가능한 스킬을 TargetSkill 에 등록 후 Chase로 변경
		if (_targetSkill != null) _controller.CurrentState = AIState.Chase;
		// 스킬이 쿨타임일 때는 TargetMonster도 초기화해야 SearchRoutine에서 안걸림
		else MonsterSkillCheck();
	}

	void ChaseAction()
	{
		//Debug.Log("Chase Action");
		if (MonsterSkillCheck()) return;

		// 공격 스킬의 범위가 공격 대상을 공격할 수 있으면 SKill로
		Vector3 dir = _targetMonster.position - _controller.transform.position;
		float distance = dir.magnitude;
		if (!MonsterSkillCheck() && distance <= _targetSkill.SkillData.Range || _targetSkill.SkillData.Range == 0)
		{
			_view.AIStop();
			StopSearchRoutine();
			_controller.CurrentState = AIState.Attack;
			Debug.Log("사거리 안 공격시작");
			return;
		}

		// 공격할 수 없으면 공격거리까지 이동 후 Skill로
		_view.Move(dir.normalized, _model.Data.Speed);
		if (_searchRoutine == null) _searchRoutine = _controller.StartCoroutine(SearchRoutine());
	}

	void AttackAction()
	{
		//Debug.Log("Attack Action");
		if (MonsterSkillCheck()) return;

		// 사거리 벗어나면 다시 추격

		float distance = (_targetMonster.position - _controller.transform.position).magnitude;
		if (distance > _targetSkill.SkillData.Range && _targetSkill.SkillData.Range != 0)
		{
			Debug.Log("Attack Action : 공격 스킬 사거리 멀음 추격 전환");
			_controller.CurrentState = AIState.Chase;
			return;
		}

		// 스킬 사용
		Debug.Log($"Attack Action : {_targetSkill.SkillData.SkillName} 스킬 사용");
		// 방향전환
		float dirX = _targetMonster.position.x - _controller.transform.position.x;
		_controller.View.PlayerFlip(dirX);

		if (_targetSkill.UseSkill(_controller.transform, _targetMonster.transform)) SkillButton.Instance.UpdateCooldown(_targetSkill.SlotIndex);
		_targetMonster = null;
		_targetSkill = null;

		// 다음 행동은 상황 따라 다시 판단
		Debug.Log("Attack Action : 공격 스킬 완료 탐색 전환");
		_controller.CurrentState = AIState.Search;
	}

	/// <summary>
	/// 공격 대상, 공격 대상의 활성화 상태, 공격 스킬, 공격 스킬의 쿨타임을 체크하여 bool 값을 반환하는 함수
	/// </summary>
	/// <returns>true : 공격불가, false : 공격 가능</returns>
	public bool MonsterSkillCheck()
	{
		bool result = false;
		if (_targetMonster == null || !_targetMonster.gameObject.activeSelf || _targetSkill == null || _targetSkill.IsCooldown)
		{
			Debug.Log($"초기화 {_controller.CurrentState}");
			_targetMonster = null;
			_targetSkill = null;
			_controller.CurrentState = AIState.Search;
			result = true;
			return result;
		}
		return result;
	}

	IEnumerator SearchRoutine()
	{
		while ((_controller.Mode == ControlMode.Auto && _controller.CurrentState == AIState.Search)
			|| (_controller.Mode == ControlMode.Auto && _controller.CurrentState == AIState.Chase))
		{
			yield return _searchDelay;

			if (_targetMonster != null) continue;

			// 1. 몬스터 탐색
			//var monsters = Physics2D.OverlapCircleAll(_controller.transform.position, _controller.SearchDistance, _controller.MonsterLayer);
			int monsterCount = Physics2D.OverlapCircleNonAlloc(_controller.transform.position, _controller.SearchDistance, _monsterTable, _controller.MonsterLayer);
			if (monsterCount == 0)
			{
				// 범위에 몬스터가 없으면 이동 정지
				_view.AIStop();
				continue;
			}

			// 2. 섹터 분류 준비
			Debug.Log("Check 4");
			// 원 안의 몬스터들을 8칸으로 분류
			int dirCount = _controller.DirectionCount;
			var searchList = new List<Transform>[dirCount];
			for (int i = 0; i < dirCount; i++)
				searchList[i] = new List<Transform>(); // 1 ~ 8

			// 3. 섹터 분류
			// 1 base에서 0 base로 변경
			Vector2 playerPos = _controller.transform.position;
			float sightAngle = _controller.SightAngle;

			for (int i = 0; i < monsterCount; i++)
			{
				var col = _monsterTable[i];
				if (col == null) continue;

				Vector2 monDir = ((Vector2)col.transform.position - playerPos).normalized;
				float angle = Vector2.SignedAngle(Vector2.up, monDir);
				if (angle < 0) angle += 360;

				int sector = (int)(angle / sightAngle);
				if (sector < 0) sector = 0;
				else if (sector >= dirCount) sector = dirCount - 1;

				searchList[sector].Add(col.transform);
			}

			// 4. 몬스터가 가장 많은 섹터들 선택
			List<int> monsterSectors = new();
			int maxCount = 0;
			for (int i = 0; i < dirCount; i++)
			{
				int count = searchList[i].Count; // 0 ~ 7
				if (count > maxCount)                    // 현재 섹터의 몬스터가 가장 많다면
				{
					maxCount = count;                    // maxCount 갱신
					monsterSectors.Clear();              // 이전의 섹터 정보들 삭제
					monsterSectors.Add(i);               // 현재 섹터 정보 추가
				}
				else if (count == maxCount && count > 0) // 몬스터가 1마리 이상이고 maxCount와 같으면 섹터 리스트에 추가
				{
					monsterSectors.Add(i);
				}
			}
			if (monsterSectors.Count == 0) continue;

			// 몬스터가 가장 많은 섹터들 중 거리합이 가장 낮은 섹터 선택
			// 최종적으로 선별할 변수들
			int targetSector = -1;
			float bestDistance = float.MaxValue; // 거리 비교용 값
			Transform nearestMonster = null;     // 최종 타겟 변수

			foreach (int sector in monsterSectors)
			{
				float currentDistance = 0f; // 현재 섹터별 값
				float minDistance = float.MaxValue;
				Transform nearMonster = null;

				foreach (var mon in searchList[sector])
				{
					float distance = ((Vector2)mon.position - playerPos).sqrMagnitude; // 현재 섹터의 몬스터들과 플레이어의 거리합산

					bool isRanged = mon.TryGetComponent(out IDamagable dmg) && dmg.Type == MonsterType.Ranged;

					if (isRanged) distance *= (1f / 9f); // squrMagnitude를 사용했기에 1/9로 3분의 1을 적용

                    currentDistance += distance;

                    // 현재 섹터 내 가장 가까운 몬스터 저장
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearMonster = mon;
                    }
				}

				if (currentDistance < bestDistance) // 현재 섹터의 거리합이 이전 섹터의 거리합보다 낮으면 변경
				{
					bestDistance = currentDistance;
					targetSector = sector;
                    nearestMonster = nearMonster;
				}
			}

			if (nearestMonster == null) continue;

            // 타겟 확정시 상태 변경
            _targetMonster = nearestMonster;
            _controller.CurrentState = AIState.SkillLoad;
            StopSearchRoutine();
            yield break;
		}
	}

	public void StopSearchRoutine()
	{
		if (_searchRoutine != null)
		{
			_controller.StopCoroutine(_searchRoutine);
			_searchRoutine = null;
		}
	}
}

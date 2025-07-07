using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAI
{
	private PlayerController _controller;
	private PlayerView _view;
	private PlayerModel _model;

	public Transform TargetMonster; // 공격할 몬스터
    private ISkill _targetSkill;
	public ISkill TargetSkill      // 사용할 스킬
    {
        get => _targetSkill;
        set
        {
            _targetSkill = value;
            if (value != null) _targetSkillName = _targetSkill.SkillData.SkillName;
        }
    }
    [SerializeField] private string _targetSkillName;

	private Coroutine _searchRoutine;
	private WaitForSeconds _searchDelay;

	public PlayerAI(PlayerController controller, PlayerView view, PlayerModel model)
	{
		_controller = controller;
		_view = view;
		_model = model;
		_searchDelay = new WaitForSeconds(0.5f);
	}

	public void Action()
	{
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
		//Debug.Log("SkillLoad Action");

		TargetSkill = null;
        //float maxCooldown = float.MinValue;

        List<ISkill> ranSkills = new();
        // 기본공격은 이 리스트에 없어야함
        // -> 모든 스킬이 쿨타임일 때 사용할 예정 
        foreach (var skill in _model.GetSkillMappingList())
		{
            // 쿨타임이 아닌 스킬 등록
            if (skill != null && !skill.IsCooldown) ranSkills.Add(skill);
        }
        // 쿨타임이 아닌 스킬들 중 랜덤 사용
        if (ranSkills.Count > 0)
        {
            TargetSkill = ranSkills[Random.Range(0, ranSkills.Count)];
        }
        else
        {
            TargetSkill = _model.Skill.DefaultAttack;
        }
        Debug.Log($"{TargetSkill.SkillData.SkillName} 스킬장전");

        // 사용 가능한 스킬을 TargetSkill 에 등록 후 Chase로 변경
        if (TargetSkill != null) _controller.CurrentState = AIState.Chase;
        // 스킬이 쿨타임일 때는 TargetMonster도 초기화해야 SearchRoutine에서 안걸림
        else MonsterSkillCheck();
    }

	void ChaseAction()
	{
		//Debug.Log("Chase Action");
		if (MonsterSkillCheck()) return;

		// 공격 스킬의 범위가 공격 대상을 공격할 수 있으면 SKill로
		Vector3 dir = TargetMonster.position - _controller.transform.position;
		float distance = dir.magnitude;
		if (!MonsterSkillCheck() && distance <= TargetSkill.SkillData.Range || TargetSkill.SkillData.Range == 0)
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
		float distance = (TargetMonster.position - _controller.transform.position).magnitude;
		if (distance > TargetSkill.SkillData.Range && TargetSkill.SkillData.Range != 0)
		{
			Debug.Log("Attack Action : 공격 스킬 사거리 멀음 추격 전환");
			_controller.CurrentState = AIState.Chase;
			return;
		}

		// 스킬 사용
		Debug.Log($"Attack Action : {TargetSkill.SkillData.SkillName} 스킬 사용");
		TargetSkill.UseSkill(_controller.transform, TargetMonster.transform);
        SkillButton.Instance.UpdateCooldown(TargetSkill.SlotIndex);
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
	public bool MonsterSkillCheck()
	{
		bool result = false;
		if (TargetMonster == null || !TargetMonster.gameObject.activeSelf || TargetSkill == null || TargetSkill.IsCooldown)
		{
			Debug.Log($"초기화 {_controller.CurrentState}");
			TargetMonster = null;
			TargetSkill = null;
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

			if (TargetMonster != null) continue;

			var monsters = Physics2D.OverlapCircleAll(_controller.transform.position, _controller.SearchDistance, _controller.MonsterLayer);
            if (monsters.Length == 0)
            {
                // 범위에 몬스터가 없으면 이동 정지
                _view.AIStop();
                continue;
            }

			// 원 안의 몬스터들을 8칸으로 분류
			Dictionary<int, List<Transform>> searchDic = new();
			for (int i = 1; i <= _controller.DirectionCount; i++)
				searchDic[i] = new List<Transform>(); // 1 ~ 8

			foreach (var collider in monsters)
			{
				Vector2 dir = (collider.transform.position - _controller.transform.position).normalized;
				float angle = Vector2.SignedAngle(Vector2.up, dir);
				if (angle < 0) angle += 360;
				int sector = (int)(angle / _controller.SightAngle) + 1;
				searchDic[sector].Add(collider.transform);
			}

			// 몬스터가 가장 많은 섹터들 선택
			List<int> monsterSectors = new();
			int maxCount = 0;
			for (int i = 1; i <= _controller.DirectionCount; i++)
			{
				int count = searchDic[i].Count; // 1 ~ 8
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
			int targetSector = monsterSectors[0];
			float prevDistance = float.MaxValue;
			Vector3 playerPos = _controller.transform.position;

			foreach (int sector in monsterSectors)
			{
				float currentDistance = 0f;
				foreach (var mon in searchDic[sector])
				{
					currentDistance += (mon.position - playerPos).magnitude; // 현재 섹터의 몬스터들과 플레이어의 거리합산
				}

				if (currentDistance < prevDistance) // 현재 섹터의 거리합이 이전 섹터의 거리합보다 낮으면 변경
				{
					prevDistance = currentDistance;
					targetSector = sector;
				}
			}

			if (searchDic[targetSector].Count == 0) continue;

			// 2. 선택된 칸 중 가까운 몬스터 선택
			float minDistance = float.MaxValue;

			foreach (var monster in searchDic[targetSector])
			{
				float distance = (monster.position - playerPos).magnitude;
				if (distance < minDistance)
				{
					minDistance = distance;
					TargetMonster = monster;
				}
			}

			if (TargetMonster != null)
			{
				Debug.Log("SkillLoad로 변경");
				_controller.CurrentState = AIState.SkillLoad;
				StopSearchRoutine();
				yield break;
			}
		}
	}

	void StopSearchRoutine()
	{
		if (_searchRoutine != null)
		{
			_controller.StopCoroutine(_searchRoutine);
			_searchRoutine = null;
		}
	}
}

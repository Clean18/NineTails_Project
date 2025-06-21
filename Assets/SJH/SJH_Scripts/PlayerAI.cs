using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIState { Idle, Chase, Attack }

public class PlayerAI
{
	// 플레이어 AI
	private PlayerController _controller;
	private PlayerModel _model;
	private PlayerView _view;

	private AIState _currentState = AIState.Idle;

	public PlayerAI(PlayerController controller, PlayerModel model, PlayerView view)
	{
		_controller = controller;
		_model = model;
		_view = view;
	}

	public void Action()
	{
		switch (_currentState)
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
				/* 몬스터 추격
				 * 현재 맵에 몬스터가 없으면 Idle
				 * 몬스터가 있으면
				 * 공격 가능한지 체크
				 * 
				 */

				break;
			case AIState.Attack:

				break;
		}
	}
}

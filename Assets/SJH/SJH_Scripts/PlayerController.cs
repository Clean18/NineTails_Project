using System.Collections.Generic;
using UnityEngine;

#region enum ControlMode
/// <summary>
/// 플레이어의 조작 방식
/// </summary>
public enum ControlMode
{
	/// <summary>
	/// 수동 조작 : 이동, 공격 전부 플레이어가 조작
	/// </summary>
	Manual,
	/// <summary>
	/// 자동 조작 : 이동, 공격 전부 AI에서 처리
	/// </summary>
	Auto
}
#endregion

#region enum AIState
/// <summary>
/// 자동 모드에서의 플레이어 AI 상태
/// </summary>
public enum AIState
{
    Idle,
	/// <summary>
	/// 적을 탐색 중인 상태
	/// </summary>
	Search,

	/// <summary>
	/// 탐지한 적을 향해 추격 중인 상태
	/// </summary>
	Chase,

	/// <summary>
	/// 사용할 스킬을 선택하는 상태
	/// </summary>
	SkillLoad,

	/// <summary>
	/// 적을 공격하는 상태
	/// </summary>
	Attack
}
#endregion

/// <summary>
/// 클라이언트의 입력을 관리하는 컴포넌트
/// </summary>
public class PlayerController : MonoBehaviour
{
	private PlayerModel _playerModel;
	public PlayerView PlayerView;
	public PlayerAI PlayerAI;

	// TODO : 게임이 시작되면 시작은 Auto
	[SerializeField]private ControlMode _mode;
	public ControlMode Mode
	{
		get => _mode;
		set
		{
			_mode = value;
			// TODO : 자동모드가 되면 항상 Search에서 시작
			Debug.Log($"AIState : {CurrentState} > {value}");
			if (CurrentState != AIState.Search) CurrentState = AIState.Search;
		}
	}

	// AI 에서 사용하는 필드변수
	public AIState CurrentState;		// AI 상태
	public float SearchDistance = 8;	// 탐색 거리
	public int DirectionCount = 8;		// 탐색할 칸의 개수 360 / 8
	public float SightAngle = 45f;		// 칸마다 각도
	public LayerMask MonsterLayer;		// 탐색할 레이어

	// Manual 에서 사용하는 필드변수
	public Vector2 MoveDir { get; private set; } // 플레이어의 이동 방향


	void Awake()
	{
		// TODO : Awake가 아니라 데이터 로드할때 초기화
		_playerModel = new PlayerModel();

		PlayerView = GetComponent<PlayerView>();
		PlayerAI = new PlayerAI(this, PlayerView, _playerModel);

		CurrentState = AIState.Search;
		//Mode = ControlMode.Manual;
		Mode = ControlMode.Auto;
		
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
        // TODO : 키세팅
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			var skill = GameManager.Instance.GetSkill("Fireball");
			if (skill != null)
			{
				skill.UseSkill(transform);
			}
		}
	}

	public void TakeDamage(long damage)
	{
		_playerModel.ApplyDamage(damage);
		// TODO : view 피격처리
		// TODO : UI 체력감소 처리
	}

	void OnDrawGizmos()
	{
		// 플레이어의 공격 범위 기즈모
		// 팔각
		for (int i = 0; i < DirectionCount; i++)
		{
			float angle = SightAngle * i;
			Vector3 dir = Quaternion.Euler(0, 0, angle) * transform.up;
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, transform.position + dir * SearchDistance);
		}
		// 원
		Gizmos.DrawWireSphere(transform.position, SearchDistance);
	}
}

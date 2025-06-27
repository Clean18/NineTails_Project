using System.Collections;
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
    [Tooltip("플레이어 데이터 로드 여부")]
    [SerializeField ]private bool _isInit = false;

	public PlayerModel PlayerModel;
    public PlayerView PlayerView;
    public PlayerAI PlayerAI;

    // TODO : 게임이 시작되면 시작은 Auto
    [Header("컨트롤 모드 Auto/Manual")]
	[SerializeField] private ControlMode _mode;
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
    [Header("AI 필드변수")]
	public AIState CurrentState;		// AI 상태
	public float SearchDistance = 8;	// 탐색 거리
	public int DirectionCount = 8;		// 탐색할 칸의 개수 360 / 8
	public float SightAngle = 45f;		// 칸마다 각도
	public LayerMask MonsterLayer;      // 탐색할 레이어

    // Manual 에서 사용하는 필드변수
    [Header("수동모드 필드변수")]
    public Vector2 MoveDir; // 플레이어의 이동 방향

    void Start()
    {
        // 시작은 자동모드
        CurrentState = AIState.Search;
        Mode = ControlMode.Auto;
        //Mode = ControlMode.Manual;

        StartCoroutine(PlayerInit());
    }

    void Update()
	{
        if (_isInit == false) return;

		// Auto일 때는 입력 제한
		if (Mode == ControlMode.Auto) PlayerAI.Action();

		// 수동 컨트롤
		else if (Mode == ControlMode.Manual) InputHandler();

        // TODO : TEST 인풋
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Test_ChangeStat();
        }
    }

	public void InputHandler()
	{
		MoveInput();
		SkillInput();
	}

	void MoveInput()
	{
		MoveDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
		PlayerView.Move(MoveDir, PlayerModel.Data.Speed);
	}

	void SkillInput()
	{
        // TODO : 키세팅
		//if (Input.GetKeyDown(KeyCode.Mouse0))
		//{
		//	var skill = GameManager.Instance.GetSkill("Fireball");
		//	if (skill != null)
		//	{
		//		skill.UseSkill(transform);
		//	}
		//}
        // TODO : 1번 2번 3번

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("기본공격 사용");
            PlayerModel.Skill.DefaultAttack.UseSkill(transform);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("1번스킬 사용");
            var skill = PlayerModel.Skill.GetSkill(KeyCode.Alpha1) as SkillLogic_1;
            skill?.UseSkill(transform);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("2번스킬 사용");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("3번스킬 사용");
        }
    }

    /// <summary>
    /// 플레이어가 대미지를 입는 함수
    /// </summary>
    /// <param name="damage"></param>
	public void TakeDamage(long damage)
	{
		PlayerModel.ApplyDamage(damage);
		// TODO : view 피격처리
		// TODO : UI 체력감소 처리
	}

    /// <summary>
    /// 플레이어가 체력을 회복하는 함수
    /// </summary>
    /// <param name="amount"></param>
    public void TakeHeal(long amount)
    {
        PlayerModel.ApplyHeal(amount);
        // TODO : view 힐처리
        // TODO : UI 체력증가 처리
    }

    public void GetItem(long amount)
    {
        // TODO : 플레이어 재화 추가
    }

    public void Test_ChangeStat()
    {
        if (PlayerModel == null) return;

        PlayerModel.Data.SetAttackLevel();
    }

    public void SaveData()
    {
        if (PlayerModel == null) return;

        SaveLoadManager.Instance.GameData = PlayerModel.GetGameData();
    }

    /// <summary>
    /// 플레이어 데이터 초기화, 게임매니저의 스탯테이블을 받아오기 전까지 대기 후 초기화
    /// </summary>
    /// <returns></returns>
    IEnumerator PlayerInit()
    {
        while (GameManager.Instance.StatDic == null || GameManager.Instance.StatDic.Count == 0)
        {
            yield return null;
        }

        PlayerModel = new PlayerModel();
        PlayerView = GetComponent<PlayerView>();
        PlayerAI = new PlayerAI(this, PlayerView, PlayerModel);

        // 게임매니저에 자기자신 참조
        GameManager.Instance.PlayerController = this;
        // 세이브로드매니저에서 데이터 받아오기
        PlayerModel.InitModel(SaveLoadManager.Instance.GameData);

        // TODO : 로딩종료

        _isInit = true;
        yield break;
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

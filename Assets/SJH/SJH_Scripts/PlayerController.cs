using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
#region enum CostType
/// <summary>
/// 재화 타입
/// </summary>
public enum CostType
{
	Warmth,         // 온기
	SpiritEnergy,   // 영기
	Soul,           // 혼백
}
#endregion

/// <summary>
/// 클라이언트의 입력을 관리하는 컴포넌트
/// </summary>
public class PlayerController : MonoBehaviour
{
	public static PlayerController Instance => GameManager.Instance.Player;

	[Tooltip("플레이어 데이터 로드 여부")]
	[SerializeField]private bool _isInit = false;
	public bool IsInit => _isInit;

	[SerializeField] private PlayerModel _model;
    public PlayerModel Model { get => _model; private set => _model = value; }
	[SerializeField] private PlayerView _view;
    public PlayerView View { get => _view; private set => _view = value; }
	[SerializeField] private PlayerAI _ai;
    public PlayerAI AI { get => _ai; private set => _ai = value; }

	public SkillController SkillController;

	// 게임이 시작되면 시작은 Auto ? Manual
	[Header("컨트롤 모드 Auto/Manual")]
	[SerializeField] private ControlMode _mode;
	public ControlMode Mode
	{
		get => _mode;
		set
		{
			_mode = value;
			// 자동모드가 되면 항상 Search에서 시작
			Debug.Log($"AIState : {CurrentState} > {value}");
			if (CurrentState != AIState.Search) CurrentState = AIState.Search;
		}
	}

	[Header("AI 필드변수")] // AI 에서 사용하는 필드변수
	public AIState CurrentState;		// AI 상태
	public float SearchDistance = 8;	// 탐색 거리
	public int DirectionCount = 8;		// 탐색할 칸의 개수 360 / 8
	public float SightAngle = 45f;		// 칸마다 각도
	public LayerMask MonsterLayer;      // 탐색할 레이어

	[Header("수동모드 필드변수")] // Manual 에서 사용하는 필드변수
	public Vector2 MoveDir; // 플레이어의 이동 방향
	[SerializeField] private AudioSource _audioSource;
	void Start()
	{
		// 시작은 자동모드
		CurrentState = AIState.Search;
		Mode = ControlMode.Auto;
		//Mode = ControlMode.Manual;
		_audioSource = GetComponent<AudioSource>();
	}

	void Update()
	{
		if (_isInit == false)
		{
			Debug.Log("초기화가 아직 안됐음");
			return;
		}
		if (Model.Data.IsDead) return;

		// Auto일 때는 입력 제한
		if (Mode == ControlMode.Auto) AI.Action();

		// 수동 컨트롤
		else if (Mode == ControlMode.Manual) InputHandler();
	}

	public void InputHandler()
	{
		MoveInput();
		SkillInput();
	}

	void MoveInput()
	{
		MoveDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
		View.Move(MoveDir, Model.Data.Speed);
	}

	void SkillInput()
	{
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			Debug.Log("기본공격 사용");
			//_model.Skill.DefaultAttack.UseSkill(transform);
			var skill = Model.Skill.GetSkill(KeyCode.Mouse0);
			skill?.UseSkill(transform);
		}
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			Debug.Log("1번 슬롯 스킬 사용");
			var skill = Model.Skill.GetSkill(KeyCode.Alpha1);
			if (skill != null && skill.UseSkill(transform)) SkillButton.Instance.UpdateCooldown(1);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			Debug.Log("2번 슬롯 스킬 사용");
			var skill = Model.Skill.GetSkill(KeyCode.Alpha2);
			if (skill != null && skill.UseSkill(transform)) SkillButton.Instance.UpdateCooldown(2);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			Debug.Log("3번 슬롯 스킬 사용");
			var skill = Model.Skill.GetSkill(KeyCode.Alpha3);
			if (skill != null && skill.UseSkill(transform)) SkillButton.Instance.UpdateCooldown(3);
		}
	}

	public bool UseSkill(int index)
	{
		// 여긴 스킬버튼에서 쿨타임 UI 처리
		switch (index)
		{
			case 0:
				Debug.Log("기본공격 사용");
				//_model.Skill.DefaultAttack.UseSkill(transform);
				var skill0 = Model.Skill.GetSkill(KeyCode.Mouse0);
				if (skill0 == null) return false;
				return skill0.UseSkill(transform);
			case 1:
				Debug.Log("1번스킬 사용");
				var skill1 = Model.Skill.GetSkill(KeyCode.Alpha1);
				if (skill1 == null) return false;
				return skill1.UseSkill(transform);
			case 2:
				Debug.Log("2번스킬 사용");
				var skill2 = Model.Skill.GetSkill(KeyCode.Alpha2);
				if (skill2 == null) return false;
				return skill2.UseSkill(transform);
			case 3:
				Debug.Log("3번스킬 사용");
				var skill3 = Model.Skill.GetSkill(KeyCode.Alpha3);
				if (skill3 == null) return false;
				return skill3.UseSkill(transform);
		}
		return false;
	}

	/// <summary>
	/// 플레이어 데이터 초기화, 게임매니저의 스탯테이블을 받아오기 전까지 대기 후 초기화
	/// </summary>
	/// <returns></returns>
	public IEnumerator PlayerInitRoutine()
	{
		// 게임매니저에 자기자신 참조
		GameManager.Instance.Player = this;

		// 모델, 뷰, AI, 스킬컨트롤러 초기화
		_model = new PlayerModel();
		_view = GetComponent<PlayerView>();
		_ai = new PlayerAI(this, _view, _model);
		SkillController = GetComponentInChildren<SkillController>();
		SkillController.InitSkillController();

        // 세이브로드매니저에서 데이터 받아오기
        Model.InitModel(SaveLoadManager.Instance.GameData);

		yield return UIManager.Instance.MainUI != null;

		// UI 초기화
		if (UIManager.Instance.SceneUIList.Count > 0)
		{
			foreach (var ui in UIManager.Instance.SceneUIList)
			{
				if (ui == null || ui.Equals(null)) continue;
				Debug.Log("UI 초기화");
				ui.UIInit();
			}
		}
		Debug.Log("UI 초기화 완료");

		_isInit = true;

		yield break;
	}

	#region Model 함수
	/// <summary>
	/// 플레이어가 대미지를 입는 함수
	/// </summary>
	/// <param name="damage"></param>
	public void TakeDamage(long damage)
	{
		if (GameManager.IsImmortal)
		{
			Debug.Log($"{damage}의 대미지를 입었지만 무적입니다.");
			return;
		}

		// 죽어있을 때
		if (Model.Data.IsDead) return;

		Debug.Log($"플레이어 [{damage}] 피해");

		// 대미지 처리
		Model.ApplyDamage(damage);

		// view 처리
		if (Model.Data.IsDead)
		{
            View.AIStop(); // velocity 0으로 변경
			View.SetBool("IsDead", true);
			OnDeath();
		}

		// 대미지 색상 변경
		UIManager.Instance.ShowDamageText(transform, damage, Color.red);

		// 업적 실패 처리
		string scene = SceneManager.GetActiveScene().name;
		if (scene == "Stage1-3_Battle" || scene == "Stage2-3_Battle" || scene == "Stage3-3_Battle")
		{
			Debug.Log("[업적 실패] 보스 스테이지에서 피격됨");
			// TODO : 업적 실패 처리
		}
	}

	/// <summary>
	/// 플레이어가 체력을 회복하는 함수
	/// </summary>
	/// <param name="amount"></param>
	public void TakeHeal(long amount)
	{
		Debug.Log($"플레이어 [{amount}] 회복");
		Model.ApplyHeal(amount);
		UIManager.Instance.ShowDamageText(transform, amount, Color.green);
	}

	/// <summary>
	/// 플레이어가 보호막을 생성하는 함수
	/// </summary>
	/// <param name="costType"></param>
	/// <returns></returns>
	public void TakeShield(long amount)
	{
		Debug.Log($"플레이어 [{amount}] 보호막 획득");
		Model.ApplyShield(amount);
		UIManager.Instance.ShowDamageText(transform, amount, Color.blue);
	}
	/// <summary>
	/// 플레이어가 죽었을 때 실행하는 함수
	/// </summary>
	public void OnDeath()
	{
		AchievementManager.Instance?.CheckDeathAchievements(); // 플레이어 Death 업적 카운트
		if (MissionManager.Instance.IsRunning())
		{
			MissionManager.Instance.DeathFailMission();
		}
		// 죽음 팝업 활성화
		UIManager.Instance.ShowPopUp<DiePopUp>();
	}
	#endregion

	/// <summary>
	/// 플레이어 저장 데이터 반환하는 함수
	/// </summary>
	/// <returns></returns>
	public GameData SaveData() => SaveLoadManager.Instance.GameData = Model.GetGameData();

	public void PlaySkillSound(AudioClip clip)
	{
		if (_audioSource == null) return;

		_audioSource.PlayOneShot(clip, 1f);
	}

 //   void OnDrawGizmos()
	//{
	//	// 플레이어의 공격 범위 기즈모
	//	// 팔각
	//	for (int i = 0; i < DirectionCount; i++)
	//	{
	//		float angle = SightAngle * i;
	//		Vector3 dir = Quaternion.Euler(0, 0, angle) * transform.up;
	//		Gizmos.color = Color.red;
	//		Gizmos.DrawLine(transform.position, transform.position + dir * SearchDistance);
	//	}
	//	// 원
	//	Gizmos.DrawWireSphere(transform.position, SearchDistance);
	//}
}

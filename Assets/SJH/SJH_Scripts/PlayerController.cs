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
	[SerializeField ]private bool _isInit = false;
    public bool IsInit => _isInit;

	[SerializeField] private PlayerModel _model;
    [SerializeField] private PlayerView _view;
    [SerializeField] private PlayerAI _ai;

    public SkillController SkillController;

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

	[Header("AI 필드변수")] // AI 에서 사용하는 필드변수
	public AIState CurrentState;		// AI 상태
	public float SearchDistance = 8;	// 탐색 거리
	public int DirectionCount = 8;		// 탐색할 칸의 개수 360 / 8
	public float SightAngle = 45f;		// 칸마다 각도
	public LayerMask MonsterLayer;      // 탐색할 레이어

	[Header("수동모드 필드변수")] // Manual 에서 사용하는 필드변수
	public Vector2 MoveDir; // 플레이어의 이동 방향

    void Start()
	{
		// 시작은 자동모드
		CurrentState = AIState.Search;
		//Mode = ControlMode.Auto;
        Mode = ControlMode.Manual;
    }

	void Update()
	{
        if (_isInit == false)
		{
			Debug.Log("초기화가 아직 안됐음");
			return;
		}
        if (GetIsDead()) return;

		// Auto일 때는 입력 제한
		if (Mode == ControlMode.Auto) _ai.Action();

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
		_view.Move(MoveDir, _model.Data.Speed);
	}

	void SkillInput()
	{
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("기본공격 사용");
            //_model.Skill.DefaultAttack.UseSkill(transform);
            var skill = _model.Skill.GetSkill(KeyCode.Mouse0);
            skill?.UseSkill(transform);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("1번 슬롯 스킬 사용");
            var skill = _model.Skill.GetSkill(KeyCode.Alpha1);
            if (skill != null && skill.UseSkill(transform)) SkillButton.Instance.UpdateCooldown(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("2번 슬롯 스킬 사용");
            var skill = _model.Skill.GetSkill(KeyCode.Alpha2);
            if (skill != null && skill.UseSkill(transform)) SkillButton.Instance.UpdateCooldown(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("3번 슬롯 스킬 사용");
            var skill = _model.Skill.GetSkill(KeyCode.Alpha3);
            if (skill != null && skill.UseSkill(transform)) SkillButton.Instance.UpdateCooldown(3);
        }
    }

    bool SkillInput(int index)
    {
        // 여긴 스킬버튼에서 쿨타임 UI 처리
        switch (index)
        {
            case 0:
                Debug.Log("기본공격 사용");
                //_model.Skill.DefaultAttack.UseSkill(transform);
                var skill0 = _model.Skill.GetSkill(KeyCode.Mouse0);
                if (skill0 == null) return false;
                return skill0.UseSkill(transform);
            case 1:
                Debug.Log("1번스킬 사용");
                var skill1 = _model.Skill.GetSkill(KeyCode.Alpha1);
                if (skill1 == null) return false;
                return skill1.UseSkill(transform);
            case 2:
                Debug.Log("2번스킬 사용");
                var skill2 = _model.Skill.GetSkill(KeyCode.Alpha2);
                if (skill2 == null) return false;
                return skill2.UseSkill(transform);
            case 3:
                Debug.Log("3번스킬 사용");
                var skill3 = _model.Skill.GetSkill(KeyCode.Alpha3);
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
        _model.InitModel(SaveLoadManager.Instance.GameData);

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

        // TODO : 로딩종료

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
        if (GetIsDead()) return;

        // 대미지 처리
        _model.ApplyDamage(damage);

        // view 처리
        if (GetIsDead())
        {
            AIStop(); // velocity 0으로 변경
            SetBool("IsDead", true);
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
		_model.ApplyHeal(amount);
        // TODO : view 힐처리
        // TODO : UI 체력증가 처리

        UIManager.Instance.ShowDamageText(transform, amount, Color.green);
    }

    /// <summary>
    /// 플레이어가 보호막을 생성하는 함수
    /// </summary>
    /// <param name="costType"></param>
    /// <returns></returns>
    public void TakeShield(long amount)
    {
        _model.ApplyShield(amount);
        // TODO : view 보호막처리
        // TODO : UI 보호막 증가 처리
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

    #region Data 관련 함수
    /// <summary>
    /// 플레이어의 죽음 체크
    /// <br/> true = 죽음
    /// <br/> false = 살음
    /// </summary>
    /// <returns></returns>
    public bool GetIsDead() => _model.GetIsDead();
    /// <summary>
    /// 플레이어의 전투력을 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public long GetPower() => _model.GetPower();
    /// <summary>
    /// 플레이어의 공격력을 반환하는 함수 
    /// </summary>
    /// <returns></returns>
    public long GetAttack() => _model.GetAttack();
    /// <summary>
    /// 플레이어의 방어력을 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public long GetDefense() => _model.GetDefense();
    /// <summary>
    /// 플레이어의 최대 체력을 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public long GetMaxHp() => _model.GetMaxHp();
    /// <summary>
    /// 플레이어의 현재 체력을 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public long GetHp() => _model.GetHp();
    /// <summary>
    /// 플레이어의 현재 보호막 체력을 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public long GetShieldHp() => _model.GetShieldHp();
    /// <summary>
    /// 플레이어의 실드량을 초기화하는 함수
    /// </summary>
    public void ClearShield() => _model.ClearShield();
    /// <summary>
    /// 플레이어의 온기 보유량을 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public long GetWarmth() => _model.GetWarmth();
    /// <summary>
    /// 플레이어의 영기 보유량을 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public long GetSpiritEnergy() => _model.GetSpiritEnergy();
    /// <summary>
    /// 플레이어의 혼백 보유량을 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public long GetSoul() => _model.GetSoul();
    /// <summary>
    /// UI 업데이트 이벤트 연결하는 함수
    /// </summary>
    /// <param name="playerStatUI"></param>
    public void ConnectEvent(Action playerStatUI) => _model.ConnectEvent(playerStatUI);
    /// <summary>
    /// 플레이어의 이름을 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public string GetPlayerName() => _model.GetPlayerName();
    /// <summary>
    /// 플레이어의 이름을 지정하는 함수
    /// </summary>
    /// <returns></returns>
    public string SetPlayerName(string newName) => _model.SetPlayerName(newName);
    /// <summary>
    /// 플레이어 스탯을 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public SavePlayerData GetPlayerData() => _model.GetPlayerData();
    /// <summary>
    /// 플레이어 공격력 레벨업 함수
    /// </summary>
    public void TryAttackLevelup() => _model.TryAttackLevelup();
    /// <summary>
    /// 플레이어 방어력 레벨업 함수
    /// </summary>
    public void TryDefenseLevelup() => _model.TryDefenseLevelup();
    /// <summary>
    /// 플레이어 체력 레벨업 함수
    /// </summary>
    public void TryHpLevelup() => _model.TryHpLevelup();
    /// <summary>
    /// 플레이어 스피드 레벨업 함수
    /// </summary>
    public void TrySpeedLevelup() => _model.TrySpeedLevelup();
    /// <summary>
    /// 플레이어 공격력 * (1 + 가하는 피해 증가)
    /// </summary>
    /// <returns></returns>
    public long GetTotalDamage() => (long)((_model.GetAttack() * (1f + _model.GetEquipmentAttack())) * (1f + _model.GetIncreseDamage()));
    public int GetPlayerSceneIndex() => _model.GetPlayerSceneIndex();
    public void SetPlayerSceneIndex(int index)
    {
        _model.SetPlayerSceneIndex(index);
    }
    #endregion

    #region Cost 관련 함수
    public long GetCost(CostType costType) => _model.GetCost(costType);
	public void AddCost(CostType costType, long amount) => _model.AddCost(costType, amount);
	public void SpendCost(CostType costType, long amount) => _model.SpendCost(costType, amount);
    #endregion

    #region Equipment 관련 함수
    /// <summary>
    /// 플레이어 장비의 등급, 레벨을 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public SaveEquipmentData GetEquipmentData() => _model.GetEquipmentData();
    /// <summary>
    /// 플레이어의 장비 강화를 실행하는 함수
    /// </summary>
    public void TryEnhance() => _model.TryEnhance();
    /// <summary>
    /// 플레이어의 장비 등급업을 실행하는 함수
    /// </summary>
    public void TryPromote() => _model.TryPromote();
    /// <summary>
    /// 플레이어의 장비 등급을 GradeType으로 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public GradeType GetGradeType() => _model.GetGradeType();
    /// <summary>
    /// 플레이어 장비의 가하는 피해 증가율을 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public float GetIncreseDamage() => _model.GetIncreseDamage();
    /// <summary>
    /// 레벨에 따라 가하는 피해 증가율을 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public float GetIncreseDamage(int level) => _model.GetIncreseDamage(level);
    /// <summary>
    /// 플레이어 장비의 스킬 쿨타임 감소율을 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public float GetCalculateCooldown(float defaultCooldown) => _model.GetCalculateCooldown(defaultCooldown);
    /// <summary>
    /// 플레이어 장비의 공격력 증가율을 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public float GetEquipmentAttack() => _model.GetEquipmentAttack();
    #endregion

    #region Skill 관련 함수
    /// <summary>
    /// 플레이어 스킬을 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public List<SaveSkillData> GetSkillData() => _model.GetSkillData();
    /// <summary>
    /// skillIndex 번 스킬을 레벨업하는 함수
    /// </summary>
    /// <param name="skillIndex"></param>
    public void TrySkillLevelUp(int skillIndex) => _model.TrySkillLevelUp(skillIndex);
    /// <summary>
    /// 단축키에 등록된 스킬들을 Dictionary<KeyCode, ISkill> 로 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public Dictionary<KeyCode, ISkill> GetMappingSkill() => _model.GetMappingSkills();
    /// <summary>
    /// 플레이어 단축키에 등록된 스킬들을 List<ISkill> 로 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public List<ISkill> GetMappingSkillList() => _model.GetSkillMappingList();
    /// <summary>
    /// 플레이어가 보유중이고 단축키에 등록되지 않은 스킬들을 List<ISkill> 로 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public List<ISkill> GetHasSkillList() => _model.GetHasSkillList();
    /// <summary>
    /// UI 스킬버튼 클릭으로 스킬 사용하는 함수
    /// </summary>
    /// <param name="index"></param>
    public bool UseSkill(int index) => SkillInput(index);
    /// <summary>
    /// skillIndex 번째 스킬을 획득하는 함수
    /// </summary>
    /// <param name="skillIndex"></param>
    public void LearnSkill(int skillIndex) => _model.LearnSkill(skillIndex);
    /// <summary>
    /// skillIndex 번째 스킬을 단축창에 추가를 시도하는 함수
    /// </summary>
    /// <param name="skillIndex"></param>
    public void AddSkillSlot(int skillIndex) => _model.AddSkillSlot(skillIndex);
    /// <summary>
    /// skillIndex 번째 스킬을 단축창에서 제거를 시도하는 함수
    /// </summary>
    /// <param name="skillIndex"></param>
    public void RemoveSkillSlot(int skillIndex) => _model.RemoveSkillSlot(skillIndex);
    #endregion

    #region Achievment, Mission 관련 함수
    /// <summary>
    /// 플레이어의 업적 데이터를 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public List<SaveAchievementData> GetAchievData() => _model.GetAchievData();
    /// <summary>
    /// 플레이어의 돌파 미션 데이터를 반환하는 함수
    /// </summary>
    /// <returns></returns>
    public List<SaveMissionData> GetMissionData() => _model.GetMissionData();
    #endregion
    #endregion

    #region View 함수

    /// <summary>
    /// 플레이어 애니메이터 전환
    /// </summary>
    /// <param name="trigger"></param>
    public void SetTrigger(string trigger) => _view.SetTrigger(trigger);
    /// <summary>
    /// 플레이어 애니메이터 전환
    /// </summary>
    /// <param name="name", ></param>
    public void SetBool(string name, bool value) => _view.SetBool(name, value);
    /// <summary>
    /// 플레이어 강제 스탑
    /// </summary>
    public void Stop() => _view.Stop();
    /// <summary>
    /// 플레이어 강제 스탑 해제
    /// </summary>
    public void Move() => _view.Move();
    /// <summary>
    /// 플레이어 AI용 스탑
    /// </summary>
    public void AIStop() => _view.AIStop();
    /// <summary>
    /// 플레이어의 강제이동 상태 체크
    /// <br/> true = 이동 가능
    /// <br/> false = 이동 불가능
    /// </summary>
    /// <returns></returns>
    public bool MoveCheck() => _view.GetMoveCheck();
    #endregion

    #region AI 함수

    public void AIInit() => _ai.MonsterSkillCheck();

    #endregion

    /// <summary>
    /// 플레이어 저장 데이터 반환하는 함수
    /// </summary>
    /// <returns></returns>
	public GameData SaveData() => SaveLoadManager.Instance.GameData = _model.GetGameData();

    #region 애니메이션 이벤트 함수

    // SkillLogic_0 애니메이션 이벤트 함수들
    public void Skill0_OnAttackStart() => (SkillController.SkillList[0] as SkillLogic_0_HitBox)?.OnAttackStart();
    public void Skill0_OnAttackEnd() => (SkillController.SkillList[0] as SkillLogic_0_HitBox)?.OnAttackEnd();
    public void Skill0_SlashCountEvent() => (SkillController.SkillList[0] as SkillLogic_0_HitBox)?.SlashCountEvent();

    // SkillLogic_1 애니메이션 이벤트 함수
    public void Skill1_DisableHitbox() => (SkillController.SkillList[1] as SkillLogic_1)?.DisableHitbox();
    public void Skill1_CreateEffect() => (SkillController.SkillList[1] as SkillLogic_1)?.CreateEffect();

    // SkillLogic_2 애니메이션 이벤트 함수
    public void Skill2_SkillRoutine() => (SkillController.SkillList[2] as SkillLogic_2)?.SkillRoutine();

    // SkillLogic_3 애니메이션 이벤트 함수
    public void Skill3_SkillRoutine() => (SkillController.SkillList[3] as SkillLogic_3)?.SkillRoutine();
    public void Skill3_OnAttackEnd() => (SkillController.SkillList[3] as SkillLogic_3)?.OnAttackEnd();

    // SkillLogic_4 애니메이션 이벤트 함수
    public void Skill4_SkillRoutine() => (SkillController.SkillList[4] as SkillLogic_4)?.SkillRoutine();

    // SkillLogic_5 애니메이션 이벤트 함수
    public void Skill5_SkillRoutine() => (SkillController.SkillList[5] as SkillLogic_5)?.SkillRoutine();

    // SkillLogic_6 애니메이션 이벤트 함수
    public void Skill6_SkillRoutine() => (SkillController.SkillList[6] as SkillLogic_6)?.SkillRoutine();

    #endregion

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

using System;
using UnityEngine;

public struct SavePlayerData
{
    public string PlayerName;
    public int AttackLevel;
    public int DefenseLevel;
    public int HpLevel;
    public long CurrentHp;
    public int SpeedLevel;
    public long ShieldHp;
}

/// <summary>
/// 플레이어의 스탯 데이터 클래스, 인스턴스화 후 InitData()를 호출하여 플레이어 데이터 넣어야 함
/// </summary>
[System.Serializable]
public class PlayerData
{
    /// <summary>
    /// 스탯 변경시 UI 업데이트 이벤트
    /// </summary>
    public event Action OnStatChanged;

    [SerializeField] private string _playerName;
    public string PlayerName
    {
        get => _playerName;
        set
        {
            _playerName = value;
            // TODO : UI 변경 이벤트
        }
    }

    /// <summary>
    /// 플레이어 전투력 
    /// </summary> 
    [SerializeField] public long PowerLevel { get => (long)((PlayerController.Instance.GetAttack() * 0.95f + MaxHp * 0.05) * (1 + Defense / 1200f * 0.25f)); }

    [field: SerializeField] private int _attackLevel;
    /// <summary>
    /// 공격력 10 ~ 5,202,220,766,384,660
    /// </summary>
    [field: SerializeField] public long Attack { get; private set; }
	/// <summary>
	/// 공격력 레벨 1 ~ 300
	/// </summary>
	public int AttackLevel
	{
		get => _attackLevel;
		private set
		{
            //Debug.Log("공격력 계산");
			_attackLevel = Mathf.Clamp(value, 1, 300);
			Attack = GetStat(StatDataType.Attack, _attackLevel);
            OnStatChanged?.Invoke();
            if (PlayerController.Instance.IsInit) AchievementManager.Instance?.CheckPowerAchievements();  // 전투력 업적 체크
        }
	}

    [field: SerializeField] private int _defenseLevel;
    /// <summary>
    /// 방어력 (받피감) 12 ~ 1200
    /// </summary>
    [field: SerializeField] public long Defense { get; private set; }
	/// <summary>
	/// 방어력 레벨 1 ~ 300
	/// </summary>
	public int DefenseLevel
	{
		get => _defenseLevel;
		private set
		{
            //Debug.Log("방어력 계산");
            _defenseLevel = Mathf.Clamp(value, 1, 300);
			Defense = GetStat(StatDataType.Defense, _defenseLevel);
            OnStatChanged?.Invoke();
            if (PlayerController.Instance.IsInit) AchievementManager.Instance?.CheckPowerAchievements();  // 전투력 업적 체크
        }
	}

    [field: SerializeField] private int _hpLevel;
    /// <summary>
    /// 최대 체력 100 ~ 237,910,090,562,588
    /// </summary>
    [field: SerializeField] public long MaxHp { get; private set; }
	/// <summary>
	/// 체력 레벨 1 ~ 300
	/// </summary>
	public int HpLevel
	{
		get => _hpLevel;
		private set
		{
            //Debug.Log("체력 계산");
            _hpLevel = Mathf.Clamp(value, 1, 300);
			MaxHp = GetStat(StatDataType.Hp, _hpLevel);
            OnStatChanged?.Invoke();
            if (PlayerController.Instance.IsInit) AchievementManager.Instance?.CheckPowerAchievements();  // 전투력 업적 체크
        }
	}

    /// <summary>
    /// 현재 체력
    /// </summary>
    [field: SerializeField] public long Hp { get; private set; }

    [field: SerializeField] private int _speedLevel;
    /// <summary>
    /// 이동 속도 100 ~ 200, 이동속도 / 50 = 유니티 이속
    /// </summary>
    [field: SerializeField] public float Speed { get; private set; }
	/// <summary>
	/// 이동 속도 레벨 51레벨까지
	/// </summary>
	public int SpeedLevel
	{
		get => _speedLevel;
		private set
		{
            //Debug.Log("스피드 계산");
            _speedLevel = Mathf.Clamp(value, 1, 50);
			Speed = GetStat(StatDataType.Speed, _speedLevel) / _speedRatio;
            OnStatChanged?.Invoke();
        }
	}
	private const int _speedRatio = 50;

    [Tooltip("죽음체크, true = 사망")]
    [SerializeField] private bool _isDead;
    public bool IsDead
    {
        get => _isDead;
        set { _isDead = value; }
    }

    [Tooltip("보호막 체력")]
    [SerializeField] private long _shieldHp;
    public long ShieldHp
    {
        get => _shieldHp;
        set
        {
            _shieldHp = value;
            // TODO : 이벤트 연결
        }
    }

    /// <summary>
    /// 불러온 플레이어의 데이터를 초기화하는 함수
    /// </summary>
    /// <param name="attackLevel"></param>
    /// <param name="defenseLevel"></param>
    /// <param name="hpLevel"></param>
    /// <param name="speedLevel"></param>
    /// <param name="increaseDamageLevel"></param>
	public void InitData(int attackLevel = 1, int defenseLevel = 1, int hpLevel = 1, long currentHp = 100, int speedLevel = 1, long shieldHp = 0)
	{
        //Debug.Log($"InitData 호출 : ATK {attackLevel}, DEF {defenseLevel}, HP {hpLevel}, SPD {speedLevel}");
        // 프로퍼티에서 레벨만으로 각 스탯 계산
        AttackLevel = attackLevel;
		DefenseLevel = defenseLevel;
		HpLevel = hpLevel;
        Hp = currentHp;
        SpeedLevel = speedLevel;
        ShieldHp = shieldHp;
	}

	public long GetStat(StatDataType statType, int level)
	{
  //      if (!DataManager.Instance.StatDataTable.TryGetValue(statType, out var levelTable))
  //      {
  //          Debug.Log("데이터매니저 StatDic == null");
  //          return 0;
  //      }
		//if (!levelTable.TryGetValue(level, out long statValue)) return 0;
		//return statValue;

        return DataManager.Instance.GetStatData(statType, level);
	}

	public void DecreaseHp(long damage)
	{
        // 체력을 감소하기 전 보호막부터 우선 감소
        long totalDamage = (long)(damage * (float)Defense / (Defense + 300f));
        totalDamage = (1 > totalDamage) ? 1 : totalDamage; // 최소 1
        if (ShieldHp > 0)
        {
            // 실드가 대미지보다 낮으면
            if (ShieldHp < totalDamage)
            {
                totalDamage -= ShieldHp; // 실드에서 대미지 차감 후
                ShieldHp = 0;       // 실드값 0
            }
            else if (ShieldHp >= totalDamage)
            {
                ShieldHp -= totalDamage;
                return;
            }
        }
		Hp -= totalDamage;
		if (Hp <= 0) Hp = 0;
        IsDead = Hp <= 0;
        //Debug.LogError($"받은 대미지 : {totalDamage} / 체력 : {Hp} / IsDead : {IsDead}");
	}

    // 체력회복하는 함수
    public void HealHp(long amount)
    {
        if ((Hp + amount) > MaxHp) Hp = MaxHp;
        else Hp += amount;
    }

    public void HealShield(long amount)
    {
        ShieldHp += amount;
    }

    public void ClearShield()
    {
        ShieldHp = 0;
        // TODO : 실드 UI 업데이트
    }

    // 스탯 변경 함수
    public void AttackLevelup()
    {
        AttackLevel += 1;
        Debug.Log($"공격력 업! 레벨 : {AttackLevel}");
    }

    public void DefenseLevelup()
    {
        DefenseLevel += 1;
        Debug.Log($"방어력 업! 레벨 : {DefenseLevel}");
    }

    public void HpLevelup()
    {
        HpLevel += 1;
        // TODO : 체력이 증가한 만큼 현재 체력도 회복
        Debug.Log($"체력 업! 레벨 : {HpLevel}");
    }

    public void SpeedLevelup()
    {
        SpeedLevel += 1;
        Debug.Log($"스피드 업! 레벨 : {SpeedLevel}");
    }

    public void SetPlayerName(string name)
    {
        PlayerName = name;
        Debug.Log($"플레이어 이름 변경 : {PlayerName}");
    }

    // 플레이어데이터 세이브 구조체
    public SavePlayerData SavePlayerData()
    {
        var data = new SavePlayerData();
        data.PlayerName = PlayerName;
        data.AttackLevel = AttackLevel;
        data.DefenseLevel = DefenseLevel;
        data.HpLevel = HpLevel;
        data.CurrentHp = Hp;
        data.SpeedLevel = SpeedLevel;
        data.ShieldHp = ShieldHp;
        return data;
    }
}

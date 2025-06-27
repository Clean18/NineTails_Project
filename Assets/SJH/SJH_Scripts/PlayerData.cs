using UnityEngine;

public struct SavePlayerData
{
    public int AttackLevel;
    public int DefenseLevel;
    public int HpLevel;
    public int SpeedLevel;
    public int IncreaseDamageLevel;
    public long ShieldHp;
}

/// <summary>
/// 플레이어의 스탯 데이터 클래스, 인스턴스화 후 InitData()를 호출하여 플레이어 데이터 넣어야 함
/// </summary>
[System.Serializable]
public class PlayerData
{
    /// <summary>
    /// 플레이어 전투력 
    /// </summary> 
    [SerializeField] public long PowerLevel { get; private set; }

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
            Debug.Log("공격력 계산");
			_attackLevel = Mathf.Clamp(value, 1, 300);
			Attack = GetStat(StatType.Attack, _attackLevel); 
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
            Debug.Log("방어력 계산");
            _defenseLevel = Mathf.Clamp(value, 1, 300);
			Defense = GetStat(StatType.Defense, _defenseLevel);
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
            Debug.Log("체력 계산");
            _hpLevel = Mathf.Clamp(value, 1, 300);
			MaxHp = GetStat(StatType.Hp, _hpLevel);
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
            Debug.Log("스피드 계산");
            _speedLevel = Mathf.Clamp(value, 1, 50);
			Speed = GetStat(StatType.Speed, _speedLevel) / _speedRatio;
		}
	}
	private const int _speedRatio = 50;

    /// <summary>
    /// 가하는 피해 증가 (특수 스탯) 기본 5%, 0.2% 씩 증가
    /// </summary>
    [Tooltip("가하는 피해 증가 (특수 스탯) 기본 5%, 0.2% 씩 증가")]
    [field: SerializeField] public float IncreaseDamage;
    /// <summary>
    /// 가하는 피해 증가 레벨
    /// </summary>
    [Tooltip("가하는 피해 증가 레벨")]
    [field: SerializeField] public int IncreaseDamageLevel; // TODO : 계산식 추가

    [Tooltip("죽음체크, true = 사망")]
    [SerializeField] private bool _isDead;
    public bool IsDead
    {
        get => (Hp <= 0);
        set { _isDead = value; }
    }

    [Tooltip("보호막 체력")]
    [SerializeField] public long ShieldHp;

    /// <summary>
    /// 불러온 플레이어의 데이터를 초기화하는 함수
    /// </summary>
    /// <param name="attackLevel"></param>
    /// <param name="defenseLevel"></param>
    /// <param name="hpLevel"></param>
    /// <param name="speedLevel"></param>
    /// <param name="increaseDamageLevel"></param>
	public void InitData(int attackLevel = 1, int defenseLevel = 1, int hpLevel = 1, long currentHp = 100, int speedLevel = 1, int increaseDamageLevel = 0, long shieldHp = 0)
	{
        Debug.Log($"InitData 호출 : ATK {attackLevel}, DEF {defenseLevel}, HP {hpLevel}, SPD {speedLevel}");
        // 프로퍼티에서 레벨만으로 각 스탯 계산
        AttackLevel = attackLevel;
		DefenseLevel = defenseLevel;
		HpLevel = hpLevel;
        Hp = currentHp;
        SpeedLevel = speedLevel;
		IncreaseDamageLevel = increaseDamageLevel;
        ShieldHp = shieldHp;
	}

	public long GetStat(StatType statType, int level)
	{
        if (!GameManager.Instance.StatDic.TryGetValue(statType, out var levelTable))
        {
            Debug.Log("게임매니저 StatDic == null");
            return 0;
        }

		if (!levelTable.TryGetValue(level, out long statValue)) return 0;
		return statValue;
	}

	public void DecreaseHp(long damage)
	{
        // TODO : 체력을 감소하기 전 보호막부터 우선 감소
        if (ShieldHp > 0)
        {
            // 실드가 대미지보다 낮으면
            if (ShieldHp < damage)
            {
                damage -= ShieldHp; // 실드에서 대미지 차감 후
                ShieldHp = 0;       // 실드값 0
            }
            else if (ShieldHp >= damage)
            {
                ShieldHp -= damage;
                return;
            }
        }
		Hp -= damage;
		if (Hp <= 0) Hp = 0;
	}

    // 체력회복하는 함수
    public void HealHp(long amount)
    {
        if ((Hp + amount) > MaxHp) Hp = MaxHp;
        else Hp += amount;
    }

    // 스탯 변경 함수
    public void SetAttackLevel()
    {
        AttackLevel += 1;
        Debug.Log($"공격력 업! 레벨 : {AttackLevel}");
    }

    public SavePlayerData SavePlayerData()
    {
        var data = new SavePlayerData();
        data.AttackLevel = AttackLevel;
        data.DefenseLevel = DefenseLevel;
        data.HpLevel = HpLevel;
        data.SpeedLevel = SpeedLevel;
        data.IncreaseDamageLevel = IncreaseDamageLevel;
        data.ShieldHp = ShieldHp;
        return data;
    }
}

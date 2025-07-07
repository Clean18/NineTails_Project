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
    public int SceneIndex;
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
            OnStatChanged?.Invoke();
        }
    }

    /// <summary>
    /// 플레이어 전투력 
    /// </summary> 
    [SerializeField]
    public long PowerLevel
    { get => (long)((PlayerController.Instance.GetAttack() * (1f + PlayerController.Instance.GetEquipmentAttack()) * 0.95f + MaxHp * 0.05) * (1 + Defense / 1200f * 0.25f)); }

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

    [SerializeField] private long _hp;
    /// <summary>
    /// 현재 체력
    /// </summary>
    public long Hp
    {
        get => _hp;
        set
        {
            var amount = Math.Clamp(value, 0, MaxHp);
            _hp = amount;

            // 죽음 상태 처리
            if (_hp <= 0 && !_isDead)
            {
                _isDead = true;
                Debug.Log("플레이어 사망처리");
            }
            else if (_hp > 0 && _isDead)
            {
                _isDead = false;
                Debug.Log("플레이어 살아있음");
            }

            OnStatChanged?.Invoke();
        }
    }

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
            OnStatChanged?.Invoke();
        }
    }

    [SerializeField] private int _sceneIndex;
    public int SceneIndex
    {
        get => _sceneIndex;
        set
        {
            // 씬 인덱스 바뀔 때마다 세이브
            _sceneIndex = value;
            if (PlayerController.Instance != null && PlayerController.Instance.IsInit) PlayerController.Instance.SaveData();
            Debug.Log($"세이브한 씬 인덱스 : {_sceneIndex} <= {value}");
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
	public void InitData(string name = "구미호", int attackLevel = 1, int defenseLevel = 1, int hpLevel = 1, long currentHp = 100, int speedLevel = 1, long shieldHp = 0, int sceneIndex = 2)
    {
        //Debug.Log($"InitData 호출 : ATK {attackLevel}, DEF {defenseLevel}, HP {hpLevel}, SPD {speedLevel}");
        // 프로퍼티에서 레벨만으로 각 스탯 계산
        PlayerName = name;
        AttackLevel = attackLevel;
        DefenseLevel = defenseLevel;
        HpLevel = hpLevel;
        Hp = currentHp;
        SpeedLevel = speedLevel;
        ShieldHp = shieldHp;
        SceneIndex = sceneIndex;
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
        data.SceneIndex = SceneIndex;
        return data;
    }

    public long GetStat(StatDataType statType, int level)
    {
        return DataManager.Instance.GetStatData(statType, level);
    }

    public void DecreaseHp(long damage)
    {
        // 체력을 감소하기 전 보호막부터 우선 감소
        long totalDamage = (long)(damage * (1- (float)Defense / (Defense + 300f)));
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
        Hp = Math.Max(0, Hp - totalDamage);
    }

    // 체력회복하는 함수
    public void HealHp(long amount)
    {
        Hp += amount;
        Debug.Log($"플레이어 {amount} 회복");
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

    /// <summary>
    /// 플레이어 공격력 스탯 강화를 시도하는 함수
    /// </summary>
    /// <param name="warmth"></param>
    public void TryAttackLevelup(long warmth)
    {
        // 현재 레벨 체크
        if (AttackLevel == 300)
        {
            Debug.Log("최대 레벨입니다.");
            return;
        }

        // 비용 체크
        long cost = DataManager.Instance.GetStatCost(StatDataType.Attack, AttackLevel);
        if (cost > warmth && !GameManager.IsCheat)
        {
            Debug.Log($"온기가 부족합니다. {cost} > {warmth}");
            return;
        }

        // 비용 감소
        if (!GameManager.IsCheat) PlayerController.Instance.SpendCost(CostType.Warmth, cost);

        // 레벨업 실행
        AttackLevelup();
    }
    /// <summary>
    /// 플레이어 방어력 스탯 강화를 시도하는 함수
    /// </summary>
    /// <param name="warmth"></param>
    public void TryDefenseLevelup(long warmth)
    {
        // 현재 레벨 체크
        if (DefenseLevel == 300)
        {
            Debug.Log("최대 레벨입니다.");
            return;
        }

        // 비용 체크
        long cost = DataManager.Instance.GetStatCost(StatDataType.Defense, DefenseLevel);
        if (cost > warmth && !GameManager.IsCheat)
        {
            Debug.Log($"온기가 부족합니다. {cost} > {warmth}");
            return;
        }

        // 비용 감소
        if (!GameManager.IsCheat) PlayerController.Instance.SpendCost(CostType.Warmth, cost);

        // 레벨업 실행
        DefenseLevelup();
    }
    /// <summary>
    /// 플레이어 체력 스탯 강화를 시도하는 함수
    /// </summary>
    public void TryHpLevelup(long warmth)
    {
        // 현재 레벨 체크
        if (HpLevel == 300)
        {
            Debug.Log("최대 레벨입니다.");
            return;
        }

        // 비용 체크
        long cost = DataManager.Instance.GetStatCost(StatDataType.Hp, HpLevel);
        if (cost > warmth && !GameManager.IsCheat)
        {
            Debug.Log($"온기가 부족합니다. {cost} > {warmth}");
            return;
        }

        // 비용 감소
        if (!GameManager.IsCheat) PlayerController.Instance.SpendCost(CostType.Warmth, cost);

        // 레벨업 실행
        HpLevelup();
    }
    /// <summary>
    /// 플레이어 이동속도 스탯 강화를 시도하는 함수
    /// </summary>
    /// <param name="warmth"></param>
    public void TrySpeedLevelup(long warmth)
    {
        // 현재 레벨 체크
        if (SpeedLevel == 300)
        {
            Debug.Log("최대 레벨입니다.");
            return;
        }

        // 비용 체크
        long cost = DataManager.Instance.GetStatCost(StatDataType.Speed, SpeedLevel);
        if (cost > warmth && !GameManager.IsCheat)
        {
            Debug.Log($"온기가 부족합니다. {cost} > {warmth}");
            return;
        }

        // 비용 감소
        if (!GameManager.IsCheat) PlayerController.Instance.SpendCost(CostType.Warmth, cost);

        // 레벨업 실행
        SpeedLevelup();
    }
}

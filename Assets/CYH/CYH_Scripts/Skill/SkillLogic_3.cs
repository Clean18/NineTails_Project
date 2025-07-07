using System.Collections;
using UnityEngine;

public class SkillLogic_3 : SkillLogic, ISkill
{
    [SerializeField] private ActiveSkillData _data;
    //[SerializeField] private PlayerControllerTypeA_Copy _playerController;
    [SerializeField] private CircleCollider2D _hitBox;
    [SerializeField] private float _radius = 2f;
    [SerializeField] GameObject _highestMonster;

    [Header("데미지 코루틴 (초)")]
    [SerializeField] private float _damageInterval = 0.2f;
    [Header("이펙트 활성 지속 시간 (초)")]
    [SerializeField] private float _effectDuration = 0.1f;
    [Header("데미지 이펙트 프리팹")]
    [SerializeField] private GameObject _damageEffectPrefab;

    [Header("이펙트 Y 오프셋")]
    [SerializeField] private float _effectYOffset = 0.5f;
    [field: SerializeField] public ActiveSkillData SkillData { get; set; }
    [field: SerializeField] public bool IsCooldown { get; set; }
    [field: SerializeField] public int SkillLevel { get; set; }
    [field: SerializeField] public int SlotIndex { get; set; }
    [field: SerializeField] public float RemainCooldown { get; set; }

    public void SkillInit()
    {
        Debug.Log("스킬 3 초기화");
        IsCooldown = false;
        SkillLevel = 0;
        SlotIndex = -1;
    }

    public void SkillInit(SaveSkillData playerSkillData)
    {
        SlotIndex = playerSkillData.SlotIndex;
        IsCooldown = playerSkillData.SkillCooldown > 0f;
        if (IsCooldown) PlayerController.Instance.StartCoroutine(CooldownCoroutine(playerSkillData.SkillCooldown));
    }
    public bool UseSkill(Transform attacker)
    {
        // 쿨타임이면 return
        if (IsCooldown || !PlayerController.Instance.MoveCheck()) return false;

        // 쿨타임 전에 몬스터가 있으면 실행 없으면 return
        // 스킬 발동 전 몬스터 목록 초기화
        _hitMonsters.Clear();
        DetectMonster();
        if (_hitMonsters.Count <= 0)
        {
            Debug.Log("스킬 3 공격할 대상이 없습니다.");
            return false;
        }

        // 쿨타임 체크 시작
        IsCooldown = true;
        PlayerController.Instance.StartCoroutine(CooldownCoroutine());

        AnimationPlay();

        OnAttackStart();
        GetHighestHpMonster();
        Debug.Log("스킬 3 사용완료");
        return true;
    }

    public bool UseSkill(Transform attacker, Transform defender)
    {
        // 쿨타임이면 return
        if (IsCooldown || !PlayerController.Instance.MoveCheck()) return false;

        // 쿨타임 전에 몬스터가 있으면 실행 없으면 return
        // 스킬 발동 전 몬스터 목록 초기화
        _hitMonsters.Clear();
        DetectMonster();
        if (_hitMonsters.Count <= 0)
        {
            Debug.Log("스킬 3 공격할 대상이 없습니다.");
            return false;
        }

        // 쿨타임 체크 시작
        IsCooldown = true;
        PlayerController.Instance.StartCoroutine(CooldownCoroutine());

        AnimationPlay();

        OnAttackStart();
        GetHighestHpMonster();
        Debug.Log("스킬 3 사용완료");
        return true;
    }

    public void SkillRoutine()
    {
        if (_highestMonster != null)
            PlayerController.Instance.StartCoroutine(DamageCoroutine(_highestMonster));
    }

    public void AnimationPlay()
    {
        //_animator.SetTrigger("UseSkill_3");
        PlayerController.Instance.SetTrigger("UseSkill_3");

        // 3초 뒤 플레이어 이동 활성화
        Invoke("PlayerMove", 3f);
    }

    public void OnAttackStart()
    {
        _isSkillUsed = true;

        PlayerController.Instance.Stop();
    }

    public void OnAttackEnd()
    {
        _isSkillUsed = false;
        PlayerController.Instance.Move();
    }

    private void PlayerMove()
    {
        PlayerController.Instance.Move();
    }

    private void DetectMonster()
    {
        Vector3 center = transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, _radius);

        foreach (var col in hits)
        {
            if (col.CompareTag("Monster"))
            {
                _hitMonsters.Add(col.gameObject);
                Debug.Log($"{col.gameObject.name} 감지");
            }
        }
    }

    // 피격 몬스터 리스트 중 가장 체력이 높은 몬스터 반환
    private GameObject GetHighestHpMonster()
    {
        Debug.Log($"감지된 몬스터 수: {_hitMonsters.Count}");

        _highestMonster = null;
        float highestHp = float.MinValue;

        foreach (var monster in _hitMonsters)
        {
            float hp = 0f;

            // 1) <MonsterFSM>인지 체크
            if (monster.TryGetComponent<BaseBossFSM>(out var mM))
                hp = mM.CurrentHealth;

            // 2) <BaseBossFSM>인지 체크
            else if (monster.TryGetComponent<BaseBossFSM>(out var mB))
                hp = mB.CurrentHealth;

            // 둘 중 하나라도 만족할 때
            if (hp > highestHp)
            {
                highestHp = hp;
                _highestMonster = monster;
            }
        }

        if (_highestMonster != null)
            Debug.Log($"최고 체력 몬스터: {_highestMonster.name} : HP {highestHp}");
        else
            Debug.Log("감지된 몬스터 x");

        return _highestMonster;
    }

    protected override void Damage(GameObject monster)
    {
        //float damage = _playerController.AttackPoint * (1.0f + 0.01f * SkillLevel);
        long damage = (long)(PlayerController.Instance.GetTotalDamage() * (1.0f + 0.01f * SkillLevel));
        monster?.GetComponent<IDamagable>().TakeDamage((long)damage);
        //Debug.Log($"{_highestMonster.name}에게 {damage}의 피해를 가했음");
    }
    
    #region Coroutine
    private IEnumerator DamageCoroutine(GameObject monster)
    {
        GameObject effect = null;
        SpriteRenderer effectSprite = null;

        // 최고 체력 몬스터 자식으로 이펙트 생성
        if (_damageEffectPrefab != null)
        {
            // 이펙트 생성 y 값 위치 조정
            Vector3 spawnPos = monster.transform.position + Vector3.up * _effectYOffset;
            effect = Instantiate(_damageEffectPrefab, spawnPos, Quaternion.identity, monster.transform);
            effect.SetActive(false);
            effectSprite = effect.GetComponent<SpriteRenderer>();
        }
        else
            Debug.Log("_damageEffectPrefab 없음");

        // 2) 5번 반복
        for (int i = 0; i < 5; i++)
        {
            if (effect != null)
            {
                // 매 반복마다 flipX 반전
                effectSprite.flipX = !effectSprite.flipX;
                effect.SetActive(true);
            }

            // 데미지
            Damage(monster);
            yield return new WaitForSeconds(_effectDuration);

            effect.SetActive(false);

            // 총 루프 시간 _damageInterval
            yield return new WaitForSeconds(_damageInterval - _effectDuration);
        }
        // 이펙트 제거
        Destroy(effect);
    }

    private IEnumerator CooldownCoroutine()
    {
        RemainCooldown = PlayerController.Instance.GetCalculateCooldown(SkillData.CoolTime);
        Debug.Log($"{SkillData.SkillIndex}번 스킬 쿨타임 {RemainCooldown} 초");
        while (RemainCooldown > 0f)
        {
            yield return new WaitForSeconds(1f);
            RemainCooldown -= 1f;
        }
        RemainCooldown = 0f;
        IsCooldown = false;
        Debug.Log($"{SkillData.SkillIndex}번 스킬 쿨타임 종료");
    }
    private IEnumerator CooldownCoroutine(float reamainCooldown)
    {
        RemainCooldown = reamainCooldown;
        Debug.Log($"{SkillData.SkillIndex}번 스킬 쿨타임 {RemainCooldown} 초");
        while (RemainCooldown > 0f)
        {
            yield return new WaitForSeconds(1f);
            RemainCooldown -= 1f;
        }
        RemainCooldown = 0f;
        IsCooldown = false;
        Debug.Log($"{SkillData.SkillIndex}번 스킬 쿨타임 종료");
    }
    #endregion

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, _radius);
    //}
}

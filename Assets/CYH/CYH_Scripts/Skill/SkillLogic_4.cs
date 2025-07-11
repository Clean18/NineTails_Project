using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class SkillLogic_4 : SkillLogic, ISkill
{
    //[SerializeField] private ActiveSkillData _data;
    //[SerializeField] private PlayerControllerTypeA_Copy _playerController;

    [SerializeField] private CircleCollider2D _hitBox;
    [SerializeField] private float _radius = 2f;

    [Header("랜덤 대상 개수")]
    [SerializeField] private int _randomTargetCount = 6;

    [SerializeField] List<GameObject> _randomMonsters = new List<GameObject>();

    [Header("데미지 코루틴 (초)")]
    [SerializeField] private float _damageInterval = 0.5f;
    [Header("이펙트 활성 지속 시간 (초)")]
    [SerializeField] private float _effectDuration = 0.5f;
    [Header("데미지 이펙트 프리팹")]
    [SerializeField] private GameObject _damageEffectPrefab;
    [Header("이펙트 오프셋")]
    [SerializeField] private float _effectOffset = 0.5f;

    [field: SerializeField] public ActiveSkillData SkillData { get; set; }
    [field: SerializeField] public bool IsCooldown { get; set; }
    [field: SerializeField] public int SkillLevel { get; set; }
    [field: SerializeField] public int SlotIndex { get; set; }

    [field: SerializeField] public float RemainCooldown { get; set; }

    public void SkillInit()
    {
        Debug.Log("스킬 4 초기화");
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
        Debug.Log("스킬 4 UseSkill");
        // 쿨타임이면 return
        if (IsCooldown || !PlayerController.Instance.MoveCheck() || IsSkillUsed) return false;

        // 쿨타임 전에 몬스터가 있으면 실행 없으면 return
        // 스킬 발동 전 몬스터 목록 초기화
        _hitMonsters.Clear();
        _randomMonsters.Clear();
        DetectMonster();
        if (_hitMonsters.Count <= 0)
        {
            Debug.Log("스킬 4 공격할 대상이 없습니다.");
            return false;
        }

        // 쿨타임 체크 시작
        IsCooldown = true;

        PlayerController.Instance.StartCoroutine(CooldownCoroutine());

        // 스킬 사운드
        PlayerController.Instance.PlaySkillSound(SkillData.SkillAudioClip);

        OnAttackStart();
        AnimationPlay();
        //DetectMonster();

        Debug.Log("스킬 4 사용완료");
        return true;
    }

    public bool UseSkill(Transform attacker, Transform defender)
    {
        Debug.Log("스킬 4 UseSkill");
        // 쿨타임이면 return
        if (IsCooldown || !PlayerController.Instance.MoveCheck() || IsSkillUsed) return false;


        // 쿨타임 전에 몬스터가 있으면 실행 없으면 return
        // 스킬 발동 전 몬스터 목록 초기화
        _hitMonsters.Clear();
        _randomMonsters.Clear();
        DetectMonster();
        if (_hitMonsters.Count <= 0)
        {
            Debug.Log("스킬 4 공격할 대상이 없습니다.");
            return false;
        }

        // 쿨타임 체크 시작
        IsCooldown = true;
        
        PlayerController.Instance.StartCoroutine(CooldownCoroutine());

        // 스킬 사운드
        PlayerController.Instance.PlaySkillSound(SkillData.SkillAudioClip);

        OnAttackStart();
        AnimationPlay();
        //DetectMonster();

        Debug.Log("스킬 4 사용완료");
        return true;
    }

    // 애니메이션 종료 시 호출 (애니메이션 이벤트)
    public void SkillRoutine()
    {
        RandomDamage();
        // 데미지 적용 직후로 수정
        //HealPlayer(_randomMonsters.Count);

        // 3초 동안 0.5초마다 데미지 + 이펙트 flipX 토글
        if (_randomMonsters.Count > 0) PlayerController.Instance.StartCoroutine(TimedDamageCoroutine());

        OnAttackEnd();
    }

    public void AnimationPlay()
    {
        //_animator.SetTrigger("UseSkill_4");
        PlayerController.Instance.SetTrigger("UseSkill_4");
        
        // 1초 뒤 플레이어 움직임 활성화
        Invoke("PlayerMove", 1f);
    }

    public void OnAttackStart()
    {
        //_isSkillUsed = true;
        IsSkillUsed = true;

        // 플레이어 이동 비활성화
        PlayerController.Instance.Stop();
        Debug.Log($"플레이어 강제 정지 : {PlayerController.Instance.MoveCheck()}");
    }

    public void OnAttackEnd()
    {
        //_isSkillUsed = false;
        IsSkillUsed = false;
        PlayerController.Instance.Move();
        Debug.Log($"플레이어 정지 해제 : {PlayerController.Instance.MoveCheck()}");
    }

    private void PlayerMove()
    {
        PlayerController.Instance.Move();
    }

    // 범위 안의 모든 몬스터 탐색
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

    // 피격 몬스터 목록으로 새 랜덤 목록 생성
    private void RandomDamage()
    {
        if (_hitMonsters.Count == 0) return;

        if (_hitMonsters.Count <= _randomTargetCount)
        {
            Debug.Log("피격 몬스터 전부 리스트에 추가");
            _randomMonsters.AddRange(_hitMonsters);

        }
        else
        {
            List<GameObject> tempList = new List<GameObject>(_hitMonsters);

            // _randomTargetCount번 뽑고 뽑을 때마다 제거 -> 중복 방지
            for (int i = 0; i < _randomTargetCount; i++)
            {
                int index = Random.Range(0, tempList.Count);
                _randomMonsters.Add(tempList[index]);
                tempList.RemoveAt(index);
            }
        }
        // 랜덤 리스트의 모든 몬스터 데미지 적용
        //foreach (var monster in _randomMonsters)
        //{
        //    Debug.Log($"_randomMonsters : {monster.name}");
        //    Damage(monster);
        //}
    }

    // 스킬 사용 시 플레이어 체력 회복 (_randomMonsters.Count 만큼)
    //private void HealPlayer(int count)
    //{
    //    if (_randomMonsters.Count == 0) return;
    //    //_playerController.hp += _playerController.maxHp * (0.05f + 0.0005f * SkillLevel) * count;
    //    long baseHeal = PlayerController.Instance.GetMaxHp() * (long)(0.05f + 0.0005f * SkillLevel) * count;
    //    long healAmount = System.Math.Max(1, baseHeal);
    //    PlayerController.Instance.TakeHeal(healAmount);
    //    //Debug.Log($"몬스터 [{count}]마리에게 데미지를 가해 총 [{_playerController.maxHp * (0.05f + 0.0005f * SkillLevel) * count}]의 Hp를 회복");
    //    Debug.Log($"몬스터 [{count}]마리에게 데미지를 가해 총 [{healAmount}]의 Hp를 회복");
    //}

    // 플레이어 체력 회복
    private void HealPlayer()
    {
        if (_randomMonsters.Count == 0) return;
        //_playerController.hp += _playerController.maxHp * (0.05f + 0.0005f * SkillLevel);
        long baseHeal = (long)(PlayerController.Instance.GetMaxHp() * (0.05f + 0.0005f * SkillLevel));
        long healAmount = System.Math.Max(1, baseHeal);
        PlayerController.Instance.TakeHeal(healAmount);
        //Debug.Log($"몬스터 [{count}]마리에게 데미지를 가해 총 [{_playerController.maxHp * (0.05f + 0.0005f * SkillLevel) * count}]의 Hp를 회복");
        Debug.Log($"Hp [{healAmount}] 회복");
    }

    protected override void Damage(GameObject monster)
    {
        //float damage = (float)(_playerController.AttackPoint * (0.15f + 0.0015f * SkillLevel));
        long damage = (long)(PlayerController.Instance.GetTotalDamage() * (0.15f + 0.0015f * SkillLevel));
        monster?.GetComponent<IDamagable>().TakeDamage((long)damage);
        //Debug.Log($"{monster.name}에게 {damage}의 피해를 가했음");
    }

    // 데미지, 이펙트 코루틴 (0.5초 간격으로 3초 동안)
    private IEnumerator TimedDamageCoroutine()
    {
        // 이펙트 관리 리스트
        var effects = new List<GameObject>();

        if (_damageEffectPrefab != null)
        {
            foreach (var monster in _randomMonsters)
            {
                Vector3 spawnPos = monster.transform.position + Vector3.up * _effectOffset;
                var effect = Instantiate(_damageEffectPrefab, spawnPos, Quaternion.identity, monster.transform);

                // 이펙트 크기 조정
                effect.transform.localScale = _damageEffectPrefab.transform.localScale;
                effects.Add(effect);
            }
        }

        // 경과 시간, flip 초기화
        float time = 0f;
        bool flip = false;

        while (time < 3f)
        {
            // 0.5초마다 데미지 적용, 플레이어 체력 회복
            foreach (var monster in _randomMonsters)
            {
                if (monster.activeSelf)
                {
                    Damage(monster);
                    HealPlayer();
                }
            }

            //Debug.Log("Damage 적용");

            // 이펙트 flipX
            foreach (var effect in effects)
            {
                var effectSprite = effect.GetComponent<SpriteRenderer>();
                effectSprite.flipX = flip;
            }
            flip = !flip;

            yield return new WaitForSeconds(_damageInterval);
            time += _damageInterval;
        }
        // 이펙트 삭제
        foreach (var effect in effects)
            Destroy(effect);
        
        // 플레이어 이동 활성화
        PlayerController.Instance.Move();
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

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, _radius);
    //}
}


using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class SkillLogic_6 : SkillLogic, ISkill
{
    //[SerializeField] private ActiveSkillData _data;
    //[SerializeField] private PlayerControllerTypeA_Copy _playerController;

    [Header("데미지 범위")]
    [SerializeField] private Vector2 _hitBoxSize = new Vector2(1, 1);
    
    [Header("데미지 범위 오프셋")]
    [SerializeField] private Vector2 _boxOffset = new Vector2(0, 0);
    [SerializeField] private LayerMask _monsterLayer;

    [Header("데미지 코루틴")]
    [SerializeField] private float _damageInterval = 0.1f;
    [SerializeField] private int _damageCount = 5;

    [SerializeField] private GameObject _videoPrefab;
    [SerializeField] private GameObject _effectPrefab;
    [SerializeField] private VideoPlayer _videoPlayer;

    [field: SerializeField] public ActiveSkillData SkillData { get; set; }
    [field: SerializeField] public bool IsCooldown { get; set; }
    [field: SerializeField] public int SkillLevel { get; set; }
    [field: SerializeField] public int SlotIndex { get; set; }

    //public PlayerController PlayerController { get; set; }

    public void SkillInit()
    {
        Debug.Log("스킬 6 초기화");
        IsCooldown = false;
        SkillLevel = 0;
        SlotIndex = -1;
    }

    public void UseSkill(Transform attacker)
    {
        // 쿨타임이면 return
        if (IsCooldown) return;
        Debug.Log($"IsCooldown: {IsCooldown}");
        if (!PlayerController.Instance.MoveCheck()) return;

        Debug.Log("스킬6 사용");

        // 무적 시작
        PlayerController.Instance.IsImmortal = true;

        // 쿨타임 체크 시작
        IsCooldown = true;
        PlayerController.Instance.StartCoroutine(CooldownCoroutine());

        // 스킬 발동 전 몬스터 목록 초기화
        _hitMonsters.Clear();

        OnAttackStart();
        CreateVideo(transform.position);
        DetectMonster();
    }

    public void UseSkill(Transform attacker, Transform defender)
    {
        Debug.Log("스킬 6 UseSkill");
        // 쿨타임이면 return
        if (IsCooldown) return;
        Debug.Log($"IsCooldown: {IsCooldown}");
        if (!PlayerController.Instance.MoveCheck()) return;

        // 쿨타임 체크 시작
        IsCooldown = true;
        PlayerController.Instance.StartCoroutine(CooldownCoroutine());

        // 스킬 발동 전 몬스터 목록 초기화
        _hitMonsters.Clear();

        OnAttackStart();
        CreateVideo(transform.position);
        DetectMonster();
        Debug.Log("스킬 5 사용완료");
    }

    public void SkillRoutine()
    {
        PlayerController.Instance.StartCoroutine(DamageRoutine());
        OnAttackEnd();
    }

    public void OnAttackStart()
    {
        _isSkillUsed = true;
    }

    public void OnAttackEnd()
    {
        // 무적 해제
        PlayerController.Instance.IsImmortal = false;
        _isSkillUsed = false;
    }

    public void AnimationPlay()
    {
        //_animator.SetTrigger("UseSkill_6");
        PlayerController.Instance.SetTrigger("UseSkill_6");
    }

    // 궁극기 비디오 생성
    private void CreateVideo(Vector3 position)
    {
        Transform camera = transform.Find("Main Camera");
        GameObject video = Instantiate(_videoPrefab, position, Quaternion.identity, camera);

        VideoPlayer videoPlayer = video.GetComponentInChildren<VideoPlayer>();
        videoPlayer.Play();

        Destroy(video, 4f);
        PlayerController.Instance.StartCoroutine(PlayVideoDelayed(4f));
    }

    // 피격 몬스터 감지
    private void DetectMonster()
    {
        float offsetX = _boxOffset.x * Mathf.Sign(transform.localScale.x);
        Vector2 center = (Vector2)transform.position + new Vector2(offsetX, _boxOffset.y);

        Collider2D[] monsters = Physics2D.OverlapBoxAll(center, _hitBoxSize, 0f, _monsterLayer);
        foreach (var monster in monsters)
        {
            _hitMonsters.Add(monster.gameObject);
        }
    }

    // 데미지 적용
    protected override void Damage(GameObject monster)
    {
        //float damage = _playerController.AttackPoint * (4.0f + 0.04f * SkillLevel);
        long damage = (long)(PlayerController.Instance.GetTotalDamage() * (4.0f + 0.04f * SkillLevel));
        monster?.GetComponent<IDamagable>().TakeDamage((long)damage);
        //Debug.Log($"{_highestMonster.name}에게 {damage}의 피해를 가했음");
    }

    // 데미지 코루틴
    private IEnumerator DamageRoutine()
    {
        for (int i = 0; i < _damageCount; i++)
        {
            foreach (var monster in _hitMonsters)
                Damage(monster);
            yield return new WaitForSeconds(_damageInterval);
        }
    }

    private IEnumerator PlayVideoDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        // 애니메이션 플레이
        AnimationPlay();
        // 스킬 이펙트 프리팹 생성
        GameObject effect = Instantiate(_effectPrefab, transform.position, Quaternion.identity);
        // 3초 뒤 삭제
        Destroy(effect, 3f);
    }

    private IEnumerator CooldownCoroutine()
    {
        float remaining = PlayerController.Instance.GetCalculateCooldown(SkillData.CoolTime);
        Debug.Log($"6번 스킬 쿨타임 {remaining} 초");
        while (remaining > 0f)
        {
            Debug.Log($"6번 스킬 쿨타임 남음: {remaining}초");
            yield return new WaitForSeconds(1f);
            remaining -= 1f;
        }
        IsCooldown = false;
        Debug.Log("6번 스킬 쿨타임 종료");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float offsetX = _boxOffset.x * Mathf.Sign(transform.localScale.x);
        Vector3 center = transform.position + new Vector3(offsetX, _boxOffset.y, 0f);
        Vector3 boxSize = new Vector3(_hitBoxSize.x, _hitBoxSize.y, 0.01f);

        Gizmos.DrawWireCube(center, boxSize);
    }
}
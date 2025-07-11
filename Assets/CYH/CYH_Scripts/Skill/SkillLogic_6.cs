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

    [Header("이펙트 생성 오프셋")]
    [SerializeField] private Vector2 _effectOffset = new Vector2(0, 3);

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

    [field: SerializeField] public float RemainCooldown { get; set; }

    public void SkillInit()
    {
        Debug.Log("스킬 6 초기화");
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
        if (IsCooldown || !PlayerController.Instance.MoveCheck() || IsSkillUsed) return false;

        Debug.Log("스킬6 사용");

        // 무적 시작
        GameManager.IsImmortal = true;

        // 쿨타임 체크 시작
        IsCooldown = true;
        PlayerController.Instance.StartCoroutine(CooldownCoroutine());

        // 스킬 사운드
        PlayerController.Instance.PlaySkillSound(SkillData.SkillAudioClip);

        // 스킬 발동 전 몬스터 목록 초기화
        _hitMonsters.Clear();

        OnAttackStart();
        CreateVideo(transform.position);
        DetectMonster();
        Debug.Log("스킬 6 사용완료");
        return true;
    }

    public bool UseSkill(Transform attacker, Transform defender)
    {
        // 쿨타임이면 return
        if (IsCooldown || !PlayerController.Instance.MoveCheck() || IsSkillUsed) return false;

        Debug.Log("스킬6 사용");

        // 무적 시작
        GameManager.IsImmortal = true;

        // 쿨타임 체크 시작
        IsCooldown = true;
        PlayerController.Instance.StartCoroutine(CooldownCoroutine());

        // 스킬 사운드
        PlayerController.Instance.PlaySkillSound(SkillData.SkillAudioClip);

        // 스킬 발동 전 몬스터 목록 초기화
        _hitMonsters.Clear();

        OnAttackStart();
        CreateVideo(transform.position);
        DetectMonster();
        Debug.Log("스킬 6 사용완료");
        return true;
    }

    public void SkillRoutine()
    {
        PlayerController.Instance.StartCoroutine(DamageRoutine());
    }

    public void OnAttackStart()
    {
        //_isSkillUsed = true;
        IsSkillUsed = true;
        PlayerController.Instance.Stop();
    }

    public void OnAttackEnd()
    {
        // 무적 해제
        if (!GameManager.IsCheat) GameManager.IsImmortal = false;
        //_isSkillUsed = false;
        IsSkillUsed = false;
        PlayerController.Instance.Move();
    }

    public void AnimationPlay()
    {
        //_animator.SetTrigger("UseSkill_6");
        PlayerController.Instance.SetTrigger("UseSkill_6");

        // 3초 뒤 플레이어 움직임 활성화
        Invoke("PlayerMove", 3f);
    }

    private void PlayerMove()
    {
        PlayerController.Instance.Move();
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
        float offsetX = Mathf.Abs(_boxOffset.x) * GetPlayerScaleX();

        Vector2 center = (Vector2)transform.position + new Vector2(offsetX, _boxOffset.y);

        Collider2D[] monsters = Physics2D.OverlapBoxAll(center, _hitBoxSize, 0f, _monsterLayer);

        _hitMonsters.Clear();

        foreach (var monster in monsters)
        {
            _hitMonsters.Add(monster.gameObject);
        }
    }

    private float GetPlayerScaleX()
    {
        PlayerController player = PlayerController.Instance;
        return Mathf.Sign(player.transform.localScale.x);
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
        Vector2 skillPos = (Vector2)PlayerController.Instance.transform.position + _effectOffset;
        GameObject effect = Instantiate(_effectPrefab, skillPos, Quaternion.identity);

        // 플레이어가 바라보는 방향에 맞게 이펙트 좌우반전
        Vector3 effectScale = effect.transform.localScale;
        effectScale.x *= -1 * GetPlayerScaleX();
        effect.transform.localScale = effectScale;

        // 플레이어가 왼쪽을 바라볼 때 이펙트 위치 x값 조정
        if (effectScale.x > 0)
        {
            effect.transform.position = new Vector3(-1 * (_effectOffset.x) + transform.position.x, _effectOffset.y+ transform.position.y, 0);
        }

        // 3초 뒤 삭제
        Destroy(effect, 3f);
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
    //    float sign = Mathf.Sign(GetPlayerScaleX());
    //    float offsetX = Mathf.Abs(_boxOffset.x) * sign;
    //    Vector2 center = (Vector2)transform.position + new Vector2(offsetX, _boxOffset.y);
    //    Gizmos.DrawWireCube(center, _hitBoxSize);
    //}
}
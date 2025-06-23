using UnityEngine;

public class BossMonsterFSM : MonoBehaviour
{
    // 보스의 상태를 정의하는 열거형

    enum BossState
    {
        Intro,      // 등장 연출
        Idle,       // 대기 상태 (패턴 전환 준비 상태)
        Pattern1,   // 일반 공격 패턴 1
        Pattern2,   // 일반 공격 패턴 2
        Special,    // 특수 공격 패턴
        Groggy,     // 패턴 공략 성공 시 상태
        Dead        // 체력이 0 이하가되면 사망
    }

    private BossState CurrentState;

    [Header("Boss HP")]
    public float MaxHealth = 1000f;     // 최대 체력
    private float CurrentHealth;        // 현재 체력

    [Header("FSM Timer")]
    public float IdleTime = 3f;         // Idle 상태에서 대기하는 시간 ( 다음 패턴 전환까지 딜레이 )
    private float IdleTimer;            // Idle 상태에서 누적된 시간

    // 시작 시 상태 초기화
    private void Start()
    {
        CurrentHealth = MaxHealth;              // 보스 체력 초기화
        TransitionToState(BossState.Intro);     // Intro 상태로 시작
    }

    // 매 프레임 상태에 따라서 처리
    private void Update()
    {
        switch (CurrentState)
        {
            case BossState.Intro:
                HandleIntro(); break;

            case BossState.Idle:
                HandleIdle(); break;

            case BossState.Pattern1:
                HandlePattern1(); break;

            case BossState.Pattern2:
                HandlePattern2(); break;

            case BossState.Special:
                HandleSpecial(); break;

            case BossState.Groggy:
                HandleGroggy(); break;

            case BossState.Dead:
                HandleDead(); break;
        }
    }

    private void TransitionToState(BossState newstate)
    {
        CurrentState = newstate;
        IdleTimer = 0f; // 상태 변화 시 타이머 초기회 (Idle 에서만 쓰임)

        // 상태 변화 시 추가 초기화 작업이 필요하면 아래에서 처리
    }

    private void HandleIntro()
    {
        // TODO : 보스 등장 애니메이션이나 카메라 연출 처리

        // 연출 끝났다고 가정하고 Idle로 전환

        TransitionToState(BossState.Idle);
    }

    // Idle 상태 처리 ( 패턴 전환 대기 상태 )
    private void HandleIdle()
    {
        IdleTimer += Time.deltaTime;

        // 대기 시간이 충분히 지나면 랜덤으로 다음 패턴 선택
        if (IdleTimer >= IdleTime)
        {
            int rand = Random.Range(0, 3); // 0~2중에 하나 선택
            switch (rand)
            {
                case 0:
                    TransitionToState(BossState.Pattern1);
                    break;
                case 1:
                    TransitionToState(BossState.Pattern2);
                    break;
                case 2:
                    TransitionToState(BossState.Special);
                    break;

            }
        }
    }

    // 일반 패턴 1 처리

    private void HandlePattern1()
    {
        // TODO : 전방 탄막 발사 같은 행동 패턴1 실행

        // 이후 Idle 상태로 돌아가야함
        TransitionToState(BossState.Idle);
    }

    // 일반 패턴 2 처리

    private void HandlePattern2()
    {
        // TODO : 회전하면서 탄을 발사한다거나 하는 패턴 2 실행

        // 이후 Idle 상태로 되돌아감
        TransitionToState(BossState.Idle);

    }

    // 특수 패턴 처리
    private void HandleSpecial()
    {
        // TODO : 특수 기믹패턴 같은 특수 행동 처리 , 처리 결과에 따라서 Idle로 갈지 Groggy로 갈지 
    }

    // 그로기 상태 처리
    private void HandleGroggy()
    {
        //TODO : 특수 패턴 처리에서 성공 조건 달성 시 움직임이 없는 기절상태 같은 애니메이션이나 이미지

        // 일정 시간 후 Idle 로 복귀
        TransitionToState(BossState.Idle);
    }

    private bool isDeadHandled = false;
    // 사망 처리
    private void HandleDead()
    {
        //TODO : 죽는 애니메이션, 드랍아이템, 클리어 처리 UI 출력 등
        // Animator.SetTrigger("Die") 의 구현?

        if (!isDeadHandled)
        {
            isDeadHandled = true;
            // 1. 죽음 애니메이션 이나 사운드의 실행
            Debug.Log("보스 사망 연출 시작");
            // 2. 스테이지 클리어 UI 나 연출 등의 추가

            // 3. 일정 시간 후 보스 오브젝트 제거
            Invoke(nameof(DestroySelf), 3f); // 3초 후 제거
        }

        // 이 상태에서는 아무것도 안함 ( 죽음 애니메이션 대기 )
    }

    private void DestroySelf()
    {
        Debug.Log("보스 오브젝트 제거됨");
        Destroy(gameObject);
    }
}

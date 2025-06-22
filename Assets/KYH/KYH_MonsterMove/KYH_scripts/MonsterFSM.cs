using System.Diagnostics;
using UnityEngine;

public class MonsterFSM : MonoBehaviour
{
    // ������ ���¸� ��Ÿ���� ������
    enum MonsterState { Idle, Move, Attack }

    [Header("Monster status")]
    public float MoveSpeed = 2f;          // �̵� �ӵ�
    public float DetectRange = 5f;        // �÷��̾ �����ϴ� ����
    public float AttackRange = 1.5f;      // ���� ������ �Ÿ�
    public float AttackCooldown = 2f;     // ���� ��ٿ� �ð�

    [Header("Search Player Cooldown")]
    public float FindInterval = 1.0f;     // �� �ʸ��� �÷��̾ ��Ž������ ����

    private MonsterState _currentState = MonsterState.Idle; // ���� ����
    private float _attackTimer;       // ���� ��ٿ� Ÿ�̸�
    private float _findTimer;         // �÷��̾� ��Ž�� Ÿ�̸�

    private Transform targetPlayer;   // ���� ���� ���� �÷��̾�

    private void Update()
    {
        // �÷��̾� ��Ž�� Ÿ�̸� ����
        _findTimer += Time.deltaTime;

        // ������ �ֱ⸶�� �÷��̾� �ٽ� Ž��
        if (_findTimer >= FindInterval)
        {
            FindClosestPlayer();
            _findTimer = 0;
        }

        // Ÿ�� �÷��̾ ������ Idle ���� ����
        if (targetPlayer == null)
        {
            ChangeState(MonsterState.Idle);
            return;
        }

        // ���� �÷��̾���� �Ÿ� ���
        float dist = Vector2.Distance(transform.position, targetPlayer.position);

        // ���� ���� ���� ó��
        switch (_currentState)
        {
            case MonsterState.Idle:
                if (dist < DetectRange)
                    ChangeState(MonsterState.Move);
                break;

            case MonsterState.Move:
                if (dist < AttackRange)
                    ChangeState(MonsterState.Attack);
                else if (dist >= AttackRange)
                    ChangeState(MonsterState.Idle);
                break;

            case MonsterState.Attack:
                if (dist > AttackRange)
                    ChangeState(MonsterState.Move);
                break;

        }

        // ���� ���¿� ���� �ൿ ����
        switch (_currentState)
        {
            case MonsterState.Idle:
                // �ƹ� �ൿ �� ��
                break;

            case MonsterState.Move:
                MoveToPlayer(); // �÷��̾� ������ �̵�
                break;

            case MonsterState.Attack:
                HandelAttack(); // ���� ó��
                break;
        }
    }

    /// <summary>
    /// �� ��ü���� "Player" �±׸� ���� ������Ʈ���� ã��
    /// ���� ����� �÷��̾ ���� ������� ����
    /// </summary>
    void FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (var playerobj in players)
        {
            if (!playerobj.activeInHierarchy) continue;

            float dist = Vector2.Distance(transform.position, playerobj.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = playerobj.transform;
            }
        }

        targetPlayer = closest;
    }

    /// <summary>
    /// ���� ���� ó�� �� �ʱ�ȭ
    /// </summary>
    void ChangeState(MonsterState newstate)
    {
        if (_currentState == newstate) return;
        UnityEngine.Debug.Log($"State changed : {_currentState} �� {newstate}");
        _currentState = newstate;
        _attackTimer = 0f; // ���� ��ٿ� �ʱ�ȭ
    }

    /// <summary>
    /// �÷��̾� ������ �̵��ϴ� ����
    /// </summary>
    void MoveToPlayer()
    {
        Vector2 dir = (targetPlayer.position - transform.position).normalized;
        transform.position += (Vector3)dir * MoveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// ���� ����: ��ٿ��� ����� ���� ����
    /// </summary>
    void HandelAttack()
    {
        _attackTimer += Time.deltaTime;
        if (_attackTimer >= AttackCooldown)
        {
            UnityEngine.Debug.Log("���Ͱ� �÷��̾ ����!");
            // ���⼭ ������ �������� �ִ� �Լ� ȣ�� ����
            _attackTimer = 0f;
        }
    }

    /// <summary>
    /// Unity Editor���� ����/���� ���� �ð�ȭ
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, DetectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}

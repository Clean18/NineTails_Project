using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SJH_Monster : MonoBehaviour
{
    public Transform _targetTransform;
	public Rigidbody2D rigid;
	public int MaxHp;
	public int Hp { get; private set; }
	public float MoveSpeed;

    // 몬스터의 AI는 Idle, Chase, Attack
    public AIState MonsterState;

	void Awake()
	{
		rigid= GetComponent<Rigidbody2D>();
        MonsterState = AIState.Idle;
	}

	void OnEnable()
	{
		if (GameManager.Instance.PlayerController != null) _targetTransform = GameManager.Instance.PlayerController.transform;
	}

	void OnDisable()
	{
		_targetTransform = null;
	}

	void Update()
	{
		if (!gameObject.activeSelf) return;

        switch (MonsterState)
        {
            case AIState.Idle: IdleAction(); break;
            case AIState.Chase: ChaseAction(); break;
            case AIState.Attack: AttackAction(); break;
        }

		Vector3 dir = (_targetTransform.position - transform.position).normalized;
		rigid.velocity = dir * MoveSpeed;
	}

	public void TakeDamage(int damage)
	{
        Debug.Log($"{damage} 의 피해를 입어 체력이 {Hp}");
		Hp -= damage;

        if (Hp <= 0)
        {
            Debug.Log("몬스터 사망");
            gameObject.SetActive(false);
            Hp = MaxHp;
        }
	}

    public void IdleAction()
    {
        if (!GameManager.Instance.PlayerController.PlayerModel.Data.IsDead) MonsterState = AIState.Chase;
    }

    public void ChaseAction()
    {

    }

    public void AttackAction()
    {

    }
}

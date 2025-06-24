using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SJH_Monster : MonoBehaviour
{
    public Transform _targetTransform;
	public Rigidbody2D rigid;
	public int MaxHp;
	[SerializeField] private int _hp;
	public int Hp
	{
		get => _hp;
		set
		{
			_hp = value;
			if (_hp < 0)
			{
				gameObject.SetActive(false);
				_hp = MaxHp;
			}
		}
	}
	public float MoveSpeed;

	void Awake()
	{
		rigid= GetComponent<Rigidbody2D>();
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
		Vector3 dir = (_targetTransform.position - transform.position).normalized;
		rigid.velocity = dir * MoveSpeed;
	}

	public void TakeDamage(int damage)
	{
		Hp -= damage;
	}
}

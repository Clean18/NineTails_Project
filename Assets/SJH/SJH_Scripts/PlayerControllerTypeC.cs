using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerTypeC : MonoBehaviour
{
	public float moveSpeed;
	public float attackSpeed;
	public Rigidbody2D rigid;
	public GameObject effectPrefab;

	public Vector3 targetPos;

	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(1)) MouseClick();
		if (Input.GetKeyDown(KeyCode.Q)) Attack();
	}

	void FixedUpdate()
	{
		Vector2 dir = (targetPos - transform.position);
		if (dir.magnitude > 0.1f)
		{
			Vector2 movePos = dir.normalized * moveSpeed;
			rigid.velocity = movePos;
		}
		else
		{
			rigid.velocity = Vector2.zero;
		}
	}

	void MouseClick()
	{
		targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		targetPos.z = 0;
	}

	void Attack()
	{
		Vector3 attackDir = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		attackDir.z = 0; // z를 0으로 해야 일정함

		Vector3 spawnPos = transform.position;
		Vector3 dir = (attackDir - spawnPos).normalized;

		spawnPos = transform.position + dir;

		var go = Instantiate(effectPrefab, spawnPos, transform.rotation);
		go.GetComponent<Rigidbody2D>().AddForce(dir * attackSpeed, ForceMode2D.Impulse);
	}
}

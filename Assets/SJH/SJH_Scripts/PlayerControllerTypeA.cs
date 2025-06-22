using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerTypeA : MonoBehaviour
{
    // wasd 이동 마우스 에임
    public float moveSpeed;
	public float attackSpeed;

	public Rigidbody2D rigid;
	public Vector2 moveInput;

	public GameObject effectPrefab;

	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
	}

	void Update()
	{
		// 물리 무시 이동
		//float x = Input.GetAxis("Horizontal");
		//float y = Input.GetAxis("Vertical");

		//Vector3 movePos = new Vector3(x, y).normalized;
		//transform.position += movePos * moveSpeed * Time.deltaTime;

		moveInput.x = Input.GetAxis("Horizontal");
		moveInput.y = Input.GetAxis("Vertical");

		if (Input.GetMouseButtonDown(0)) Attack();
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

	void FixedUpdate()
	{
		Vector2 movePos = moveInput.normalized * moveSpeed;
		rigid.velocity = movePos;
	}
}

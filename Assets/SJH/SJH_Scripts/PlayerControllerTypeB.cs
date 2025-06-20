using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerTypeB : MonoBehaviour
{
	public float moveSpeed;
	public float attackSpeed;
	public Rigidbody2D rigid;
	public Vector2 moveInput;
	public Vector2 attackDir;
	public GameObject effectPrefab;

	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
	}

	void Update()
	{
		moveInput.x = Input.GetAxisRaw("Horizontal");
		moveInput.y = Input.GetAxisRaw("Vertical");

		if (moveInput != Vector2.zero)
			attackDir = moveInput.normalized;

		if (Input.GetKeyDown(KeyCode.Q)) Attack();
	}

	void FixedUpdate()
	{
		Vector2 movePos = moveInput.normalized * moveSpeed;
		rigid.velocity = movePos;
	}

	void Attack()
	{
		Instantiate(effectPrefab, transform.position, transform.rotation).GetComponent<Rigidbody2D>().AddForce(attackDir * attackSpeed, ForceMode2D.Impulse);
	}
}

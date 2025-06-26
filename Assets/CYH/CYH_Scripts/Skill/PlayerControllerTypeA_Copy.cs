using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControllerTypeA_Copy : MonoBehaviour
{
    // wasd 이동 마우스 에임
    public float moveSpeed;
	public float attackSpeed;

	public Rigidbody2D rigid;
	public Vector2 moveInput;

	public GameObject effectPrefab;

    private SpriteRenderer spriteRenderer;

    // 추가한 부분
    private SpriteRenderer playerSpriteRenderer;
    public int facingDir;

    void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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


        // 추가한 부분 (플레이어 좌우반전)
        if (moveInput.x < 0f)
        {
            facingDir = -1;
            transform.localScale = new Vector3(1, 1, 1);
            //transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (moveInput.x > 0f)
        {
            facingDir = 1;
            transform.localScale = new Vector3(-1, 1, 1);
            //transform.rotation = Quaternion.Euler(0, 180, 0);
        }
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

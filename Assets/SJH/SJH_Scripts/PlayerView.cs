using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigid;
	void Awake()
	{
		rigid = GetComponent<Rigidbody2D>();
	}

	public void Move(Vector2 dir, float moveSpeed)
	{
		Vector2 movePos = dir.normalized * moveSpeed;
		rigid.velocity = movePos;
	}
}

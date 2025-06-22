using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigid;

	void Awake()
	{
		_rigid = GetComponent<Rigidbody2D>();
	}

	public void Move(Vector2 dir, float moveSpeed)
	{
		Vector2 movePos = dir.normalized * moveSpeed;
		_rigid.velocity = movePos;
	}
}

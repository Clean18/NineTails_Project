using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 오브젝트의 이동, 애니메이션 전환을 처리하는 컴포넌트
/// </summary>
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

	public void Stop() => _rigid.velocity = Vector2.zero;
}

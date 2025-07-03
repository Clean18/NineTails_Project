using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 오브젝트의 이동, 애니메이션 전환을 처리하는 컴포넌트
/// </summary>
public class PlayerView : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigid;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private Animator _anim;

    private float _facingDir;
    [SerializeField] float _spriteSize;

    [SerializeField] private bool canMove;

    void Awake()
	{
		_rigid = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();

        canMove = true;
    }

	public void Move(Vector2 dir, float moveSpeed)
	{
        if (!canMove) return;

        if (dir != Vector2.zero) _anim.SetBool("IsMoving", true);
        else _anim.SetBool("IsMoving", false);

        Vector2 movePos = dir.normalized * moveSpeed;
		_rigid.velocity = movePos;

        if (dir.x < 0f)
        {
            _facingDir = -1;
            transform.localScale = new Vector3(-_spriteSize, _spriteSize, _spriteSize);
            //transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (dir.x > 0f)
        {
            _facingDir = 1;
            transform.localScale = new Vector3(_spriteSize, _spriteSize, _spriteSize);
            //transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

	public void Stop()
    {
        canMove = false;
        _anim.SetBool("IsMoving", false);
        _rigid.velocity = Vector2.zero;
    }

    public void Move() => canMove = true;

    public void AIStop()
    {
        _anim.SetBool("IsMoving", false);
        _rigid.velocity = Vector2.zero;
    }

    public void SetTrigger(string trigger)
    {
        _anim.SetTrigger(trigger);
    }

    public bool GetMoveCheck() => canMove;
}

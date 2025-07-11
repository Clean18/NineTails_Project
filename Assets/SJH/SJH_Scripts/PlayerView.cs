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

        PlayerFlip(dir.x);
    }

    public void PlayerFlip(float dirX)
    {
        if (dirX < 0f)
        {
            _facingDir = -1;
            transform.localScale = new Vector3(-_spriteSize, _spriteSize, _spriteSize);
        }
        else if (dirX > 0f)
        {
            _facingDir = 1;
            transform.localScale = new Vector3(_spriteSize, _spriteSize, _spriteSize);
        }
    }

    /// <summary>
    /// 애니메이션, velocity, 플레이어 이동을 멈추는 함수
    /// </summary>
    public void Stop()
    {
        canMove = false;
        _anim.SetBool("IsMoving", false);
        _rigid.velocity = Vector2.zero;
    }

    public void Move() => canMove = true;

    /// <summary>
    /// 애니메이션과 velocity만 멈추는 함수
    /// </summary>
    public void AIStop()
    {
        _anim.SetBool("IsMoving", false);
        _rigid.velocity = Vector2.zero;
    }

    public void SetTrigger(string trigger)
    {
        _anim.SetTrigger(trigger);
    }

    public void SetBool(string name, bool value)
    {
        _anim.SetBool(name, value);
    }

    public bool GetMoveCheck() => canMove;
}

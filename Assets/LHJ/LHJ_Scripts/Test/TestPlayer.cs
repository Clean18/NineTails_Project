using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private Rigidbody2D rigid;
    private Vector2 move;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        move.x = Input.GetAxisRaw("Horizontal"); 
        move.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        rigid.MovePosition(rigid.position + move * moveSpeed * Time.fixedDeltaTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_CYH : MonoBehaviour
{
    //public Rigidbody2D rigid;
    public int MaxHp;
    public float Hp { get; private set; }
    public float MoveSpeed;

    private void Awake()
    {
        //rigid = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(float damage)
    {
        Hp -= damage;
    }
}

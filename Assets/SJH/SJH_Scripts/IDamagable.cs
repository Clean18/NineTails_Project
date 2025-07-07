using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    public MonsterType Type { get; set; }
    public void TakeDamage(long damage);
}

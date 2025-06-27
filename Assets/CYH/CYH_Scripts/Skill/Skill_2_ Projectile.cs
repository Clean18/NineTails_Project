using UnityEngine;
using System;

public class Skill_2_Projectile : MonoBehaviour
{
    public static event Action<Collider2D> Skill_2_Event;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            //Debug.Log($"{gameObject.name}과 {other.name}!");
            Skill_2_Event?.Invoke(other);
        }
    }
}

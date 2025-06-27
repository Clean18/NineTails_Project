using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterProjectile : MonoBehaviour
{
    private float damage;
    [SerializeField] private float lifeTime = 5f; // 수명 제한

    private void Start()
    {
        Destroy(gameObject, lifeTime); // 일정 시간 후 자동 삭제
    }

    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어 충돌 감지
        if (other.CompareTag("Player"))
        {
            //Game.Data.PlayerData player = other.GetComponent<Game.Data.PlayerData>();
            var player = GameManager.Instance.PlayerController;
            if (player != null)
            {
                //player.TakeDamage(damage);
                player.TakeDamage((long)damage);
                Debug.Log($"투사체가 플레이어에게 {(long)damage} 데미지 줌");
            }

            Destroy(gameObject); // 명중 후 파괴
        }
        else if (other.CompareTag("Wall") || other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject); // 벽이나 바닥에 맞으면 파괴
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 플레이어 충돌 감지
        if (collision.transform.CompareTag("Player"))
        {
            //Game.Data.PlayerData player = other.GetComponent<Game.Data.PlayerData>();
            var player = GameManager.Instance.PlayerController;
            if (player != null)
            {
                //player.TakeDamage(damage);
                player.TakeDamage((long)damage);
                Debug.Log($"투사체가 플레이어에게 {(long)damage} 데미지 줌");
            }

            Destroy(gameObject); // 명중 후 파괴
        }
        else if (collision.transform.CompareTag("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject); // 벽이나 바닥에 맞으면 파괴
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Game.Data;
using UnityEngine;

public class SpearGhostPrefebs : MonoBehaviour
{
    [SerializeField] private float DamagePercent = 0.2f;
    [SerializeField] private float LifeTime = 5f;
    [SerializeField] AudioClip SpearGhostSound;

    private void Start()
    {
        AudioSource.PlayClipAtPoint(SpearGhostSound, transform.position);   
        Destroy(gameObject, LifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"[창귀 충돌] 충돌 대상: {collision.name} / 태그: {collision.tag}");

        if (collision.CompareTag("Player"))
        {
            //Game.Data.PlayerData player = collision.GetComponent<Game.Data.PlayerData>();
            //if (player != null)
            //{
            //    // player.TakeDamageByPercent(DamagePercent);
            //    // Debug.Log("창귀 가 플레이어에게 최대 체력의 20% 데미지");
            //    Debug.Log(" PlayerData 찾음. 데미지 적용 시도 중");

            //    player.TakeDamageByPercent(DamagePercent);

            //    Debug.Log($" 데미지 적용 완료. 적용된 퍼센트: {DamagePercent * 100}");
            //}
            PlayerController.Instance.TakeDamage((long)(PlayerController.Instance.GetMaxHp() * DamagePercent));
            Debug.Log($" 데미지 적용 완료. 적용된 퍼센트: {DamagePercent * 100}");
            Destroy(gameObject);
        }

        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_5_Field : MonoBehaviour
{
    public static event Action<List<GameObject>> Skill_5_Event;

    [SerializeField] private float _detectionRadius = 4f;
    [SerializeField] private float _detectionInterval = 0.5f;
    [SerializeField] private LayerMask _monsterLayer;

    [SerializeField] private float _effectDuration = 10f;

    private WaitForSeconds _wait;


    private void Awake()
    {
        _wait = new WaitForSeconds(_detectionInterval);
    }

    private IEnumerator DetectCoroutine()
    {
        Debug.Log("스킬 5번 시작");
        while (true)
        {
            var monsters = new List<GameObject>();
            float time = 0f;
            int count = 1;

            while (time < _effectDuration)
            {
                monsters.Clear();
                Debug.Log($"리스트 초기화 {count} 회");
                count++;

                var hits = Physics2D.OverlapCircleAll(transform.position, _detectionRadius, _monsterLayer);

                foreach (var col in hits)
                {
                    monsters.Add(col.gameObject);
                    //Debug.Log($"{col.gameObject.name}");
                }

                Skill_5_Event?.Invoke(monsters);

                yield return _wait;
                time += _detectionInterval;
            }

            // 10초 지나면 코루틴 종료
            Debug.Log("스킬 지속 시간 10초 종료");
            Destroy(gameObject);
            yield break;
        }
    }

    private void OnEnable()
    {
        StartCoroutine(DetectCoroutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}

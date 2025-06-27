using System.Collections.Generic;
using UnityEngine;

public class SkillLogic_2 : MonoBehaviour
{
    [SerializeField] private ActiveSkillData _data;
    [SerializeField] private PlayerControllerTypeA_Copy _playerController;

    [Header("투사체 프리팹")]
    [SerializeField] private GameObject projectilePrefab;

    [Header("투사체 수")]
    [SerializeField] private int objSize = 3;

    [Header("원 궤도 설정")]
    [SerializeField] private float circleR = 3f;
    [SerializeField] private float objSpeed = 140f;

    private float degree;
    private GameObject[] targets;

    [SerializeField] private int _skillLevel = 0;
    [SerializeField] private List<GameObject> _hitMonsters = new List<GameObject>();


    private void Start()
    {
        targets = new GameObject[objSize];
        for (int i = 0; i < objSize; i++)
        {
            targets[i] = Instantiate(projectilePrefab, transform);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UseSkill();
        }
    }

    private void UseSkill()
    {
        degree = (degree + Time.deltaTime * objSpeed) % 360f;

        float interval = 360f / objSize;
        Vector3 center = transform.position;

        // 각 투사체 위치 갱신
        for (int i = 0; i < objSize; i++)
        {
            float angle = degree + interval * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(
                Mathf.Sin(rad) * circleR,
                Mathf.Cos(rad) * circleR,
                0f
            );

            var go = targets[i];
            go.transform.position = center + offset;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, circleR);
    }

    private void Damage()
    {
        float damage = _playerController.AttackPoint * (0.75f + 0.0075f * _skillLevel);

        foreach (var monster in _hitMonsters)
        {
            monster.GetComponent<Monster_CYH>().TakeDamage(damage);
            Debug.Log($"{monster.name}에게 {damage}의 피해를 가했음");
        }
    }
}
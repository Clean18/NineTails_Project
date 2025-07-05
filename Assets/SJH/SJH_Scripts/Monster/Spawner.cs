using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Tooltip("프리팹 순서는\nMelee > Ranged > Tanker\n고정입니다.")]
    public GameObject[] MonsterPrefabs;
	[SerializeField] private List<List<GameObject>> monsterPool = new();

    [Tooltip("생성할 몬스터들의 위치들")]
	public List<Transform> SpawnPoints;
    [Tooltip("몬스터 생성 딜레이")]
	public float SpawnDelay;

	private Coroutine SpawnRoutine;

    /// <summary>
    /// 몬스터 종류별로 생성할 마리수
    /// </summary>
    private const int _spawnCount = 20;

    [Tooltip("생성할 몬스터의 레벨")]
    [SerializeField] private int _spawnLevel;

	void Awake()
	{
        for (int i = 0; i < MonsterPrefabs.Length; i++)
        {
            monsterPool.Add(new());
		    for (int j = 0; j < _spawnCount; j++)
		    {
			    var mon = Instantiate(MonsterPrefabs[i]);
                monsterPool[i].Add(mon);
			    mon.SetActive(false);
		    }
        }
		GameManager.Instance.Spawner = this;
	}

	void Update()
	{
        if (PlayerController.Instance == null || !PlayerController.Instance.IsInit) return;

		if (SpawnRoutine == null)
			SpawnRoutine = StartCoroutine(Spawn());
	}

	IEnumerator Spawn()
	{
		while (true)
		{
			yield return new WaitForSeconds(SpawnDelay);

            var tableIndex = Random.Range(0, MonsterPrefabs.Length);
            // 몬스터 타입
            MonsterType type = (MonsterType)tableIndex;
            MonsterData data = DataManager.Instance.GetMonsterData(type, _spawnLevel);
            bool isSpawn = false;

            foreach (var mon in monsterPool[tableIndex])
			{
				if (mon.activeSelf) continue;

				int ran = Random.Range(0, SpawnPoints.Count);
				mon.transform.position = SpawnPoints[ran].position;

                // 데이터 할당
                var monScript = mon.GetComponent<BaseMonsterFSM>();
                monScript.MonsterDataInit(data);

				mon.SetActive(true);
                isSpawn = true;
				break;
			}

            if (!isSpawn)
            {
			    var newMon = Instantiate(MonsterPrefabs[tableIndex], SpawnPoints[Random.Range(0, SpawnPoints.Count)].position, Quaternion.identity);

                var monScript = newMon.GetComponent<BaseMonsterFSM>();
                monScript.MonsterDataInit(data);

                monsterPool[tableIndex].Add(newMon);
            }
		}
	}
}

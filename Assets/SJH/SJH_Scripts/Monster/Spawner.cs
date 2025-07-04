using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	public GameObject[] MonsterPrefabs;
	private List<List<GameObject>> monsterPool = new();

	public List<Transform> SpawnPoints;
	public float SpawnDelay;

	private Coroutine SpawnRoutine;

    private const int _spawnCount = 20;

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

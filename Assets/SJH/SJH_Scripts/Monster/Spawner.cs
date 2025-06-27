using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	// 오브젝트풀
	// 스폰리스트
	// 스폰할 프리팹

	public GameObject[] MonsterPrefabs;
	private List<List<GameObject>> monsterPool = new();

	public List<Transform> SpawnPoints;
	public float SpawnDelay;

	private Coroutine SpawnRoutine;

    private const int _spawnCount = 20;

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
		if (SpawnRoutine == null)
			SpawnRoutine = StartCoroutine(Spawn());
	}

	IEnumerator Spawn()
	{
		while (true)
		{
			yield return new WaitForSeconds(SpawnDelay);

            var tableIndex = Random.Range(0, MonsterPrefabs.Length);
            bool isSpawn = false;

            foreach (var mon in monsterPool[tableIndex])
			{
				if (mon.activeSelf) continue;

				int ran = Random.Range(0, SpawnPoints.Count);
				mon.transform.position = SpawnPoints[ran].position;
				mon.SetActive(true);
                isSpawn = true;
				break;
			}

            if (!isSpawn)
            {
			    var newMon = Instantiate(MonsterPrefabs[tableIndex], SpawnPoints[Random.Range(0, SpawnPoints.Count)].position, Quaternion.identity);
                monsterPool[tableIndex].Add(newMon);
            }
		}
	}
}

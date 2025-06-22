using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	// 오브젝트풀
	// 스폰리스트
	// 스폰할 프리팹

	public GameObject MonsterPrefab;
	private List<GameObject> monsterPool = new();

	public List<Transform> SpawnPoints;
	public float SpawnDelay;

	private Coroutine SpawnRoutine;

	void Awake()
	{
		for (int i = 0; i < 20; i++)
		{
			var mon = Instantiate(MonsterPrefab);
			monsterPool.Add(mon);
			mon.SetActive(false);
		}
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

			foreach (var mon in monsterPool)
			{
				if (mon.activeSelf) continue;

				int ran = Random.Range(0, SpawnPoints.Count);
				mon.transform.position = SpawnPoints[ran].position;
				mon.SetActive(true);
				break;
			}

			var newMon = Instantiate(MonsterPrefab, SpawnPoints[Random.Range(0, SpawnPoints.Count)].position, Quaternion.identity);
			monsterPool.Add(newMon);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public float TotlaDamage;

	void OnTriggerEnter2D(Collider2D collision)
	{
		string tag = collision.tag;
		switch (tag)
		{
			case "Wall":
				Destroy(gameObject);
				break;
			case "Monster":
				// 대미지
				SJH_Monster mon = collision.GetComponent<SJH_Monster>();
				if (mon != null)
				{
					mon.TakeDamage((int)TotlaDamage);
				}
				break;
		}
	}
}

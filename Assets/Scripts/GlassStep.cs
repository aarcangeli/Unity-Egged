using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using StarterAssets;
using UnityEngine;

public class GlassStep : MonoBehaviour
{
	public bool IsReal;
	public GameObject FakeGlassPrefab;
	public float ExplosionForce;

	private Collider _collider;
	private RandomSoundsScript _sound;

	void Start()
	{
		_collider = GetComponentInChildren<Collider>();
		_sound = GetComponent<RandomSoundsScript>();
		if (!IsReal)
		{
			_collider.isTrigger = true;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == GameManager.Instance.Player.gameObject || other.GetComponent<EggEntity>())
		{
			if (!IsReal)
			{
				var selfTransform = transform;
				var frac = Instantiate(FakeGlassPrefab, selfTransform.position, selfTransform.rotation,
					selfTransform.parent);

				foreach (var rb in frac.GetComponentsInChildren<Rigidbody>())
				{
					rb.AddExplosionForce(ExplosionForce, selfTransform.position, 10);
				}

				Destroy(gameObject);

				if (_sound)
				{
					_sound.PlayRandomSound();
				}
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (IsReal)
		{
			Gizmos.color = Color.green;
		}
		else
		{
			Gizmos.color = Color.red;
		}

		Gizmos.DrawSphere(transform.position, 0.5f);
	}
}

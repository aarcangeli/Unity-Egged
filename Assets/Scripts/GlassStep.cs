using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class GlassStep : MonoBehaviour
{
	public bool IsReal;
	public GameObject FakeGlassPrefab;
	public float ExplosionForce;

	private Collider _collider;
	private FirstPersonController _player;

	// Yes, really bad practice, but I'm lazy
	public static float LastFallTime;

	void Start()
	{
		_collider = GetComponentInChildren<Collider>();
		_player = FindObjectOfType<FirstPersonController>();
		LastFallTime = float.MinValue;
		if (!IsReal)
		{
			_collider.isTrigger = true;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == _player.gameObject)
		{
			if (!IsReal)
			{
				LastFallTime = Time.time;
				
				var selfTransform = transform;
				var frac = Instantiate(FakeGlassPrefab, selfTransform.position, selfTransform.rotation,
					selfTransform.parent);
				
				foreach (var rb in frac.GetComponentsInChildren<Rigidbody>())
				{
					rb.AddExplosionForce(ExplosionForce, selfTransform.position, 10);
					Destroy(rb.gameObject, 5);
				}

				Destroy(gameObject);
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

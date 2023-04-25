using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class EggEntity : MonoBehaviour
{
	public LayerMask ExplodeOnLayers;
	public GameObject BrokenEggPrefab;
	public GameObject DecalPrefab;

	private bool _isBroken;

	private EggManager _eggManager;

	private void Start()
	{
		_eggManager = FindObjectOfType<EggManager>();
	}

	private void OnCollisionEnter(Collision other)
	{
		// destroy only on certain layers
		var bodyLayerMask = 1 << other.gameObject.layer;
		if ((bodyLayerMask & ExplodeOnLayers.value) == 0) return;

		// Send a message to the other object
		var enemy = other.gameObject.GetComponentInParent<Enemy>();
		if (enemy != null)
		{
			enemy.OnEggHit();
		}

		// Draw a decal
		if (!_isBroken)
		{
			foreach (ContactPoint contact in other.contacts)
			{
				if (contact.thisCollider.gameObject == gameObject || contact.otherCollider.gameObject == gameObject)
				{
					var decal = Instantiate(DecalPrefab, contact.point, Quaternion.LookRotation(contact.normal));
					decal.transform.SetParent(other.transform);
					decal.SetActive(true);
					// Destroy(decal, 10);
				}
			}
		}

		Break();
	}

	private void Break()
	{
		if (_isBroken) return;
		_isBroken = true;

		Destroy(gameObject);

		// Spawn broken egg
		var transform1 = transform;
		var body = Instantiate(BrokenEggPrefab, transform1.position, transform1.rotation);

		var velocity = GetComponent<Rigidbody>().velocity * 0.2f;
		foreach (var rb in body.GetComponentsInChildren<Rigidbody>())
		{
			rb.velocity = velocity;
		}

		// Destroy broken egg after a while
		_eggManager.AddEgg(body);
	}
}

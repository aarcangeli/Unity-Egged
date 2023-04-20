using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EggEntity : MonoBehaviour
{
	public LayerMask ExplodeOnLayers;
	public GameObject BrokenEggPrefab;
	public float EggBreakTime = 5f;
	private bool _isBroken;

	private void OnCollisionEnter(Collision other)
	{
		// destroy only on certain layers
		var bodyLayerMask = 1 << other.gameObject.layer;
		if ((bodyLayerMask & ExplodeOnLayers.value) == 0) return;

		// Send a message to the other object
		other.gameObject.SendMessage("OnEggCollision", SendMessageOptions.DontRequireReceiver);

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
		Destroy(body, EggBreakTime);
	}
}

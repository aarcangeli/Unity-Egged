using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using StarterAssets;
using UnityEngine;

public class EggLooter : MonoBehaviour
{
	public float speed = 1.0f;
	public int eggs = 0;

	private bool _isLooted = false;

	void Update()
	{
		transform.Rotate(0, speed * Time.deltaTime, 0);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_isLooted)
		{
			return;
		}

		Destroy(gameObject);
		GameManager.Instance.Player.AddEggs(eggs);
		_isLooted = true;
	}
}

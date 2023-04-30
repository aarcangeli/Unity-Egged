using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealEnemies : MonoBehaviour
{
	private AudioSource _sound;
	
	public float Delay = 1f;
	
	[SerializeField]
	private bool IsTriggered;

	private void Start()
	{
		_sound = GetComponent<AudioSource>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (IsTriggered || !other.gameObject.CompareTag("Player")) return;

		IsTriggered = true;
		StartCoroutine(ShowEnemies());
	}

	private IEnumerator ShowEnemies()
	{
		var enemies = FindObjectsOfType<Enemy>();
		foreach (var enemy in enemies)
		{
			if (!enemy.LatentShow) continue;
			enemy.ShowEnemy();
			_sound.Play();
			yield return new WaitForSeconds(Delay);
		}
	}
}

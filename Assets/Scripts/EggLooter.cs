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
	public RandomSoundsScript Sound;
	public bool SaveSnapshotOnLoot;

	private bool _isLooted = false;

	void Update()
	{
		transform.Rotate(0, speed * Time.deltaTime, 0);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_isLooted || !other.gameObject.CompareTag("Player"))
		{
			return;
		}

		Destroy(gameObject);
		GameManager.Instance.Player.AddEggs(eggs);
		_isLooted = true;

		// Play sound
		var sound = Instantiate(Sound, transform.position, transform.rotation);
		sound.PlayRandomSound();
		Destroy(sound.gameObject, sound.Clip.clip.length + 2);

		// Save the snapshot when the player hits the checkpoint
		if (SaveSnapshotOnLoot)
		{
			SnapshotManager.Instance.TakeSnapshotAsync();
		}
	}
}

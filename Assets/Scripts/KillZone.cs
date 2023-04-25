using System;
using Managers;
using StarterAssets;
using UnityEngine;

public class KillZone : MonoBehaviour
{
	public String CustomMessage;
	public float VerticalTolerance;
	private FirstPersonController _player;

	void Start()
	{
		_player = FindObjectOfType<FirstPersonController>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == _player.gameObject && Time.time - GlassStep.LastFallTime > 0.5f)
		{
			if (_player.transform.position.y >= transform.position.y - VerticalTolerance)
			{
				GameManager.Instance.GameOver(CustomMessage);
			}
		}
	}
}

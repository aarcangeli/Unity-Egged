using System;
using Managers;
using StarterAssets;
using UnityEngine;

public class KillArea : MonoBehaviour
{
	public String CustomMessage;
	public bool KillPlayer;
	public bool DestroyObjects;

	private void OnTriggerEnter(Collider other)
	{
		var player = other.GetComponent<FirstPersonController>();
		if (player)
		{
			if (KillPlayer)
			{
				GameManager.Instance.GameOver(CustomMessage);
			}
		}
		else if (DestroyObjects)
		{
			Destroy(other.gameObject);
		}
	}
}

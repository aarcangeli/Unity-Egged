using Managers;
using Unity.VisualScripting;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		// Save the snapshot when the player hits the checkpoint
		if (other.gameObject.CompareTag("Player"))
		{
			SnapshotManager.Instance.TakeSnapshot();
			Destroy(gameObject);
		}
	}
}

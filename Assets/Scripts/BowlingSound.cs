using UnityEngine;

public class BowlingSound : MonoBehaviour
{
	public RandomSoundsScript Sound;
	public LayerMask CollisionMask;
	public float MaxVolume;
	public float Multiplier;


	private void OnCollisionEnter(Collision other)
	{
		if (Utils.IsInLayerMask(other.gameObject, CollisionMask))
		{
			var tempAudio = Instantiate(Sound);
			tempAudio.Clip.volume = Mathf.Clamp(other.relativeVelocity.magnitude * Multiplier, 0, MaxVolume);
			tempAudio.PlayRandomSound();
			Destroy(tempAudio.gameObject, tempAudio.Clip.clip.length + 2);
		}
	}
}

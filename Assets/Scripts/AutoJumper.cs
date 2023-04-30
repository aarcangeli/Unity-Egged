using StarterAssets;
using UnityEngine;

public class AutoJumper : MonoBehaviour
{
	public float JumpHeight;

    private void OnTriggerEnter(Collider other)
    {
	    var player = other.gameObject.GetComponent<FirstPersonController>();
	    if (player)
	    {
		    var minVerticalVelocity = player.CalculateVerticalVelocity(JumpHeight);
		    player.AutoJump(Mathf.Max(-player.VerticalVelocity, minVerticalVelocity));
	    }
    }
}

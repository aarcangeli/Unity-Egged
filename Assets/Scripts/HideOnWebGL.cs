using UnityEngine;

namespace StarterAssets
{
	public class HideOnWebGL : MonoBehaviour
	{
		private void Awake()
		{
			if (Application.platform == RuntimePlatform.WebGLPlayer)
			{
				gameObject.SetActive(false);
			}
		}
	}
}

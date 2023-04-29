using Managers;
using TMPro;
using UnityEngine;

namespace Binders
{
	public class LevelNumberBinder : MonoBehaviour
	{
		// Start is called before the first frame update
		void Start()
		{
			GetComponent<TMP_Text>().text = "Level " + GameManager.Instance.CurrentLevel;
		}
	}
}

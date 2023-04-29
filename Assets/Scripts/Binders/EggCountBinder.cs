using Managers;
using StarterAssets;
using TMPro;
using UnityEngine;

namespace Binders
{
	public class EggCountBinder : MonoBehaviour
	{
		private TMP_Text _text;

		private int _lastEggs = -1;

		void Start()
		{
			_text = GetComponent<TMP_Text>();
		}

		void Update()
		{
			var player = GameManager.Instance.Player;
			if (player && player.Eggs != _lastEggs)
			{
				OnEggsChanged(player.Eggs);
			}
		}

		private void OnEggsChanged(int eggs)
		{
			_text.text = "x" + eggs;
			_lastEggs = eggs;
		}
	}
}

using StarterAssets;
using TMPro;
using UnityEngine;

namespace Binders
{
	public class EggCountBinder : MonoBehaviour
	{
		private FirstPersonController _player;
		private TMP_Text _text;

		void Start()
		{
			_player = FindObjectOfType<FirstPersonController>();
			_player.OnEggsChanged += OnEggsChanged;
			_text = GetComponent<TMP_Text>();
			OnEggsChanged(_player.Eggs);
		}

		private void OnEggsChanged(int newcount)
		{
			_text.text = "x" + newcount.ToString();
		}
	}
}

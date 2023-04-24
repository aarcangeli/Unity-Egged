using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
	public class EggManager : MonoBehaviour
	{
		public int MaxBrokenEggs = 5;
	
		private List<GameObject> _eggs = new();
	
		public void AddEgg(GameObject body)
		{
			_eggs.Add(body);
			if (_eggs.Count > MaxBrokenEggs)
			{
				Destroy(_eggs[0]);
				_eggs.RemoveAt(0);
			}
		}
	}
}

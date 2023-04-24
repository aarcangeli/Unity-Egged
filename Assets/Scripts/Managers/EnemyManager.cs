using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace Managers
{
	public class EnemyManager : MonoBehaviour
	{
		// ReSharper disable once FieldCanBeMadeReadOnly.Local (We need to make it serializable)
		private List<Enemy> _enemies = new();

		public void RegisterEnemy(Enemy enemy)
		{
			_enemies.Add(enemy);
		}

		public void OnEnemyKilled(Enemy enemy)
		{
			if (_enemies.Remove(enemy) && _enemies.Count == 0)
			{
				OpenGates();
			}
		}

		private void OpenGates()
		{
			foreach (var gate in FindObjectsOfType<Gate>())
			{
				gate.OpenGate();
			}
		}
	}
}

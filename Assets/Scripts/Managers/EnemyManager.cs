using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace Managers
{
	public class EnemyManager : MonoBehaviour
	{
		// ReSharper disable once FieldCanBeMadeReadOnly.Local (We need to make it serializable)
		private List<Enemy> _enemies;

		private Gate[] _gates;

		private void Start()
		{
			_enemies = FindObjectsOfType<Enemy>().ToList();
			_gates = FindObjectsOfType<Gate>();
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
			foreach (var gate in _gates)
			{
				gate.OpenGate();
			}
		}
	}
}

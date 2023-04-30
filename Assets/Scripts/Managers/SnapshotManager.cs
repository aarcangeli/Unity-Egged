using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
	public class SnapshotManager : MonoBehaviour
	{
		public static SnapshotManager Instance
		{
			get
			{
				var gameManager = GameManager.Instance;
				return gameManager ? gameManager.SnapshotManager : null;
			}
		}

		public delegate void RestoreSnapshotDelegate();

		public RestoreSnapshotDelegate OnRestoreSnapshot;

		[SerializeField] private GameObject _prefabWrapper;

		private Snapshot _lastSnapshot;

		private void Awake()
		{
			if (!_prefabWrapper)
			{
				_prefabWrapper = new GameObject("__prefabs");
				_prefabWrapper.transform.parent = null;
			}
		}

		private void Start()
		{
			TakeSnapshot();
		}

		private void Update()
		{
#if UNITY_EDITOR
			if (Keyboard.current.f1Key.wasPressedThisFrame)
			{
				TakeSnapshot();
			}

			if (Keyboard.current.f2Key.wasPressedThisFrame)
			{
				RestoreSnapshot();
			}
#endif
		}

		public void TakeSnapshotAsync()
		{
			Invoke(nameof(TakeSnapshot), 0.01f);
		}

		public void TakeSnapshot()
		{
			print("Taking snapshot");

			var enemies = FindObjectsOfType<RestorableObject>();
			var snapshot = new Snapshot();
			foreach (var enemy in enemies)
			{
				var enemyTransform = enemy.transform;
				var item = Instantiate(enemy.gameObject, enemyTransform.position, enemyTransform.rotation);
				item.SetActive(false);
				item.transform.parent = _prefabWrapper.transform;
				snapshot.Enemies.Add(item);
			}

			// destroy old snapshot
			if (_lastSnapshot != null)
			{
				foreach (var it in _lastSnapshot.Enemies)
				{
					Destroy(it);
				}
			}

			_lastSnapshot = snapshot;
		}

		public void RestoreSnapshotAsync()
		{
			Invoke(nameof(RestoreSnapshot), 0.01f);
		}

		public void RestoreSnapshot()
		{
			if (_lastSnapshot == null)
			{
				return;
			}

			foreach (var enemy in FindObjectsOfType<RestorableObject>())
			{
				var it = enemy.gameObject;
				Destroy(it);

				// set inactive to avoid FindObjectsOfType picking it up
				it.SetActive(false);
			}

			foreach (var it in _lastSnapshot.Enemies)
			{
				var newEnemy = Instantiate(it);
				newEnemy.transform.position = it.transform.position;
				newEnemy.transform.rotation = it.transform.rotation;
				newEnemy.SetActive(true);
			}

			OnRestoreSnapshot?.Invoke();
		}
	}

	public class Snapshot
	{
		public List<GameObject> Enemies = new();
	}
}

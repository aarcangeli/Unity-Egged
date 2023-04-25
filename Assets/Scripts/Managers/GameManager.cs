using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Managers
{
	[RequireComponent(typeof(EnemyManager))]
	public class GameManager : MonoBehaviour
	{
		public List<Scene> Levels;

		[Header("Pause Menu")] public GameObject PauseMenu;

		[Header("Stop Menu")] public GameObject StopMenu;
		public TMP_Text TimeText;

		private float _startTime;

		private EnemyManager _enemyManager;

		private static GameManager m_Instance;

		public static GameManager Instance
		{
			get
			{
				if (!m_Instance)
				{
					m_Instance = FindObjectOfType<GameManager>();
				}

				return m_Instance;
			}
		}

		public bool IsPaused { get; private set; }

		public bool IsStopped { get; private set; }

		// Start is called before the first frame update
		void Awake()
		{
			StartGame();
			_enemyManager = GetComponent<EnemyManager>();
		}

		private void OnDisable()
		{
			m_Instance = null;
		}

		private void Update()
		{
			if (Keyboard.current.escapeKey.wasPressedThisFrame)
			{
				SetPaused(!IsPaused);
			}
		}

		// Update is called once per frame
		private void StartGame()
		{
			_startTime = Time.time;
			Cursor.visible = false;
			IsPaused = false;
			IsStopped = false;
			StopMenu.SetActive(false);
			PauseMenu.SetActive(false);
			Cursor.lockState = CursorLockMode.Locked;
		}

		public void EndGame()
		{
			var gameTime = Time.time - _startTime;
			TimeText.text = TimeSpan.FromSeconds(gameTime).ToString("mm':'ss'.'ff");
			Time.timeScale = 0;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			StopMenu.SetActive(true);
			IsPaused = true;
			IsStopped = true;
		}

		public void ResumeGame() => SetPaused(false);

		public void LoadMenu()
		{
			SceneManager.LoadScene("Menu");
			Time.timeScale = 1;
		}

		public void QuitGame()
		{
			Application.Quit();
		}

		public void RegisterEnemy(Enemy enemy)
		{
			_enemyManager.RegisterEnemy(enemy);
		}

		public void OnEnemyKilled(Enemy enemy)
		{
			_enemyManager.OnEnemyKilled(enemy);
		}

		private void SetPaused(bool Paused)
		{
			if (IsStopped)
			{
				return;
			}

			IsPaused = Paused;
			if (IsPaused)
			{
				Time.timeScale = 0;
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
				PauseMenu.SetActive(true);
			}
			else
			{
				Time.timeScale = 1;
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
				PauseMenu.SetActive(false);
			}
		}

		public void GameOver(string customMessage)
		{
			// TODO
			EndGame();
		}
	}
}

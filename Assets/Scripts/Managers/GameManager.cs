using System;
using System.Linq;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

namespace Managers
{
	[RequireComponent(typeof(EnemyManager))]
	[RequireComponent(typeof(SnapshotManager))]
	[RequireComponent(typeof(ScoreManager))]
	public class GameManager : MonoBehaviour
	{
		public TMP_Text PersistentTimeText;

		public bool IsMenu;

		[Header("Pause Menu")] public GameObject PauseMenu;

		[Header("Stop Menu")] public GameObject StopMenu;
		public TMP_Text TimeText;
		public GameObject NextLevelButton;
		public GameObject NewHighScorePanel;
		public AudioSource WinSound;

		[Header("Splash Menu")] public GameObject SplashMenu;
		public TMP_Text HighScoreText;
		public GameObject HighScorePanel;

		public int FirstLevelIndex;

		private float _startTime;

		public EnemyManager EnemyManager { get; private set; }
		public SnapshotManager SnapshotManager { get; private set; }
		public ScoreManager ScoreManager { get; private set; }

		private static GameManager _instance;

		private FirstPersonController _player;

		private bool _isPaused;
		private bool _isWaitingAnyKey;

		public static GameManager Instance
		{
			get
			{
				if (!_instance)
				{
					_instance = FindObjectOfType<GameManager>();
				}

				return _instance;
			}
		}

		public int CurrentLevel => SceneManager.GetActiveScene().buildIndex - FirstLevelIndex + 1;
		public int NumLevels => SceneManager.sceneCountInBuildSettings - FirstLevelIndex;

		public bool IsPaused => _isPaused || _isWaitingAnyKey || IsMenu;

		public bool IsStopped { get; private set; }

		public FirstPersonController Player => _player;

		// Start is called before the first frame update
		void Awake()
		{
			_instance = this;
			EnemyManager = GetComponent<EnemyManager>();
			SnapshotManager = GetComponent<SnapshotManager>();
			ScoreManager = GetComponent<ScoreManager>();
			if (IsMenu)
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
			else
			{
				StartGame();
			}

			SnapshotManager.OnRestoreSnapshot += SetupPlayer;

			SetupPlayer();

			if (CurrentLevel == NumLevels)
			{
				NextLevelButton.SetActive(false);
			}
		}

		private void SetupPlayer()
		{
			_player = FindObjectOfType<FirstPersonController>();
		}

		private void OnDisable()
		{
			_instance = null;
		}

		private void Update()
		{
			if (Keyboard.current.escapeKey.wasPressedThisFrame)
			{
				SetPaused(!IsPaused);
			}

			var gameTime = Time.time - _startTime;
			PersistentTimeText.text = TimeToString(gameTime);

			if (_isWaitingAnyKey)
			{
				CheckSplashScreen();
			}
		}

		// Update is called once per frame
		private void StartGame()
		{
			_startTime = Time.time;
			Cursor.visible = false;
			_isPaused = false;
			IsStopped = false;
			StopMenu.SetActive(false);
			PauseMenu.SetActive(false);
			Cursor.lockState = CursorLockMode.Locked;
			_isWaitingAnyKey = true;
			Time.timeScale = 0;

			SplashMenu.SetActive(true);
			var highScore = ScoreManager.GetHighScore(CurrentLevel);
			HighScoreText.text = TimeToString(highScore);
			HighScorePanel.SetActive(highScore > 0);
		}

		public void EndGame()
		{
			var gameTime = Time.time - _startTime;
			TimeText.text = TimeToString(gameTime);
			Time.timeScale = 0;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			StopMenu.SetActive(true);
			_isPaused = true;
			IsStopped = true;

			// save high score
			NewHighScorePanel.SetActive(false);
			if (ScoreManager.IsBetterScore(gameTime, CurrentLevel))
			{
				NewHighScorePanel.SetActive(true);
				ScoreManager.StoreHighScore(CurrentLevel, gameTime);
			}

			if (WinSound)
			{
				WinSound.Play();
			}
		}

		private static string TimeToString(float gameTime)
		{
			return TimeSpan.FromSeconds(gameTime).ToString("mm':'ss'.'ff");
		}

		public void ResumeGame() => SetPaused(false);

		public void RestartGame()
		{
			Time.timeScale = 1;
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

		public void LoadNextLevel()
		{
			Time.timeScale = 1;
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		}

		public void LoadMenu()
		{
			SceneManager.LoadScene("Menu");
			Time.timeScale = 1;
		}

		public void QuitGame()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
		}

		private void SetPaused(bool Paused)
		{
			if (IsStopped || _isWaitingAnyKey)
			{
				return;
			}

			_isPaused = Paused;
			if (_isPaused)
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

		private void CheckSplashScreen()
		{
			var wasPressed = Keyboard.current?.anyKey.wasPressedThisFrame == true ||
			                 Gamepad.current?.allControls.Any(x =>
				                 x is ButtonControl { isPressed: true } && !x.synthetic) == true;
			if (!wasPressed) return;
			_isWaitingAnyKey = false;
			SplashMenu.SetActive(false);
			Time.timeScale = 1;
			Cursor.lockState = CursorLockMode.Locked;
		}

		public void GameOver(string customMessage)
		{
			// todo
			SnapshotManager.RestoreSnapshotAsync();
		}
	}
}

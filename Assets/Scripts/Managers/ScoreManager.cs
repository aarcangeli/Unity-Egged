using UnityEngine;

namespace Managers
{
	public class ScoreManager : MonoBehaviour
	{
		public static ScoreManager Instance => GameManager.Instance.ScoreManager;

		public void StoreHighScore(int level, float score)
		{
			PlayerPrefs.SetFloat($"Time{level}", score);
			PlayerPrefs.Save();
		}

		public float GetHighScore(int level)
		{
			return PlayerPrefs.GetFloat($"Time{level}", 0);
		}

		public bool IsBetterScore(float time, int currentLevel)
		{
			var currentHighScore = GetHighScore(currentLevel);
			return currentHighScore == 0 || time < currentHighScore;
		}
	}
}

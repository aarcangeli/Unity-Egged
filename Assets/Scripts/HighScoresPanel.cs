using System.Collections;
using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;

public class HighScoresPanel : MonoBehaviour
{
	public GameObject Panel;
	public GameObject HighScorePrefab;

	void Start()
	{
		// Remove the prefab from the hierarchy
		HighScorePrefab.transform.SetParent(null);

		// And create a new one for each level
		var numLevels = GameManager.Instance.NumLevels;
		float total = 0;
		for (var i = 0; i < numLevels; i++)
		{
			var levelNumber = i + 1;
			var highScore = ScoreManager.Instance.GetHighScore(levelNumber);
			AddRow(levelNumber + ". " + GameManager.TimeToString(highScore));
			total += highScore;
		}
		
		AddRow("Total. " + GameManager.TimeToString(total));

		if (total == 0)
		{
			gameObject.SetActive(false);
		}
	}

	private void AddRow(string timeToString)
	{
		var obj = Instantiate(HighScorePrefab, Panel.transform);
		obj.GetComponent<TMP_Text>().SetText(timeToString);
	}
}

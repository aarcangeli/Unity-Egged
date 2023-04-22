using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Goal : MonoBehaviour
{
	public GameObject RotatableFlag;

	private GameObject _player;

	// Start is called before the first frame update
	void Start()
	{
		_player = GameObject.FindWithTag("Player");
	}

	// Update is called once per frame
	void Update()
	{
		// make rotation
		if (!RotatableFlag) return;
		
		var delta = _player.transform.position - RotatableFlag.transform.position;
		delta.y = 0;
		delta.Normalize();
		var rotation = Quaternion.LookRotation(delta);

		var rotationEulerAngles = RotatableFlag.transform.rotation.eulerAngles;
		RotatableFlag.transform.rotation = rotation;
	}
}

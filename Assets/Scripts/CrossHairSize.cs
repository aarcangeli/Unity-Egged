using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

[RequireComponent(typeof(Image))]
public class CrossHairSize : MonoBehaviour
{
	private Image _crossHair;

	public float SizeMultiplier = 1.0f;

	private Camera _camera;

	private void Start()
	{
		SnapshotManager.Instance.OnRestoreSnapshot += OnSetup;

		_crossHair = GetComponent<Image>();
		OnSetup();
	}

	private void OnSetup()
	{
		_camera = Camera.main;
	}

	// Update is called once per frame
	void Update()
	{
		if (!_crossHair || !_camera) return;

		var crossHairSprite = _crossHair.sprite;
		if (!crossHairSprite) return;

		// get aspect
		var aspect = crossHairSprite.rect.width / crossHairSprite.rect.height;

		// get screen size
		var screenSize = new Vector2(Screen.width, Screen.height);

		// mid height of the near plane in meters (at 1 meter distance from the camera)
		var height = Mathf.Tan(_camera.fieldOfView * 0.5f * Mathf.Deg2Rad);

		var imageHeight = height * SizeMultiplier * screenSize.y;
		var width = imageHeight * aspect;
		_crossHair.rectTransform.sizeDelta = new Vector2(width, imageHeight);
	}
}

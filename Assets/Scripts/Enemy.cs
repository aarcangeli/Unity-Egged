using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
	public bool RotateToPlayer = true;

	private Animator _animator;
	private GameObject _player;
	private bool _isDead;

	void Start()
	{
		_animator = GetComponent<Animator>();
		_player = GameObject.FindWithTag("Player");
		GameManager.Instance.RegisterEnemy(this);
	}

	void Update()
	{
		if (RotateToPlayer && !_isDead)
		{
			transform.LookAt(_player.transform);
		}
	}

	public void KillEnemy()
	{
		if (!_isDead)
		{
			_isDead = true;
			_animator.SetTrigger("isDead");
			GameManager.Instance.OnEnemyKilled(this);
		}
	}

	void OnFinishDeath()
	{
		Destroy(gameObject);
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.GetComponent<Enemy>())
		{
			KillEnemy();
		}
	}
}

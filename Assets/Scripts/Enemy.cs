using System;
using Managers;
using StarterAssets;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
	public bool RotateToPlayer = true;
	public bool KillableByEgg;
	public bool KillableByFall;
	public bool LatentShow;

	public RandomSoundsScript DieSound;

	[Tooltip("If the angle of the pin is greater than this, the enemy will die")]
	public float FallAngle;

	private Animator _animator;

	[SerializeField] private bool IsEnemyDead;

	private static readonly int IsDead = Animator.StringToHash("isDead");

	void Start()
	{
		_animator = GetComponent<Animator>();
		_animator.SetBool("LatentShow", LatentShow);

		// After deserialization
		if (IsEnemyDead)
		{
			GameManager.Instance.EnemyManager.OnEnemyKilled(this);
			Destroy(gameObject);
		}
	}

	protected void Update()
	{
		if (RotateToPlayer && !IsEnemyDead)
		{
			var lookPos = GameManager.Instance.Player.transform.position - transform.position;
			lookPos.y = 0;
			transform.rotation = Quaternion.LookRotation(lookPos);
		}

		if (KillableByFall && FallAngle > 0)
		{
			var upHeight = transform.up.y;
			if (upHeight < Math.Cos(FallAngle * Mathf.Deg2Rad) || transform.position.y < -10)
			{
				KillEnemy();
			}
		}
	}

	public void OnEggHit()
	{
		RotateToPlayer = false;
		if (KillableByEgg)
		{
			KillEnemy();
		}
	}

	private void KillEnemy()
	{
		if (IsEnemyDead) return;

		IsEnemyDead = true;
		_animator.SetTrigger(IsDead);
		GameManager.Instance.EnemyManager.OnEnemyKilled(this);

		if (DieSound)
		{
			DieSound.PlayRandomSound();
		}
	}

	void OnFinishDeath()
	{
		Destroy(gameObject);
	}

	public void ShowEnemy()
	{
		LatentShow = false;
		_animator.SetBool("LatentShow", LatentShow);
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.GetComponent<Enemy>())
		{
			// Disable rotation to player if we collide with another enemy
			RotateToPlayer = false;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		var player = other.GetComponent<FirstPersonController>();
		if (player && !IsEnemyDead)
		{
			GameManager.Instance.GameOver("");
		}
	}
}

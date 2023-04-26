using System;
using Managers;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
	public bool RotateToPlayer = true;
	public bool KillableByEgg = false;
	public bool KillableByFall = false;

	[Tooltip("If the angle of the pin is greater than this, the enemy will die")]
	public float FallAngle;

	private Animator _animator;
	private bool _isDead;

	private static readonly int IsDead = Animator.StringToHash("isDead");

	void Start()
	{
		_animator = GetComponent<Animator>();
	}

	protected void Update()
	{
		if (RotateToPlayer && !_isDead)
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
		if (!_isDead)
		{
			_isDead = true;
			_animator.SetTrigger(IsDead);
			GameManager.Instance.EnemyManager.OnEnemyKilled(this);
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
			// Disable rotation to player if we collide with another enemy
			RotateToPlayer = false;
		}
	}
}

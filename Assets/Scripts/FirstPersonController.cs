using System;
using System.Threading;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
	[RequireComponent(typeof(PlayerInput))]
	public class FirstPersonController : MonoBehaviour
	{
		[Header("Player")] [Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 4.0f;

		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 6.0f;

		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.0f;

		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;

		[Space(10)] [Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;

		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.1f;

		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		[Tooltip("The player is automatically killed if the vertical velocity is over this value")]
		public float KillVelocity;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;

		[Tooltip("Useful for rough ground")] public float GroundedOffset = -0.14f;

		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;

		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;

		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;

		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;

		public Animator HandAnimator;

		[Header("Egg")] public GameObject EggPrototype;
		public float EggVelocity = 10f;
		public float EggAngularVelocity = 10f;
		public float EggSpawnDelay = 0.2f;
		public float EggSpawnerTimeout = 0.2f;

		[Tooltip("Extra pitch when firing in degrees")]
		public float ExtraFirePitch = 0;

		// cinemachine
		[SerializeField] private float _cinemachineTargetPitch;

		// player
		private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

		private PlayerInput _playerInput;
		private CharacterController _controller;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;
		private bool _isFiring;

		private static readonly int PThrow = Animator.StringToHash("p_throw");
		private static readonly int IsThereEggs = Animator.StringToHash("IsThereEggs");

		[SerializeField] private int TotalEggs;

		private const float _threshold = 0.01f;

		public delegate void EggChangeEvent(int newCount);

		public EggChangeEvent OnEggsChanged;

		private bool IsCurrentDeviceMouse
		{
			get { return _playerInput.currentControlScheme == "KeyboardMouse"; }
		}

		private void Awake()
		{
			// get a reference to our main camera
			_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();
			_playerInput = GetComponent<PlayerInput>();

			// Ensure the egg is not active
			EggPrototype.SetActive(false);

			UpdateHandAnimation();
		}

		private void Start()
		{
			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;
		}

		private void OnEnable()
		{
			SnapshotManager.Instance.OnRestoreSnapshot += RestoreSnapshot;
		}

		private void OnDisable()
		{
			SnapshotManager.Instance.OnRestoreSnapshot -= RestoreSnapshot;
		}

		private void RestoreSnapshot()
		{
			_input.Reset();
		}

		private void Update()
		{
			JumpAndGravity();
			GroundedCheck();
			Move();
			CheckFire();
		}

		private void CheckFire()
		{
			if (_input.isFired && !_isFiring && !GameManager.Instance.IsPaused && TotalEggs > 0)
			{
				_isFiring = true;
				TotalEggs--;
				HandAnimator.SetTrigger(PThrow);
				Invoke(nameof(SpawnEgg), EggSpawnDelay);
				Invoke(nameof(EnableEggSpawner), EggSpawnerTimeout);
				UpdateHandAnimation();
				OnEggsChanged?.Invoke(TotalEggs);
			}

			_input.isFired = false;
		}

		private void SpawnEgg()
		{
			var projectile = Instantiate(EggPrototype);
			projectile.transform.position = EggPrototype.transform.position;
			projectile.transform.rotation = EggPrototype.transform.rotation;
			projectile.GetComponent<Rigidbody>().velocity = GetThrowDirection() * EggVelocity;
			projectile.GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * EggAngularVelocity;
			projectile.SetActive(true);
		}

		private void EnableEggSpawner()
		{
			_isFiring = false;
		}

		private void LateUpdate()
		{
			CameraRotation();
		}

		private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
				transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
				QueryTriggerInteraction.Ignore);
		}

		private void CameraRotation()
		{
			// if there is an input
			if (_input.look.sqrMagnitude >= _threshold && !GameManager.Instance.IsPaused)
			{
				//Don't multiply mouse input by Time.deltaTime
				float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

				_cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
				_rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

				// clamp our pitch rotation
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

				// Update Cinemachine camera target pitch
				CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

				// rotate the player left and right
				transform.Rotate(Vector3.up * _rotationVelocity);
			}
		}

		private void Move()
		{
			// set target speed based on move speed, sprint speed and if sprint is pressed
			float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

			// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

			// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is no input, set the target speed to 0
			if (_input.move == Vector2.zero) targetSpeed = 0.0f;

			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset ||
			    currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
					Time.deltaTime * SpeedChangeRate);

				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}

			// normalise input direction
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
			{
				// move
				inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
			}

			// move the player
			_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) +
			                 new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
		}

		private void JumpAndGravity()
		{
			if (Grounded)
			{
				// reset the fall timeout timer
				_fallTimeoutDelta = FallTimeout;

				// stop our velocity dropping infinitely when grounded
				if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f;
				}

				// Jump
				if (_input.jump && _jumpTimeoutDelta <= 0.0f)
				{
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
				}

				// jump timeout
				if (_jumpTimeoutDelta >= 0.0f)
				{
					_jumpTimeoutDelta -= Time.deltaTime;
				}
			}
			else
			{
				// reset the jump timeout timer
				_jumpTimeoutDelta = JumpTimeout;

				// fall timeout
				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Time.deltaTime;
				}
			}

			// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
			if (_verticalVelocity < _terminalVelocity)
			{
				_verticalVelocity += Gravity * Time.deltaTime;
			}

			if (KillVelocity > 0 && _verticalVelocity < -KillVelocity)
			{
				GameManager.Instance.GameOver("");
			}
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private Vector3 GetThrowDirection()
		{
			var rotation = EggPrototype.transform.rotation.eulerAngles;

			// Change the pitch
			// normalize between -180 and 180
			rotation.x -= ExtraFirePitch;

			return Quaternion.Euler(rotation) * Vector3.forward;
		}

		private void OnDrawGizmos()
		{
			DrawArrow.ForDebug(EggPrototype.transform.position, GetThrowDirection(), Color.red);
		}

		private void OnDrawGizmosSelected()
		{
			var transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			var transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(
				new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
				GroundedRadius);
		}

		public void AddEggs(int eggs)
		{
			TotalEggs += eggs;
			UpdateHandAnimation();
			OnEggsChanged?.Invoke(TotalEggs);
		}

		private void UpdateHandAnimation()
		{
			HandAnimator.SetBool(IsThereEggs, TotalEggs > 0);
		}

		public int Eggs => TotalEggs;
	}
}

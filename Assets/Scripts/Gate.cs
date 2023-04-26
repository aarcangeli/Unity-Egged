using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Gate : MonoBehaviour
{
	private static readonly int IsOpen = Animator.StringToHash("IsOpen");

	private Animator _animator;

	void Start()
	{
		_animator = GetComponent<Animator>();
	}

	public void OpenGate()
	{
		_animator.SetBool(IsOpen, true);
	}
}

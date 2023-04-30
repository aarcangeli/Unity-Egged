using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Gate : MonoBehaviour
{
	private static readonly int IsOpen = Animator.StringToHash("IsOpen");

	private Animator _animator;
	private AudioSource _audioSource;

	void Start()
	{
		_animator = GetComponent<Animator>();
		_audioSource = GetComponent<AudioSource>();
	}

	public void OpenGate()
	{
		_animator.SetBool(IsOpen, true);
		if (_audioSource)
		{
			_audioSource.Play();
		}
	}
}

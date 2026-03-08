namespace Common.Runtime.Audio
{
	using UnityEngine;
	using UnityEngine.UI;
	using Zenject;

	[DisallowMultipleComponent]
	public sealed class ButtonClickSound : MonoBehaviour
	{
		[SerializeField] private Button _button;
		[SerializeField] private AudioClip _clip;

		[Range(0f, 1f)]
		[SerializeField] private float _volume = 1f;

		[Inject] private AudioSource _audioSource;

		private void Reset()
		{
			_button = GetComponent<Button>();
		}

		private void Awake()
		{
			if (_button == null)
			{
				_button = GetComponent<Button>();
			}

			if (_audioSource == null)
			{
				_audioSource = GetComponent<AudioSource>();
			}

			if (_button == null)
			{
				Debug.LogError($"{nameof(ButtonClickSound)}: Button is not assigned and not found on GameObject", this);
				enabled = false;
				return;
			}

			if (_audioSource == null)
			{
				Debug.LogError($"{nameof(ButtonClickSound)}: AudioSource is not assigned and not found on GameObject", this);
				enabled = false;
				return;
			}

			_button.onClick.AddListener(Play);
		}

		private void OnDestroy()
		{
			if (_button != null)
			{
				_button.onClick.RemoveListener(Play);
			}
		}

		private void Play()
		{
			if (_clip == null)
			{
				return;
			}

			_audioSource.PlayOneShot(_clip, _volume);
		}
	}
}
namespace Common.Runtime.UI
{
	using DG.Tweening;
	using UnityEngine;

	public sealed class SpinnerRotation : MonoBehaviour
	{
		[SerializeField] private RectTransform _target;

		[Min(0.01f)]
		[SerializeField] private float _secondsPerRevolution = 1f;

		private Tween _tween;

		private void Reset()
		{
			_target = transform as RectTransform;
		}

		private void OnEnable()
		{
			if (_target == null)
			{
				_target = transform as RectTransform;
			}

			StartTween();
		}

		private void OnDisable()
		{
			StopTween(resetRotation: false);
		}

		private void OnDestroy()
		{
			StopTween(resetRotation: false);
		}

		private void StartTween()
		{
			StopTween(resetRotation: false);

			if (_target == null)
			{
				return;
			}
			if (_secondsPerRevolution <= 0f)
			{
				_secondsPerRevolution = 1f;
			}

			_tween = _target
				.DORotate(new Vector3(0f, 0f, -360f), _secondsPerRevolution, RotateMode.FastBeyond360)
				.SetEase(Ease.Linear)
				.SetLoops(-1, LoopType.Restart)
				.SetUpdate(true);
		}

		private void StopTween(bool resetRotation)
		{
			if (_tween != null)
			{
				_tween.Kill();
				_tween = null;
			}

			if (resetRotation && _target != null)
			{
				_target.localRotation = Quaternion.identity;
			}
		}
	}
}
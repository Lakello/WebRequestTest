namespace Common.Runtime.Clicker
{
	using Features.Clicker.Runtime.Views;
	using UnityEngine;
	using Zenject;

	public sealed class ClickerTapPointRegistrar : MonoBehaviour
	{
		[Inject] private ClickerTapPointProvider _provider;
		[Inject] private ClickerTabView _view;

		private RectTransform _rect;

		private void Awake()
		{
			_rect = _view.ClickButtonRect;
		}

		private void OnEnable()
		{
			_provider.SetClickButtonRect(_rect);
		}

		private void OnDisable()
		{
			_provider.ClearClickButtonRect(_rect);
		}
	}
}
namespace Common.Runtime.Clicker
{
	using Features.Clicker.Runtime.Views;
	using UnityEngine;
	using Zenject;

	public sealed class ClickerTapPointRegistrar : MonoBehaviour
	{
		[Inject] private IClickerTapPointRegistry _registry;
		[Inject] private ClickerTabView _view;

		private RectTransform _rect;

		private void Awake()
		{
			_rect = _view.ClickButtonRect;
		}

		private void OnEnable()
		{
			_registry.SetClickButtonRect(_rect);
		}

		private void OnDisable()
		{
			_registry.ClearClickButtonRect(_rect);
		}
	}
}
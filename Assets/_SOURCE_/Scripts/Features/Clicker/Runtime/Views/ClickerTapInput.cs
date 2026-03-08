namespace Features.Clicker.Runtime.Views
{
	using R3;
	using UnityEngine;
	using UnityEngine.EventSystems;

	public sealed class ClickerTapInput : MonoBehaviour, IPointerDownHandler
	{
		private readonly Subject<Vector3> _tapWorldPositions = new();
		public Observable<Vector3> TapWorldPositions => _tapWorldPositions;

		public void OnPointerDown(PointerEventData eventData)
		{
			// world point на плоскости того RectTransform, на котором висит компонент
			var rt = (RectTransform)transform;

			if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
				rt, eventData.position, eventData.pressEventCamera, out var world))
			{
				_tapWorldPositions.OnNext(world);
			}
		}

		private void OnDestroy()
		{
			_tapWorldPositions.Dispose();
		}
	}
}
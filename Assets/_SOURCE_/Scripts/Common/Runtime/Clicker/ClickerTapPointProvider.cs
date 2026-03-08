namespace Common.Runtime.Clicker
{
	using UnityEngine;

	public sealed class ClickerTapPointProvider : IClickerTapPointProvider, IClickerTapPointRegistry
	{
		private RectTransform _clickButtonRect;

		public void SetClickButtonRect(RectTransform rect) => _clickButtonRect = rect;

		public void ClearClickButtonRect(RectTransform rect)
		{
			if (_clickButtonRect == rect)
			{
				_clickButtonRect = null;
			}
		}

		public bool TryGetRandomPointInClickButton(out Vector3 worldPos)
		{
			if (_clickButtonRect == null)
			{
				worldPos = default;
				return false;
			}

			var rect = _clickButtonRect.rect;
			var local = new Vector2(
				Random.Range(rect.xMin, rect.xMax),
				Random.Range(rect.yMin, rect.yMax)
			);

			worldPos = _clickButtonRect.TransformPoint(local);
			return true;
		}
	}
}
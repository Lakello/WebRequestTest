namespace Common.Runtime.Clicker
{
	using UnityEngine;

	public interface IClickerTapPointRegistry
	{
		void SetClickButtonRect(RectTransform rect);

		void ClearClickButtonRect(RectTransform rect);
	}
}
namespace Common.Runtime.Clicker
{
	using UnityEngine;

	public interface IClickerTapPointProvider
	{
		bool TryGetRandomPointInClickButton(out Vector3 worldPos);
	}
}
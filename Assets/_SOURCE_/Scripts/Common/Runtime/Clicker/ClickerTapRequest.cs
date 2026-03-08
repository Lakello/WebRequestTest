namespace Common.Runtime.Clicker
{
	using UnityEngine;

	public readonly struct ClickerTapRequest
	{
		public readonly ClickerTapSource Source;
		public readonly Vector3 WorldPos;

		public ClickerTapRequest(ClickerTapSource source, Vector3 worldPos)
		{
			Source = source;
			WorldPos = worldPos;
		}
	}
}
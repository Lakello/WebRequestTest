namespace Common.Runtime.Clicker
{
	using UnityEngine;

	public readonly struct ClickerTapPerformed
	{
		public readonly ClickerTapSource Source;
		public readonly Vector3 WorldPos;

		public ClickerTapPerformed(ClickerTapSource source, Vector3 worldPos)
		{
			Source = source;
			WorldPos = worldPos;
		}
	}
}
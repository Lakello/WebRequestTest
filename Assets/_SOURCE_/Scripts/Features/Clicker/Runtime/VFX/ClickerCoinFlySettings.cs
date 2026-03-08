namespace Features.Clicker.Runtime.VFX
{
	using UnityEngine;

	[System.Serializable]
	public sealed class ClickerCoinFlySettings
	{
		[Header("Duration (seconds)")]
		public float DurationMin = 0.45f;
		public float DurationMax = 0.75f;

		[Header("Curve amplitude (pixels)")]
		public float CurveAmplitudeMin = 80f;
		public float CurveAmplitudeMax = 160f;

		public AnimationCurve Curve = AnimationCurve.EaseInOut(0, 0, 1, 0);
	}
}
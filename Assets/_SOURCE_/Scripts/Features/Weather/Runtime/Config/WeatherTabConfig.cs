namespace Features.Weather.Runtime.Config
{
	using UnityEngine;

	[CreateAssetMenu(
		fileName = "WeatherTabConfig",
		menuName = "Game/Configs/Weather Tab Config",
		order = 11)]
	public sealed class WeatherTabConfig : ScriptableObject
	{
		[Header("Refresh")]
		[Min(0.1f)]
		public float RefreshIntervalSeconds = 5f;
	}
}
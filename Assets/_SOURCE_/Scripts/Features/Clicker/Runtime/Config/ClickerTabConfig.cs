namespace Features.Clicker.Runtime.Config
{
	using System;
	using UnityEngine;

	[CreateAssetMenu(
		fileName = "ClickerTabConfig",
		menuName = "Game/Configs/Clicker Tab Config",
		order = 10)]
	public sealed class ClickerTabConfig : ScriptableObject
	{
		[Header("Currency reward")]
		[Min(0)] public long ManualTapCurrency = 1;
		[Min(0)] public long AutoTapCurrency = 1;

		[Header("Energy cost")]
		[Min(1)] public int TapEnergyCost = 1;

		[Header("Energy settings")]
		[Min(1)] public int EnergyMax = 1000;

		[Tooltip("Если 0 или меньше — стартовое значение будет равно EnergyMax")]
		public int EnergyStart = 0;

		[Min(0f)] public float EnergyRegenIntervalSeconds = 10f;
		[Min(0)] public int EnergyRegenAmount = 10;

		[Header("Auto tap")]
		[Min(0f)] public float AutoTapIntervalSeconds = 3f;

		public int GetEnergyStartOrMax()
		{
			if (EnergyStart <= 0) return EnergyMax;
			return Mathf.Clamp(EnergyStart, 0, EnergyMax);
		}

		private void OnValidate()
		{
			EnergyMax = Mathf.Max(1, EnergyMax);
			TapEnergyCost = Mathf.Max(1, TapEnergyCost);

			EnergyRegenIntervalSeconds = Mathf.Max(0f, EnergyRegenIntervalSeconds);
			EnergyRegenAmount = Mathf.Max(0, EnergyRegenAmount);

			AutoTapIntervalSeconds = Mathf.Max(0f, AutoTapIntervalSeconds);

			ManualTapCurrency = Math.Max(0, ManualTapCurrency);
			AutoTapCurrency = Math.Max(0, AutoTapCurrency);
		}
	}
}
namespace Common.Runtime.Energy
{
	using System;
	using System.Threading;
	using Cysharp.Threading.Tasks;
	using Features.Clicker.Runtime.Config;
	using Zenject;

	public sealed class EnergyRegenerator : IInitializable, IDisposable
	{
		private readonly IEnergy _energy;
		private readonly ClickerTabConfig _config;

		private CancellationTokenSource _cts;

		public EnergyRegenerator(IEnergy energy, ClickerTabConfig config)
		{
			_energy = energy;
			_config = config;
		}

		public void Initialize()
		{
			_cts = new CancellationTokenSource();
			RunAsync(_cts.Token).Forget();
		}

		private async UniTaskVoid RunAsync(CancellationToken ct)
		{
			while (!ct.IsCancellationRequested)
			{
				var seconds = _config.EnergyRegenIntervalSeconds;

				if (seconds <= 0f || _config.EnergyRegenAmount <= 0)
				{
					await UniTask.Delay(TimeSpan.FromSeconds(0.25f), cancellationToken: ct);
					continue;
				}

				await UniTask.Delay(TimeSpan.FromSeconds(seconds), cancellationToken: ct);
				if (ct.IsCancellationRequested) break;

				_energy.Add(_config.EnergyRegenAmount);
			}
		}

		public void Dispose()
		{
			_cts?.Cancel();
			_cts?.Dispose();
			_cts = null;
		}
	}
}
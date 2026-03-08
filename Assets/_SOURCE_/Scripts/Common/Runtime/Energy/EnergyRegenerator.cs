namespace _SOURCE_.Scripts.Common.Runtime.Energy
{
	using System;
	using System.Threading;
	using Cysharp.Threading.Tasks;
	using Zenject;

	public sealed class EnergyRegenerator : IInitializable, IDisposable
	{
		private readonly IEnergy _energy;

		private readonly TimeSpan _interval = TimeSpan.FromSeconds(10);
		private const int RegenAmount = 10;

		private CancellationTokenSource _cts;

		public EnergyRegenerator(IEnergy energy)
		{
			_energy = energy;
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
				await UniTask.Delay(_interval, cancellationToken: ct);
				if (ct.IsCancellationRequested) break;

				_energy.Add(RegenAmount);
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
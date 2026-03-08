namespace _SOURCE_.Scripts.Common.Runtime.Currency
{
	using System;
	using System.Threading;
	using Clicker;
	using Cysharp.Threading.Tasks;
	using Zenject;

	public sealed class AutoTapGenerator : IInitializable, IDisposable
	{
		private readonly IClickerTapBus _bus;

		private CancellationTokenSource _cts;
		private readonly TimeSpan _interval = TimeSpan.FromSeconds(3);

		public AutoTapGenerator(IClickerTapBus bus)
		{
			_bus = bus;
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

				_bus.RequestTap(new ClickerTapRequest(ClickerTapSource.Auto));
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
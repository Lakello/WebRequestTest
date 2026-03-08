namespace Common.Runtime.Clicker
{
	using System;
	using System.Threading;
	using Cysharp.Threading.Tasks;
	using Features.Clicker.Runtime.Config;
	using Zenject;

	public sealed class AutoTapGenerator : IInitializable, IDisposable
	{
		private readonly IClickerTapBus _bus;
		private readonly IClickerTapPointProvider _pointProvider;
		private readonly ClickerTabConfig _config;

		private CancellationTokenSource _cts;

		public AutoTapGenerator(
			IClickerTapBus bus,
			IClickerTapPointProvider pointProvider,
			ClickerTabConfig config)
		{
			_bus = bus;
			_pointProvider = pointProvider;
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
				var seconds = _config.AutoTapIntervalSeconds;

				// если 0 — фактически выключаем автоклик, чтобы не спамить в 0-delay loop
				if (seconds <= 0f)
				{
					await UniTask.Delay(TimeSpan.FromSeconds(0.25f), cancellationToken: ct);
					continue;
				}

				await UniTask.Delay(TimeSpan.FromSeconds(seconds), cancellationToken: ct);
				if (ct.IsCancellationRequested) break;

				var hasPoint = _pointProvider.TryGetRandomPointInClickButton(out var worldPos);
				_bus.RequestTap(new ClickerTapRequest(ClickerTapSource.Auto, hasPoint ? worldPos : default));
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
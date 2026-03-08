namespace Common.Runtime.Clicker
{
	using System;
	using System.Threading;
	using Cysharp.Threading.Tasks;
	using Zenject;

	public sealed class AutoTapGenerator : IInitializable, IDisposable
	{
		private readonly IClickerTapBus _bus;
		private readonly IClickerTapPointProvider _pointProvider;

		private CancellationTokenSource _cts;
		private readonly TimeSpan _interval = TimeSpan.FromSeconds(3);

		public AutoTapGenerator(IClickerTapBus bus, IClickerTapPointProvider pointProvider)
		{
			_bus = bus;
			_pointProvider = pointProvider;
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

				var hasPoint = _pointProvider.TryGetRandomPointInClickButton(out var worldPos);
				_bus.RequestTap(new ClickerTapRequest(ClickerTapSource.Auto, hasPoint
					? worldPos
					: default));
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
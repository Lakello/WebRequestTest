namespace _SOURCE_.Scripts.Common.Runtime.Currency
{
	using System;
	using System.Threading;
	using Cysharp.Threading.Tasks;
	using Zenject;

	public sealed class AutoCurrencyCollector : IInitializable, IDisposable
	{
		private readonly IWallet _wallet;
		private CancellationTokenSource _cts;

		// позже можно вынести в конфиг/настройку
		private readonly TimeSpan _interval = TimeSpan.FromSeconds(3);

		public AutoCurrencyCollector(IWallet wallet)
		{
			_wallet = wallet;
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

				_wallet.Add(1);
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
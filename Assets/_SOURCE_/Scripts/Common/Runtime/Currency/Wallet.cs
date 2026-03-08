namespace Common.Runtime.Currency
{
	using System;
	using R3;

	public sealed class Wallet : IWallet, IDisposable
	{
		private readonly ReactiveProperty<long> _balance = new(0);

		public ReadOnlyReactiveProperty<long> Balance => _balance;

		public void Add(long amount)
		{
			if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
			_balance.Value += amount;
		}

		public void Dispose()
		{
			_balance.Dispose();
		}
	}
}
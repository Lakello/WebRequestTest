namespace Common.Runtime.Clicker
{
	using System;
	using Currency;
	using Energy;
	using Features.Clicker.Runtime.Config;
	using R3;
	using Zenject;

	public sealed class ClickerTapProcessor : IInitializable, IDisposable, IClickerTapFeedback
	{
		private readonly IClickerTapBus _bus;
		private readonly IWallet _wallet;
		private readonly IEnergy _energy;
		private readonly ClickerTabConfig _config;

		private readonly Subject<ClickerTapPerformed> _performed = new();
		private readonly CompositeDisposable _d = new();

		public Observable<ClickerTapPerformed> TapPerformed => _performed;

		public ClickerTapProcessor(
			IClickerTapBus bus,
			IWallet wallet,
			IEnergy energy,
			ClickerTabConfig config)
		{
			_bus = bus;
			_wallet = wallet;
			_energy = energy;
			_config = config;
		}

		public void Initialize()
		{
			_bus.TapRequested
				.Subscribe(req =>
				{
					if (_energy.TrySpend(_config.TapEnergyCost))
					{
						var reward = req.Source == ClickerTapSource.Manual
							? _config.ManualTapCurrency
							: _config.AutoTapCurrency;

						if (reward > 0)
						{
							_wallet.Add(reward);
						}

						_performed.OnNext(new ClickerTapPerformed(req.Source, req.WorldPos));
					}
				})
				.AddTo(_d);
		}

		public void Dispose()
		{
			_d.Dispose();
			_performed.Dispose();
		}
	}
}
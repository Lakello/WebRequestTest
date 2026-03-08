namespace _SOURCE_.Scripts.Common.Runtime.Clicker
{
	using System;
	using Currency;
	using Energy;
	using R3;
	using Zenject;

	public sealed class ClickerTapProcessor : IInitializable, IDisposable, IClickerTapFeedback
	{
		private readonly IClickerTapBus _bus;
		private readonly IWallet _wallet;
		private readonly IEnergy _energy;

		private readonly Subject<ClickerTapPerformed> _performed = new();
		public Observable<ClickerTapPerformed> TapPerformed => _performed;

		private readonly CompositeDisposable _d = new();

		public ClickerTapProcessor(IClickerTapBus bus, IWallet wallet, IEnergy energy)
		{
			_bus = bus;
			_wallet = wallet;
			_energy = energy;
		}

		public void Initialize()
		{
			_bus.TapRequested
				.Subscribe(req =>
				{
					// единая стоимость клика
					if (_energy.TrySpend(1))
					{
						_wallet.Add(1);
						_performed.OnNext(new ClickerTapPerformed(req.Source));
					}
					// иначе: энергии нет -> ничего не делаем (позже можно emit "failed tap")
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
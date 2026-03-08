namespace Features.Clicker.Runtime.Presentation
{
	using System;
	using Common.Runtime.Clicker;
	using Common.Runtime.Currency;
	using Common.Runtime.Energy;
	using R3;
	using Views;

	public sealed class ClickerTabPresenter : IDisposable
	{
		private readonly ClickerTabView _view;
		private readonly IWallet _wallet;
		private readonly IEnergy _energy;
		private readonly IClickerTapBus _tapBus;
		private readonly IClickerTapFeedback _feedback;

		private readonly CompositeDisposable _d = new();

		public ClickerTabPresenter(
			ClickerTabView view,
			IWallet wallet,
			IEnergy energy,
			IClickerTapBus tapBus,
			IClickerTapFeedback feedback)
		{
			_view = view;
			_wallet = wallet;
			_energy = energy;
			_tapBus = tapBus;
			_feedback = feedback;

			// init UI
			_view.SetBalance(_wallet.Balance.CurrentValue);
			_view.SetEnergy(_energy.Current.CurrentValue, _energy.Max);

			// manual tap request
			_view.TapWorldPositions
				.Subscribe(worldPos =>
					_tapBus.RequestTap(new ClickerTapRequest(ClickerTapSource.Manual, worldPos)))
				.AddTo(_d);

			// balance -> UI
			_wallet.Balance
				.Subscribe(v => _view.SetBalance(v))
				.AddTo(_d);

			// energy -> UI
			_energy.Current
				.Subscribe(v => _view.SetEnergy(v, _energy.Max))
				.AddTo(_d);

			// feedback hook (на будущее для эффектов/анимаций)
			_feedback.TapPerformed
				.Subscribe(_ =>
				{
					// тут позже добавим эффекты
				})
				.AddTo(_d);
		}

		public void Dispose() => _d.Dispose();
	}
}
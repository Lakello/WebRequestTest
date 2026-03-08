namespace _SOURCE_.Scripts.Features.Clicker.Runtime.Presentation
{
	using System;
	using Common.Runtime.Currency;
	using R3;
	using Views;

	public sealed class ClickerTabPresenter : IDisposable
	{
		private readonly ClickerTabView _view;
		private readonly IWallet _wallet;

		private readonly CompositeDisposable _d = new();

		public ClickerTabPresenter(ClickerTabView view, IWallet wallet)
		{
			_view = view;
			_wallet = wallet;

			// Инициализация UI текущим значением
			_view.SetBalance(_wallet.Balance.CurrentValue);

			// Клик -> +1 валюта
			_view.Clicks
				.Subscribe(_ => _wallet.Add(1))
				.AddTo(_d);

			// Баланс -> обновить UI
			_wallet.Balance
				.Subscribe(v => _view.SetBalance(v))
				.AddTo(_d);
		}

		public void Dispose()
		{
			_d.Dispose();
		}
	}
}
namespace Features.Weather.Runtime.Presentation
{
	using System;
	using System.Threading;
	using Common.Runtime.Navigation;
	using Common.Runtime.Networking;
	using Cysharp.Threading.Tasks;
	using Features.Weather.Runtime.Config;
	using Features.Weather.Runtime.Networking;
	using Features.Weather.Runtime.Views;
	using R3;

	public sealed class WeatherTabPresenter : IDisposable
	{
		private readonly WeatherTabView _view;
		private readonly WeatherTabConfig _config;
		private readonly WeatherApiClient _api;
		private readonly IRequestQueue _queue;
		private readonly ITabActivityRegistry _tabs;

		private readonly CompositeDisposable _d = new();

		private CancellationTokenSource _loopCts;
		private bool _hasLoadedOnce;

		public WeatherTabPresenter(
			WeatherTabView view,
			WeatherTabConfig config,
			WeatherApiClient api,
			IRequestQueue queue,
			ITabActivityRegistry tabs)
		{
			_view = view;
			_config = config;
			_api = api;
			_queue = queue;
			_tabs = tabs;

			_hasLoadedOnce = false;
			_view.ShowLoading();

			_tabs.IsActive(TabId.Weather)
				.Subscribe(isActive =>
				{
					if (isActive) StartLoop();
					else StopLoop();
				})
				.AddTo(_d);
		}

		private void StartLoop()
		{
			StopLoop();

			_loopCts = new CancellationTokenSource();
			LoopAsync(_loopCts.Token).Forget();
		}

		private void StopLoop()
		{
			_loopCts?.Cancel();
			_loopCts?.Dispose();
			_loopCts = null;

			// Требование: отменить текущий и удалить ожидающие запросы этого владельца
			_queue.CancelOwner(this);

			// При уходе — просто выключаем индикаторы обновления.
			// WeatherRoot/LoadingRoot не трогаем (можно оставить последнее состояние).
			_view.SetUpdating(false);
		}

		private async UniTaskVoid LoopAsync(CancellationToken ct)
		{
			while (!ct.IsCancellationRequested)
			{
				try
				{
					if (_hasLoadedOnce)
					{
						// обновление: показываем спиннер, данные остаются видимыми
						_view.ShowWeather();
						_view.SetUpdating(true);
					}
					else
					{
						// первая загрузка: скрываем данные и показываем LoadingRoot
						_view.ShowLoading();
					}

					var dto = await _queue.Enqueue(token => _api.FetchTodayAsync(token), ownerTag: this);

					_hasLoadedOnce = true;

					_view.SetUpdating(false);
					_view.ShowWeather();
					_view.SetWeather(dto.Icon, $"{dto.Title} - {dto.Temperature}{dto.TemperatureUnit}");
				}
				catch (OperationCanceledException)
				{
					// При отмене просто убираем updating, loading можно не трогать
					_view.SetUpdating(false);
				}
				catch (Exception)
				{
					_view.SetUpdating(false);

					// Если данных ещё не было — можно показывать LoadingRoot или ошибку.
					// Я показываю ошибку в WeatherRoot, чтобы было явно.
					_view.SetError("Ошибка загрузки погоды");
				}

				var seconds = Math.Max(0.1f, _config.RefreshIntervalSeconds);
				await UniTask.Delay(TimeSpan.FromSeconds(seconds), cancellationToken: ct);
			}
		}

		public void Dispose()
		{
			StopLoop();
			_d.Dispose();
		}
	}
}
namespace Features.Tabs.Runtime.Presentation
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using Common.Runtime.Navigation;
	using Cysharp.Threading.Tasks;
	using R3;
	using Views;

	public sealed class TabsPresenter : IDisposable, ITabsNavigation, ITabActivityRegistry, ITabTransitions
	{
		private readonly TabsView _view;
		private readonly Dictionary<TabId, ITabState> _states;

		private readonly Subject<TabTransitionEvent> _tabChanging = new();
		private readonly Subject<TabTransitionEvent> _tabChanged = new();

		private readonly Dictionary<TabId, ReactiveProperty<bool>> _isActive = new();
		private readonly CompositeDisposable _disposables = new();

		private ITabState _current;

		private CancellationTokenSource _transitionCts;
		private bool _isTransitioning;

		public TabsPresenter(TabsView view, IReadOnlyList<ITabState> states)
		{
			_view = view;

			_states = new Dictionary<TabId, ITabState>();
			foreach (var s in states)
			{
				_states[s.Id] = s;
				_isActive[s.Id] = new ReactiveProperty<bool>(false);
			}

			_view.TabRequested
				.Subscribe(id => SwitchToAsync(id).Forget())
				.AddTo(_disposables);
		}

		public TabId Current => _current?.Id ?? default;
		public TabId? TransitionTarget { get; private set; }

		public Observable<TabTransitionEvent> TabChanging => _tabChanging;
		public Observable<TabTransitionEvent> TabChanged => _tabChanged;

		public ReadOnlyReactiveProperty<bool> IsActive(TabId tabId)
		{
			if (!_isActive.TryGetValue(tabId, out var rp))
			{
				throw new InvalidOperationException($"Unknown tabId: {tabId}");
			}
			return rp;
		}

		public async UniTask InitializeAsync(TabId startTab, CancellationToken ct)
		{
			_current = null;
			await SwitchToAsync(startTab);
		}

		public UniTask SwitchToAsync(TabId tabId)
		{
			if (_current != null && _current.Id == tabId)
			{
				return UniTask.CompletedTask;
			}

			if (_isTransitioning && TransitionTarget.HasValue && TransitionTarget.Value == tabId)
			{
				return UniTask.CompletedTask;
			}

			if (_isTransitioning)
			{
				_transitionCts?.Cancel();
			}

			return RunTransitionAsync(tabId);
		}

		private async UniTask RunTransitionAsync(TabId target)
		{
			if (!_states.TryGetValue(target, out var next))
			{
				throw new InvalidOperationException($"No state registered for tab {target}");
			}

			_isTransitioning = true;
			TransitionTarget = target;

			_transitionCts?.Dispose();
			_transitionCts = new CancellationTokenSource();
			var ct = _transitionCts.Token;

			var fromId = _current?.Id;
			_tabChanging.OnNext(new TabTransitionEvent(fromId, target));

			try
			{
				if (_current != null)
				{
					_isActive[_current.Id].Value = false;
					await _current.ExitAsync(ct);
				}

				_current = next;
				await _current.EnterAsync(ct);
				_isActive[_current.Id].Value = true;

				_tabChanged.OnNext(new TabTransitionEvent(fromId, target));
			}
			catch (OperationCanceledException) { }
			finally
			{
				_isTransitioning = false;
				TransitionTarget = null;
			}
		}

		public void Dispose()
		{
			_transitionCts?.Cancel();
			_transitionCts?.Dispose();

			foreach (var kv in _isActive)
			{
				kv.Value.Dispose();
			}

			_tabChanging.Dispose();
			_tabChanged.Dispose();

			_disposables.Dispose();
		}
	}
}
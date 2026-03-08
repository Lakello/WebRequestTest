namespace Features.Facts.Runtime.Presentation
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using Common.Runtime.Navigation;
	using Common.Runtime.Networking;
	using Cysharp.Threading.Tasks;
	using Networking;
	using Pooling;
	using R3;
	using Views;

	public sealed class FactsTabPresenter : IDisposable
	{
		private const int BreedsLimit = 10;

		private readonly FactsTabView _view;
		private readonly ITabActivityRegistry _tabs;
		private readonly IRequestQueue _queue;
		private readonly DogApiClient _api;
		private readonly DogBreedItemPool _itemPool;

		private readonly CompositeDisposable _d = new();

		private readonly object _breedsOwnerTag;

		private readonly List<IDisposable> _itemSubscriptions = new();
		private readonly List<DogBreedItemView> _spawnedItems = new();

		private CancellationTokenSource _activeCts;
		private DogBreedItemView _loadingItem;

		private object _detailsOwnerTag;

		public FactsTabPresenter(
			FactsTabView view,
			ITabActivityRegistry tabs,
			IRequestQueue queue,
			DogApiClient api,
			DogBreedItemPool itemPool)
		{
			_view = view;
			_tabs = tabs;
			_queue = queue;
			_api = api;
			_itemPool = itemPool;

			_breedsOwnerTag = this;
			_detailsOwnerTag = new object();

			_tabs.IsActive(TabId.Facts)
				.Subscribe(isActive =>
				{
					if (isActive)
					{
						OnEnter();
					}
					else
					{
						OnExit();
					}
				})
				.AddTo(_d);
		}

		private void OnEnter()
		{
			OnExit();

			_activeCts = new CancellationTokenSource();
			LoadBreedsAsync(_activeCts.Token).Forget();
		}

		private void OnExit()
		{
			_activeCts?.Cancel();
			_activeCts?.Dispose();
			_activeCts = null;

			_queue.CancelOwner(_breedsOwnerTag);
			_queue.CancelOwner(_detailsOwnerTag);

			SetLoadingItem(null);

			_view.HideAllIndicators();
			_view.Popup?.Hide();
		}

		private async UniTaskVoid LoadBreedsAsync(CancellationToken ct)
		{
			ClearList();
			SetLoadingItem(null);

			_view.ShowLoading();

			try
			{
				var breeds = await _queue.Enqueue(
					token => _api.FetchBreedsAsync(BreedsLimit, token),
					ownerTag: _breedsOwnerTag);

				BuildList(breeds);
				_view.ShowList();
			}
			catch (OperationCanceledException) { }
			catch (Exception)
			{
				ClearList();
				_view.ShowList();
				_view.Popup?.Hide();
			}
		}

		private void BuildList(IReadOnlyList<DogBreedListItemDto> breeds)
		{
			ClearList();

			for (var i = 0; i < breeds.Count; i++)
			{
				var index = i + 1;
				var dto = breeds[i];

				var item = _itemPool.Spawn();
				item.transform.SetParent(_view.ContentRoot, false);
				item.SetText($"{index} - {dto.Name}");
				item.SetLoading(false);

				_spawnedItems.Add(item);

				var sub = item.Clicked.Subscribe(_ =>
				{
					RequestBreedDetails(dto.Id, dto.Name, item);
				});

				_itemSubscriptions.Add(sub);
			}
		}

		private void RequestBreedDetails(string breedId, string breedName, DogBreedItemView itemView)
		{
			if (_loadingItem != null && ReferenceEquals(_loadingItem, itemView))
			{
				return;
			}

			_queue.CancelOwner(_detailsOwnerTag);

			SetLoadingItem(null);

			var ownerTag = new object();
			_detailsOwnerTag = ownerTag;

			SetLoadingItem(itemView);

			_view.Popup?.Hide();

			LoadBreedDetailsAsync(breedId, breedName, ownerTag, itemView).Forget();
		}

		private async UniTaskVoid LoadBreedDetailsAsync(string breedId, string breedName, object ownerTag, DogBreedItemView itemView)
		{
			try
			{
				var details = await _queue.Enqueue(
					token => _api.FetchBreedDetailsAsync(breedId, token),
					ownerTag: ownerTag);

				if (!ReferenceEquals(_detailsOwnerTag, ownerTag))
				{
					return;
				}

				if (ReferenceEquals(_loadingItem, itemView))
				{
					SetLoadingItem(null);
				}

				_view.Popup?.Show(details.Name, details.Description);
			}
			catch (OperationCanceledException)
			{
				if (ReferenceEquals(_detailsOwnerTag, ownerTag) && ReferenceEquals(_loadingItem, itemView))
				{
					SetLoadingItem(null);
				}
			}
			catch (Exception)
			{
				if (!ReferenceEquals(_detailsOwnerTag, ownerTag))
				{
					return;
				}

				if (ReferenceEquals(_loadingItem, itemView))
				{
					SetLoadingItem(null);
				}

				_view.Popup?.Show(breedName, "Ошибка загрузки описания породы");
			}
		}

		private void SetLoadingItem(DogBreedItemView next)
		{
			if (_loadingItem != null)
			{
				_loadingItem.SetLoading(false);
			}

			_loadingItem = next;

			if (_loadingItem != null)
			{
				_loadingItem.SetLoading(true);
			}
		}

		private void ClearList()
		{
			for (int i = 0; i < _itemSubscriptions.Count; i++)
			{
				_itemSubscriptions[i]?.Dispose();
			}
			_itemSubscriptions.Clear();

			for (int i = 0; i < _spawnedItems.Count; i++)
			{
				_itemPool.Despawn(_spawnedItems[i]);
			}
			_spawnedItems.Clear();
		}

		public void Dispose()
		{
			OnExit();
			ClearList();
			_d.Dispose();
		}
	}
}
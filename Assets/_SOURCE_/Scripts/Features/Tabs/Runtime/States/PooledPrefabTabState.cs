namespace Features.Tabs.Runtime.States
{
	using System.Threading;
	using Common.Runtime.Navigation;
	using Cysharp.Threading.Tasks;
	using DG.Tweening;
	using Pooling;
	using UnityEngine;
	using Views;

	public sealed class PooledPrefabTabState : ITabState
	{
		private readonly TabViewPool _pool;
		private readonly Transform _parent;
		private readonly float _duration;

		private TabPrefabRoot _instance;

		public PooledPrefabTabState(TabId id, TabViewPool pool, Transform parent, float duration = 0.2f)
		{
			Id = id;
			_pool = pool;
			_parent = parent;
			_duration = duration;
		}

		public TabId Id { get; }

		public UniTask EnterAsync(CancellationToken ct) => ShowAsync(ct);

		public UniTask ExitAsync(CancellationToken ct) => HideAsync(ct);

		private void EnsureInstance()
		{
			if (_instance != null)
			{
				return;
			}

			_instance = _pool.Spawn();
			_instance.transform.SetParent(_parent, false);
		}

		private async UniTask ShowAsync(CancellationToken ct)
		{
			EnsureInstance();

			var group = _instance.CanvasGroup;
			group.gameObject.SetActive(true);

			group.interactable = false;
			group.blocksRaycasts = false;

			group.alpha = 0f;

			await group.DOFade(1f, _duration)
				.SetUpdate(true)
				.ToUniTask(cancellationToken: ct);

			group.interactable = true;
			group.blocksRaycasts = true;
		}

		private async UniTask HideAsync(CancellationToken ct)
		{
			if (_instance == null)
			{
				return;
			}

			var group = _instance.CanvasGroup;
			group.interactable = false;
			group.blocksRaycasts = false;

			await group.DOFade(0f, _duration)
				.SetUpdate(true)
				.ToUniTask(cancellationToken: ct);

			_pool.Despawn(_instance);
			_instance = null;
		}
	}
}
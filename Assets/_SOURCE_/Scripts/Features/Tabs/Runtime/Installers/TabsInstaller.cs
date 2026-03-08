namespace _SOURCE_.Scripts
{
	using System.Collections.Generic;
	using Common.Runtime.Currency;
	using UnityEngine;
	using Zenject;

	public sealed class TabsInstaller : MonoInstaller
	{
		[SerializeField] private TabsView _tabsView;

		[Header("Tabs Container (under Canvas)")]
		[SerializeField] private Transform _tabsContainer;

		[Header("Tab Prefabs")]
		[SerializeField] private TabPrefabRoot _clickerPrefab;
		[SerializeField] private TabPrefabRoot _weatherPrefab;
		[SerializeField] private TabPrefabRoot _factsPrefab;

		public override void InstallBindings()
		{
			Container.Bind<TabsView>().FromInstance(_tabsView).AsSingle();

			Container.Bind<IWallet>().To<Wallet>().AsSingle().NonLazy();
			
			BindTab(TabId.Clicker, _clickerPrefab);
			BindTab(TabId.Weather, _weatherPrefab);
			BindTab(TabId.Facts, _factsPrefab);

			// Собираем states явно (иначе states по WithId не попадут сами в IEnumerable)
			Container.Bind<IReadOnlyList<ITabState>>().FromMethod(ctx => new ITabState[]
			{
				ctx.Container.ResolveId<ITabState>(TabId.Clicker),
				ctx.Container.ResolveId<ITabState>(TabId.Weather),
				ctx.Container.ResolveId<ITabState>(TabId.Facts),
			}).AsSingle();

			Container.BindInterfacesAndSelfTo<TabsPresenter>().AsSingle();
			Container.BindInterfacesAndSelfTo<TabsEntryPoint>().AsSingle();
		}

		private void BindTab(TabId id, TabPrefabRoot prefab)
		{
			// Pool per tab
			Container.BindMemoryPool<TabPrefabRoot, TabViewPool>()
				.WithId(id)
				.WithInitialSize(0)
				.FromComponentInNewPrefab(prefab)
				.UnderTransform(_tabsContainer);

			// State per tab
			Container.Bind<ITabState>()
				.WithId(id)
				.FromMethod(ctx =>
				{
					var pool = ctx.Container.ResolveId<TabViewPool>(id);
					return new PooledPrefabTabState(id, pool, _tabsContainer, 0.2f);
				})
				.AsCached();
		}
	}
}
namespace Features.Facts.Runtime.Installers
{
	using Networking;
	using Pooling;
	using Presentation;
	using UnityEngine;
	using Views;
	using Zenject;

	public sealed class FactsTabInstaller : MonoInstaller
	{
		[Header("List item prefab")]
		[SerializeField] private DogBreedItemView _breedItemPrefab;

		public override void InstallBindings()
		{
			Container.Bind<FactsTabView>().FromComponentInHierarchy().AsSingle();

			Container.Bind<DogApiClient>().AsSingle();

			Container.BindMemoryPool<DogBreedItemView, DogBreedItemPool>()
				.WithInitialSize(0)
				.FromComponentInNewPrefab(_breedItemPrefab)
				.UnderTransformGroup("BreedsItemsPool");

			Container.BindInterfacesAndSelfTo<FactsTabPresenter>().AsSingle().NonLazy();
		}
	}
}
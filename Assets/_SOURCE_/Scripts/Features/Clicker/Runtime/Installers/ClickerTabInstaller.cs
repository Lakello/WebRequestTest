namespace Features.Clicker.Runtime.Installers
{
	using Presentation;
	using UnityEngine;
	using VFX.Fx;
	using VFX.Pools;
	using VFX.Settings;
	using VFX.Spawners;
	using Views;
	using Zenject;

	public sealed class ClickerTabInstaller : MonoInstaller
	{
		[Header("VFX Root")]
		[SerializeField] private Transform _vfxRoot;

		[Header("Tap Particle (start)")]
		[SerializeField] private ClickerTapParticleFx _tapParticlePrefab;

		[Header("Impact Particle (end)")]
		[SerializeField] private ClickerTapParticleFx _impactParticlePrefab;

		[Header("Coin Fly")]
		[SerializeField] private CoinFlyFx _coinPrefab;
		[SerializeField] private ClickerCoinFlySettings _coinFlySettings = new();

		public override void InstallBindings()
		{
			Container.Bind<ClickerTabView>().FromComponentInHierarchy().AsSingle();

			Container.BindInterfacesAndSelfTo<ClickerTabPresenter>().AsSingle().NonLazy();

			// settings
			Container.BindInstance(_coinFlySettings).AsSingle();

			// particles pools
			Container.BindMemoryPool<ClickerTapParticleFx, TapParticlePool>()
				.WithInitialSize(0)
				.FromComponentInNewPrefab(_tapParticlePrefab)
				.UnderTransform(_vfxRoot);

			Container.BindMemoryPool<ClickerTapParticleFx, ImpactParticlePool>()
				.WithInitialSize(0)
				.FromComponentInNewPrefab(_impactParticlePrefab)
				.UnderTransform(_vfxRoot);

			// coin pool
			Container.BindMemoryPool<CoinFlyFx, CoinFlyFxPool>()
				.WithInitialSize(0)
				.FromComponentInNewPrefab(_coinPrefab)
				.UnderTransform(_vfxRoot);

			// spawners
			Container.BindInterfacesAndSelfTo<ClickerTapVfxSpawner>().AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo<ClickerCoinFlySpawner>().AsSingle().NonLazy();
		}
	}
}
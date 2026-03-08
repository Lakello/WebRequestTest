namespace _SOURCE_.Scripts.Features.Clicker.Runtime.Installers
{
	using Presentation;
	using UnityEngine;
	using VFX;
	using Views;
	using Zenject;

	public sealed class ClickerTabInstaller : MonoInstaller
	{
		[Header("VFX")]
		[SerializeField] private ClickerTapParticleFx _tapParticlePrefab;
		[SerializeField] private Transform _vfxRoot;

		public override void InstallBindings()
		{
			Container.Bind<ClickerTabView>().FromComponentInHierarchy().AsSingle();

			// Presenter
			Container.BindInterfacesAndSelfTo<ClickerTabPresenter>().AsSingle().NonLazy();

			// VFX pool (живет в рамках префаба вкладки)
			Container.BindMemoryPool<ClickerTapParticleFx, ClickerTapParticleFxPool>()
				.WithInitialSize(0)
				.FromComponentInNewPrefab(_tapParticlePrefab)
				.UnderTransform(_vfxRoot);

			// Spawner (подписывается на TapPerformed)
			Container.BindInterfacesAndSelfTo<ClickerTapVfxSpawner>().AsSingle().NonLazy();
		}
	}
}
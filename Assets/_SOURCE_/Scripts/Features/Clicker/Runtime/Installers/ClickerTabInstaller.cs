namespace _SOURCE_.Scripts.Features.Clicker.Runtime.Installers
{
	using Presentation;
	using Views;
	using Zenject;

	public sealed class ClickerTabInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			// View лежит на этом же prefab-инстансе
			Container.Bind<ClickerTabView>().FromComponentInHierarchy().AsSingle();

			// Presenter живёт вместе с View
			Container.BindInterfacesAndSelfTo<ClickerTabPresenter>().AsSingle().NonLazy();
		}
	}
}
namespace Features.Weather.Runtime.Installers
{
	using Config;
	using Networking;
	using Presentation;
	using UnityEngine;
	using Views;
	using Zenject;

	public sealed class WeatherTabInstaller : MonoInstaller
	{
		[SerializeField] private WeatherTabConfig _config;

		public override void InstallBindings()
		{
			Container.BindInstance(_config).AsSingle();

			Container.Bind<WeatherTabView>().FromComponentInHierarchy().AsSingle();

			Container.Bind<WeatherApiClient>().AsSingle();

			Container.BindInterfacesAndSelfTo<WeatherTabPresenter>().AsSingle().NonLazy();
		}
	}
}
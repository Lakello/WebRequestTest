namespace _SOURCE_.Scripts
{
	using Cysharp.Threading.Tasks;
	using Zenject;

	public sealed class TabsEntryPoint : IInitializable
	{
		private readonly TabsPresenter _tabs;

		public TabsEntryPoint(TabsPresenter tabs)
		{
			_tabs = tabs;
		}

		public void Initialize()
		{
			_tabs.InitializeAsync(TabId.Clicker, default).Forget();
		}
	}
}
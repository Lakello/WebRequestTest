namespace _SOURCE_.Scripts
{
	using R3;

	public interface ITabActivityRegistry
	{
		ReadOnlyReactiveProperty<bool> IsActive(TabId tabId);
	}
}
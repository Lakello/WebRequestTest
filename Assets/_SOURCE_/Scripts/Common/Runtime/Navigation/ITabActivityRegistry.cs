namespace Common.Runtime.Navigation
{
	using R3;

	public interface ITabActivityRegistry
	{
		ReadOnlyReactiveProperty<bool> IsActive(TabId tabId);
	}
}
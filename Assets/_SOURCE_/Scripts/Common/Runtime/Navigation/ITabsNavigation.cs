namespace _SOURCE_.Scripts
{
	using Cysharp.Threading.Tasks;

	public interface ITabsNavigation
	{
		TabId Current { get; }
		TabId? TransitionTarget { get; }   // если идёт переход
		UniTask SwitchToAsync(TabId tabId);
	}
}
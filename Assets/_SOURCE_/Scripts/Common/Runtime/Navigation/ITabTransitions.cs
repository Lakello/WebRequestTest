namespace _SOURCE_.Scripts
{
	using R3;

	public interface ITabTransitions
	{
		Observable<TabTransitionEvent> TabChanging { get; }
		Observable<TabTransitionEvent> TabChanged { get; }
	}
}
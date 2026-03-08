namespace Common.Runtime.Clicker
{
	using R3;

	public interface IClickerTapBus
	{
		Observable<ClickerTapRequest> TapRequested { get; }

		void RequestTap(ClickerTapRequest request);
	}
}
namespace _SOURCE_.Scripts.Common.Runtime.Clicker
{
	using R3;

	public interface IClickerTapFeedback
	{
		Observable<ClickerTapPerformed> TapPerformed { get; }
	}
}
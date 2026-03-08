namespace _SOURCE_.Scripts.Common.Runtime.Clicker
{
	public readonly struct ClickerTapRequest
	{
		public readonly ClickerTapSource Source;

		public ClickerTapRequest(ClickerTapSource source)
		{
			Source = source;
		}
	}
}
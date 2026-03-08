namespace _SOURCE_.Scripts.Common.Runtime.Clicker
{
	public readonly struct ClickerTapPerformed
	{
		public readonly ClickerTapSource Source;

		public ClickerTapPerformed(ClickerTapSource source)
		{
			Source = source;
		}
	}
}
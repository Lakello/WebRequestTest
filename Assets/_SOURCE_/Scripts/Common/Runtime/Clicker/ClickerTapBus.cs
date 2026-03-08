namespace _SOURCE_.Scripts.Common.Runtime.Clicker
{
	using System;
	using R3;

	public sealed class ClickerTapBus : IClickerTapBus, IDisposable
	{
		private readonly Subject<ClickerTapRequest> _tapRequested = new();

		public Observable<ClickerTapRequest> TapRequested => _tapRequested;

		public void RequestTap(ClickerTapRequest request) => _tapRequested.OnNext(request);

		public void Dispose() => _tapRequested.Dispose();
	}
}
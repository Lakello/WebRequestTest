namespace Common.Runtime.Navigation
{
	public readonly struct TabTransitionEvent
	{
		public readonly TabId? From;
		public readonly TabId To;

		public TabTransitionEvent(TabId? from, TabId to)
		{
			From = from;
			To = to;
		}
	}
}
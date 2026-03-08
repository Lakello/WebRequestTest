namespace Common.Runtime.Currency
{
	using R3;

	public interface IWallet
	{
		ReadOnlyReactiveProperty<long> Balance { get; }
		void Add(long amount);
	}
}
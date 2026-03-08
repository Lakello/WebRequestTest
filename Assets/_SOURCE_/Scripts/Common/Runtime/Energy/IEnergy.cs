namespace _SOURCE_.Scripts.Common.Runtime.Energy
{
	using R3;

	public interface IEnergy
	{
		int Max { get; }
		ReadOnlyReactiveProperty<int> Current { get; }

		/// <summary>Попытаться потратить энергию. True если успешно.</summary>
		bool TrySpend(int amount);

		/// <summary>Добавить энергию (с клэмпом до Max).</summary>
		void Add(int amount);
	}
}
namespace Features.Clicker.Runtime.VFX.Pools
{
	using Fx;
	using Zenject;

	public sealed class CoinFlyFxPool : MonoMemoryPool<CoinFlyFx>
	{
		protected override void OnSpawned(CoinFlyFx item)
		{
			item.gameObject.SetActive(true);
		}

		protected override void OnDespawned(CoinFlyFx item)
		{
			item.gameObject.SetActive(false);
		}
	}
}
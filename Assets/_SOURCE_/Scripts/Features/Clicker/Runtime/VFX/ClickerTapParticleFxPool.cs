namespace _SOURCE_.Scripts.Features.Clicker.Runtime.VFX
{
	using Zenject;

	public sealed class ClickerTapParticleFxPool : MonoMemoryPool<ClickerTapParticleFx>
	{
		protected override void OnDespawned(ClickerTapParticleFx item)
		{
			item.gameObject.SetActive(false);
		}

		protected override void OnSpawned(ClickerTapParticleFx item)
		{
			item.gameObject.SetActive(true);
		}
	}
}
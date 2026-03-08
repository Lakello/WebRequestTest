namespace Features.Clicker.Runtime.VFX
{
	using Zenject;

	public sealed class ImpactParticlePool : MonoMemoryPool<ClickerTapParticleFx>
	{
		protected override void OnSpawned(ClickerTapParticleFx item) => item.gameObject.SetActive(true);
		protected override void OnDespawned(ClickerTapParticleFx item) => item.gameObject.SetActive(false);
	}
}
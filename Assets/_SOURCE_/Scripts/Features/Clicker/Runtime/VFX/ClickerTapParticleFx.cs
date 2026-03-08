namespace Features.Clicker.Runtime.VFX
{
	using UnityEngine;

	public sealed class ClickerTapParticleFx : MonoBehaviour
	{
		[SerializeField] private ParticleSystem _ps;

		public float DurationSeconds
		{
			get
			{
				// main.duration не учитывает lifetime полностью, поэтому берём оценочно:
				var main = _ps.main;
				return main.duration + main.startLifetime.constantMax;
			}
		}

		public void Play()
		{
			_ps.Clear(true);
			_ps.Play(true);
		}
	}
}
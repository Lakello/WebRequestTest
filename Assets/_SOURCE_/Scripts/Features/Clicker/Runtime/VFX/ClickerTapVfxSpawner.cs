namespace Features.Clicker.Runtime.VFX
{
	using System;
	using Common.Runtime.Clicker;
	using Cysharp.Threading.Tasks;
	using R3;
	using UnityEngine;

	public sealed class ClickerTapVfxSpawner : IDisposable
	{
		private readonly TapParticlePool _pool;
		private readonly IClickerTapFeedback _feedback;
		private readonly CompositeDisposable _d = new();

		public ClickerTapVfxSpawner(
			TapParticlePool pool,
			IClickerTapFeedback feedback)
		{
			_pool = pool;
			_feedback = feedback;

			_feedback.TapPerformed
				.Subscribe(performed => Spawn(performed.WorldPos).Forget())
				.AddTo(_d);
		}

		private async UniTaskVoid Spawn(Vector3 worldPos)
		{
			var fx = _pool.Spawn();
			fx.transform.position = worldPos;
			fx.Play();

			var ms = Mathf.Max(50, (int)(fx.DurationSeconds * 1000f));
			await UniTask.Delay(ms);

			_pool.Despawn(fx);
		}

		public void Dispose() => _d.Dispose();
	}
}
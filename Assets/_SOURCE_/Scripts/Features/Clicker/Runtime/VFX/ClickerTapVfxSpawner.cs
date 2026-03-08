namespace _SOURCE_.Scripts.Features.Clicker.Runtime.VFX
{
	using System;
	using Common.Runtime.Clicker;
	using Cysharp.Threading.Tasks;
	using R3;
	using Views;

	public sealed class ClickerTapVfxSpawner : IDisposable
	{
		private readonly ClickerTabView _view;
		private readonly ClickerTapParticleFxPool _pool;
		private readonly IClickerTapFeedback _feedback;

		private readonly CompositeDisposable _d = new();

		public ClickerTapVfxSpawner(
			ClickerTabView view,
			ClickerTapParticleFxPool pool,
			IClickerTapFeedback feedback)
		{
			_view = view;
			_pool = pool;
			_feedback = feedback;

			// Эффект только на реально выполненный тап (энергия списалась, валюта начислилась)
			_feedback.TapPerformed
				.Subscribe(_ => SpawnAtButtonCenter().Forget())
				.AddTo(_d);
		}

		private async UniTaskVoid SpawnAtButtonCenter()
		{
			var fx = _pool.Spawn();

			// UI world-position центра кнопки
			fx.transform.position = _view.ClickButtonRect.position;

			fx.Play();

			// Через длительность вернуть в пул
			var ms = (int)(fx.DurationSeconds * 1000f);
			if (ms < 50) ms = 50;

			await UniTask.Delay(ms);
			_pool.Despawn(fx);
		}

		public void Dispose()
		{
			_d.Dispose();
		}
	}
}
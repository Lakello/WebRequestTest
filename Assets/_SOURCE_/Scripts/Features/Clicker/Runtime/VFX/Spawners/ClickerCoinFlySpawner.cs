namespace Features.Clicker.Runtime.VFX.Spawners
{
	using System;
	using Common.Runtime.Clicker;
	using Cysharp.Threading.Tasks;
	using DG.Tweening;
	using Pools;
	using R3;
	using Settings;
	using UnityEngine;
	using Views;
	using Random = UnityEngine.Random;

	public sealed class ClickerCoinFlySpawner : IDisposable
	{
		private readonly ClickerTabView _view;
		private readonly CoinFlyFxPool _coinPool;
		private readonly ImpactParticlePool _impactPool;
		private readonly IClickerTapFeedback _feedback;
		private readonly ClickerCoinFlySettings _settings;

		private readonly CompositeDisposable _d = new();

		public ClickerCoinFlySpawner(
			ClickerTabView view,
			CoinFlyFxPool coinPool,
			ImpactParticlePool impactPool,
			IClickerTapFeedback feedback,
			ClickerCoinFlySettings settings)
		{
			_view = view;
			_coinPool = coinPool;
			_impactPool = impactPool;
			_feedback = feedback;
			_settings = settings;

			_feedback.TapPerformed
				.Subscribe(performed => SpawnAndFly(performed.WorldPos))
				.AddTo(_d);
		}

		private void SpawnAndFly(Vector3 start)
		{
			if (_view.CoinsTargetRect == null)
			{
				Debug.LogError("CoinsTargetRect is not assigned in ClickerTabView");
				return;
			}

			var end = GetRectCenterWorld(_view.CoinsTargetRect);

			var duration = Random.Range(_settings.DurationMin, _settings.DurationMax);
			var amplitude = Random.Range(_settings.CurveAmplitudeMin, _settings.CurveAmplitudeMax);
			var curve = _settings.Curve;

			var coin = _coinPool.Spawn();
			coin.RectTransform.position = start;

			var dir = (end - start);
			var dir2 = new Vector2(dir.x, dir.y);

			var perp = dir2.sqrMagnitude < 0.0001f
				? Vector2.up
				: new Vector2(-dir2.y, dir2.x).normalized;

			DOTween.To(
					() => 0f,
					t =>
					{
						var basePos = Vector3.LerpUnclamped(start, end, t);
						var offset = (Vector3)(perp * (curve.Evaluate(t) * amplitude));
						coin.RectTransform.position = basePos + offset;
					},
					1f,
					duration
				)
				.SetEase(Ease.Linear)
				.SetUpdate(true)
				.OnComplete(() =>
				{
					SpawnImpact(end).Forget();
					_coinPool.Despawn(coin);
				})
				.OnKill(() =>
				{
					// если вкладку закроют во время полёта
					if (coin != null && coin.gameObject.activeSelf)
						_coinPool.Despawn(coin);
				});
		}

		private async UniTaskVoid SpawnImpact(Vector3 worldPos)
		{
			var fx = _impactPool.Spawn();
			fx.transform.position = worldPos;
			fx.Play();

			var ms = Mathf.Max(50, (int)(fx.DurationSeconds * 1000f));
			await UniTask.Delay(ms);

			_impactPool.Despawn(fx);
		}

		private static Vector3 GetRectCenterWorld(RectTransform rt) => rt.TransformPoint(rt.rect.center);

		public void Dispose() => _d.Dispose();
	}
}
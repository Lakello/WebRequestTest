namespace Common.Runtime.Networking
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using Cysharp.Threading.Tasks;
	using Zenject;

	public sealed class RequestQueue : IInitializable, IDisposable, IRequestQueue
	{
		private readonly object _gate = new();
		private readonly Queue<IRequestItem> _queue = new();

		private CancellationTokenSource _workerCts;
		private IRequestItem _current;

		public void Initialize()
		{
			_workerCts = new CancellationTokenSource();
			WorkerLoopAsync(_workerCts.Token).Forget();
		}

		public UniTask<T> Enqueue<T>(Func<CancellationToken, UniTask<T>> operation, object ownerTag)
		{
			if (operation == null)
			{
				throw new ArgumentNullException(nameof(operation));
			}
			if (ownerTag == null)
			{
				throw new ArgumentNullException(nameof(ownerTag));
			}

			var item = new RequestItem<T>(operation, ownerTag);

			lock (_gate)
			{
				_queue.Enqueue(item);
			}

			return item.Task;
		}

		public void CancelOwner(object ownerTag)
		{
			if (ownerTag == null)
			{
				return;
			}

			lock (_gate)
			{
				if (_current != null && ReferenceEquals(_current.OwnerTag, ownerTag))
				{
					_current.Cancel();
				}

				if (_queue.Count == 0)
				{
					return;
				}

				var tmp = new Queue<IRequestItem>(_queue.Count);
				while (_queue.Count > 0)
				{
					var it = _queue.Dequeue();
					if (ReferenceEquals(it.OwnerTag, ownerTag))
					{
						it.Cancel();
						continue;
					}

					tmp.Enqueue(it);
				}

				while (tmp.Count > 0)
				{
					_queue.Enqueue(tmp.Dequeue());
				}
			}
		}

		private async UniTaskVoid WorkerLoopAsync(CancellationToken ct)
		{
			while (!ct.IsCancellationRequested)
			{
				IRequestItem next = null;

				lock (_gate)
				{
					if (_queue.Count > 0)
					{
						next = _queue.Dequeue();
						_current = next;
					}
				}

				if (next == null)
				{
					await UniTask.Yield(PlayerLoopTiming.Update, ct);
					continue;
				}

				try
				{
					await next.ExecuteAsync(ct);
				}
				finally
				{
					lock (_gate)
					{
						if (ReferenceEquals(_current, next))
						{
							_current = null;
						}
					}
				}
			}
		}

		public void Dispose()
		{
			_workerCts?.Cancel();
			_workerCts?.Dispose();
			_workerCts = null;

			lock (_gate)
			{
				_current?.Cancel();
				_current = null;

				while (_queue.Count > 0)
				{
					_queue.Dequeue().Cancel();
				}
			}
		}

		private interface IRequestItem
		{
			object OwnerTag { get; }

			void Cancel();

			UniTask ExecuteAsync(CancellationToken workerCt);
		}

		private sealed class RequestItem<T> : IRequestItem
		{
			private readonly Func<CancellationToken, UniTask<T>> _operation;
			private readonly CancellationTokenSource _itemCts = new();

			private readonly UniTaskCompletionSource<T> _tcs = new();

			public object OwnerTag { get; }
			public UniTask<T> Task => _tcs.Task;

			public RequestItem(Func<CancellationToken, UniTask<T>> operation, object ownerTag)
			{
				_operation = operation;
				OwnerTag = ownerTag;
			}

			public void Cancel()
			{
				if (!_itemCts.IsCancellationRequested)
				{
					_itemCts.Cancel();
				}

				if (!_tcs.Task.Status.IsCompleted())
				{
					_tcs.TrySetCanceled(_itemCts.Token);
				}
			}

			public async UniTask ExecuteAsync(CancellationToken workerCt)
			{
				using var linked = CancellationTokenSource.CreateLinkedTokenSource(workerCt, _itemCts.Token);
				try
				{
					var result = await _operation(linked.Token);
					_tcs.TrySetResult(result);
				}
				catch (OperationCanceledException)
				{
					_tcs.TrySetCanceled(linked.Token);
				}
				catch (Exception e)
				{
					_tcs.TrySetException(e);
				}
				finally
				{
					_itemCts.Dispose();
				}
			}
		}
	}

	internal static class UniTaskStatusExtensions
	{
		public static bool IsCompleted(this UniTaskStatus status)
		{
			return status == UniTaskStatus.Succeeded
				|| status == UniTaskStatus.Canceled
				|| status == UniTaskStatus.Faulted;
		}
	}
}
namespace Common.Runtime.Networking
{
	using System;
	using Cysharp.Threading.Tasks;

	public interface IRequestQueue
	{
		/// <summary>
		/// Добавить операцию в очередь. Все операции выполняются строго последовательно.
		/// ownerTag нужен, чтобы потом удалить/отменить операции при смене вкладки.
		/// </summary>
		UniTask<T> Enqueue<T>(Func<System.Threading.CancellationToken, UniTask<T>> operation, object ownerTag);

		/// <summary>
		/// Удалить все ожидающие операции с данным ownerTag и отменить выполняемую (если она этого ownerTag).
		/// </summary>
		void CancelOwner(object ownerTag);
	}
}
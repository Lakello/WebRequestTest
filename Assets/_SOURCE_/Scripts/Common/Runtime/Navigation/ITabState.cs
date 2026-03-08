namespace Common.Runtime.Navigation
{
	using System.Threading;
	using Cysharp.Threading.Tasks;

	public interface ITabState
	{
		TabId Id { get; }

		UniTask EnterAsync(CancellationToken ct);

		UniTask ExitAsync(CancellationToken ct);
	}
}
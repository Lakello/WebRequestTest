namespace _SOURCE_.Scripts
{
	using Cysharp.Threading.Tasks;
	using System.Threading;

	public interface ITabState
	{
		TabId Id { get; }
		UniTask EnterAsync(CancellationToken ct);
		UniTask ExitAsync(CancellationToken ct);
	}
}
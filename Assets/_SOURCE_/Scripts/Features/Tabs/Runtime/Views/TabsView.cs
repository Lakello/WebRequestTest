namespace _SOURCE_.Scripts
{
	using R3;
	using UnityEngine;
	using UnityEngine.UI;

	public sealed class TabsView : MonoBehaviour
	{
		[SerializeField] private Button _clickerButton;
		[SerializeField] private Button _weatherButton;
		[SerializeField] private Button _factsButton;

		private readonly Subject<TabId> _tabRequested = new();

		public Observable<TabId> TabRequested => _tabRequested;

		private void Awake()
		{
			_clickerButton.onClick.AddListener(() => _tabRequested.OnNext(TabId.Clicker));
			_weatherButton.onClick.AddListener(() => _tabRequested.OnNext(TabId.Weather));
			_factsButton.onClick.AddListener(() => _tabRequested.OnNext(TabId.Facts));
		}

		private void OnDestroy()
		{
			_tabRequested?.Dispose();
		}
	}
}
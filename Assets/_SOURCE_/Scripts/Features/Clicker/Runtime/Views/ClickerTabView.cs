namespace _SOURCE_.Scripts.Features.Clicker.Runtime.Views
{
	using R3;
	using TMPro;
	using UnityEngine;
	using UnityEngine.UI;

	public sealed class ClickerTabView : MonoBehaviour
	{
		[SerializeField] private Button _clickButton;
		[SerializeField] private TMP_Text _balanceText;
		[SerializeField] private TMP_Text _energyText;

		private readonly Subject<Unit> _clicks = new();
		public Observable<Unit> Clicks => _clicks;

		private void Awake()
		{
			_clickButton.onClick.AddListener(() => _clicks.OnNext(Unit.Default));
		}

		public void SetBalance(long value)
		{
			_balanceText.text = value.ToString();
		}

		public void SetEnergy(int current, int max)
		{
			_energyText.text = $"{current}/{max}";
		}

		private void OnDestroy()
		{
			_clicks.Dispose();
		}
	}
}
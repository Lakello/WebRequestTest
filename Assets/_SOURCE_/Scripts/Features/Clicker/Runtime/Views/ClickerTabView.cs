namespace Features.Clicker.Runtime.Views
{
	using R3;
	using TMPro;
	using UnityEngine;
	using UnityEngine.UI;

	public sealed class ClickerTabView : MonoBehaviour
	{
		[SerializeField] private Button _clickButton;
		[SerializeField] private ClickerTapInput _tapInput;
		[SerializeField] private RectTransform _coinsTarget;

		[SerializeField] private TMP_Text _balanceText;
		[SerializeField] private TMP_Text _energyText;

		public RectTransform ClickButtonRect => (RectTransform)_clickButton.transform;
		public RectTransform CoinsTargetRect => _coinsTarget;

		public Observable<Vector3> TapWorldPositions => _tapInput.TapWorldPositions;

		public void SetBalance(long value) => _balanceText.text = value.ToString();

		public void SetEnergy(int current, int max) => _energyText.text = $"{current}/{max}";
	}
}
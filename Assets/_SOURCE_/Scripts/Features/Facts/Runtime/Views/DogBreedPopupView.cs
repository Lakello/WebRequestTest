namespace Features.Facts.Runtime.Views
{
	using TMPro;
	using UnityEngine;
	using UnityEngine.UI;

	public sealed class DogBreedPopupView : MonoBehaviour
	{
		[SerializeField] private GameObject _root;
		[SerializeField] private TMP_Text _title;
		[SerializeField] private TMP_Text _description;
		[SerializeField] private Button _closeButton;

		private void Awake()
		{
			if (_closeButton != null)
				_closeButton.onClick.AddListener(Hide);

			Hide();
		}

		public void Show(string title, string description)
		{
			if (_title != null) _title.text = title;
			if (_description != null) _description.text = description;

			if (_root != null) _root.SetActive(true);
			
			LayoutRebuilder.ForceRebuildLayoutImmediate(_root.transform as RectTransform);
		}

		public void Hide()
		{
			if (_root != null) _root.SetActive(false);
		}
	}
}
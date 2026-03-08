namespace Features.Facts.Runtime.Views
{
	using R3;
	using TMPro;
	using UnityEngine;
	using UnityEngine.UI;

	public sealed class DogBreedItemView : MonoBehaviour
	{
		private readonly Subject<Unit> _clicked = new();

		[SerializeField] private Button _button;
		[SerializeField] private TMP_Text _text;

		[Header("Per-item loading")]
		[SerializeField] private GameObject _loadingRoot;

		public Observable<Unit> Clicked => _clicked;

		public void SetText(string text)
		{
			if (_text != null)
			{
				_text.text = text;
			}
		}

		public void SetLoading(bool isLoading)
		{
			if (_loadingRoot != null)
			{
				_loadingRoot.SetActive(isLoading);
			}
		}

		private void Awake()
		{
			if (_button != null)
			{
				_button.onClick.AddListener(() => _clicked.OnNext(Unit.Default));
			}

			SetLoading(false);
		}

		private void OnDestroy()
		{
			_clicked.Dispose();
		}
	}
}
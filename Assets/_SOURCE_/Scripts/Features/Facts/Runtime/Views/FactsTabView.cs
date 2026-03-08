namespace Features.Facts.Runtime.Views
{
	using UnityEngine;

	public sealed class FactsTabView : MonoBehaviour
	{
		[Header("Roots")]
		[SerializeField] private GameObject _loadingRoot;
		[SerializeField] private GameObject _listRoot;

		[Header("List")]
		[SerializeField] private Transform _contentRoot;

		[Header("Popup")]
		[SerializeField] private DogBreedPopupView _popup;

		public Transform ContentRoot => _contentRoot;
		public DogBreedPopupView Popup => _popup;

		public void ShowLoading()
		{
			if (_loadingRoot != null)
			{
				_loadingRoot.SetActive(true);
			}
			if (_listRoot != null)
			{
				_listRoot.SetActive(false);
			}
		}

		public void ShowList()
		{
			if (_loadingRoot != null)
			{
				_loadingRoot.SetActive(false);
			}
			if (_listRoot != null)
			{
				_listRoot.SetActive(true);
			}
		}

		public void HideAllIndicators()
		{
			if (_loadingRoot != null)
			{
				_loadingRoot.SetActive(false);
			}
		}
	}
}
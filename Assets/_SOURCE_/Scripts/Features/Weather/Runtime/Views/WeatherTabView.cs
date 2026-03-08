namespace Features.Weather.Runtime.Views
{
	using TMPro;
	using UnityEngine;
	using UnityEngine.UI;

	public sealed class WeatherTabView : MonoBehaviour
	{
		[Header("Roots")]
		[SerializeField] private GameObject _weatherRoot; // иконка + текст
		[SerializeField] private GameObject _loadingRoot; // "Loading..." экран/панель
		[SerializeField] private GameObject _updateRoot;  // спиннер обновления поверх данных

		[Header("Weather UI")]
		[SerializeField] private Image _icon;
		[SerializeField] private TMP_Text _label;

		public void ShowLoading()
		{
			if (_weatherRoot != null) _weatherRoot.SetActive(false);
			if (_loadingRoot != null) _loadingRoot.SetActive(true);
			if (_updateRoot != null) _updateRoot.SetActive(false);
		}

		public void ShowWeather()
		{
			if (_weatherRoot != null) _weatherRoot.SetActive(true);
			if (_loadingRoot != null) _loadingRoot.SetActive(false);
			if (_updateRoot != null) _updateRoot.SetActive(false);
		}

		public void SetUpdating(bool isUpdating)
		{
			// при обновлении WeatherRoot должен оставаться включённым
			if (_updateRoot != null) _updateRoot.SetActive(isUpdating);
		}

		public void SetWeather(Sprite icon, string text)
		{
			if (_icon != null)
			{
				_icon.sprite = icon;
				_icon.enabled = icon != null;
			}

			if (_label != null)
				_label.text = text;
		}

		public void SetError(string message)
		{
			// Ошибку показываем в WeatherRoot (если он есть), чтобы пользователь видел состояние
			ShowWeather();
			SetUpdating(false);
			SetWeather(null, message);
		}
	}
}
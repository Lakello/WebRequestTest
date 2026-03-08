namespace Features.Clicker.Runtime.VFX.Fx
{
	using UnityEngine;
	using UnityEngine.UI;

	public sealed class CoinFlyFx : MonoBehaviour
	{
		[field: SerializeField] public RectTransform RectTransform { get; private set; }
		[field: SerializeField] public Image Image { get; private set; }

		private void Reset()
		{
			RectTransform = GetComponent<RectTransform>();
			Image = GetComponent<Image>();
		}
	}
}
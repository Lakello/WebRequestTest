namespace Features.Tabs.Runtime.Views
{
	using UnityEngine;

	public sealed class TabPrefabRoot : MonoBehaviour
	{
		[field: SerializeField] public CanvasGroup CanvasGroup { get; private set; }
	}
}
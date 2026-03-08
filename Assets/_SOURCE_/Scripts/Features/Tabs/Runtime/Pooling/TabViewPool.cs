namespace _SOURCE_.Scripts
{
	using Zenject;

	public sealed class TabViewPool : MonoMemoryPool<TabPrefabRoot>
	{
		protected override void OnCreated(TabPrefabRoot item)
		{
			item.gameObject.SetActive(false);
		}

		protected override void OnSpawned(TabPrefabRoot item)
		{
			item.gameObject.SetActive(true);
		}

		protected override void OnDespawned(TabPrefabRoot item)
		{
			item.gameObject.SetActive(false);
		}
	}
}
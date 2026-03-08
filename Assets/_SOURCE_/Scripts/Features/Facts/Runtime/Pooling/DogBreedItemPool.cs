namespace Features.Facts.Runtime.Pooling
{
	using Views;
	using Zenject;

	public sealed class DogBreedItemPool : MonoMemoryPool<DogBreedItemView>
	{
		protected override void OnCreated(DogBreedItemView item)
		{
			item.SetLoading(false);
			item.gameObject.SetActive(false);
		}

		protected override void OnSpawned(DogBreedItemView item)
		{
			item.gameObject.SetActive(true);
			item.SetLoading(false);
		}

		protected override void OnDespawned(DogBreedItemView item)
		{
			item.SetLoading(false);
			item.gameObject.SetActive(false);
		}
	}
}
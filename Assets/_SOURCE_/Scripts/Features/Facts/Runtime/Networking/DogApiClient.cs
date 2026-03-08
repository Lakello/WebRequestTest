namespace Features.Facts.Runtime.Networking
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using Cysharp.Threading.Tasks;
	using Newtonsoft.Json.Linq;
	using UnityEngine.Networking;

	public sealed class DogApiClient
	{
		private const string BaseUrl = "https://dogapi.dog/api/v2";

		public async UniTask<IReadOnlyList<DogBreedListItemDto>> FetchBreedsAsync(int limit, CancellationToken ct)
		{
			var url = $"{BaseUrl}/breeds?limit={limit}";
			var json = await GetTextAsync(url, ct);

			var root = JObject.Parse(json);
			var data = root["data"] as JArray;
			if (data == null)
			{
				throw new Exception("Dog API: data is missing");
			}

			var list = new List<DogBreedListItemDto>(data.Count);
			foreach (var token in data)
			{
				var obj = token as JObject;
				if (obj == null)
				{
					continue;
				}

				var id = obj.Value<string>("id");
				var name = obj["attributes"]?.Value<string>("name");

				if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name))
				{
					continue;
				}

				list.Add(new DogBreedListItemDto(id, name));
			}

			return list;
		}

		public async UniTask<DogBreedDetailsDto> FetchBreedDetailsAsync(string breedId, CancellationToken ct)
		{
			if (string.IsNullOrWhiteSpace(breedId))
			{
				throw new ArgumentOutOfRangeException(nameof(breedId));
			}

			var url = $"{BaseUrl}/breeds/{breedId}";
			var json = await GetTextAsync(url, ct);

			var root = JObject.Parse(json);
			var data = root["data"] as JObject;
			if (data == null)
			{
				throw new Exception("Dog API: data is missing");
			}

			var id = data.Value<string>("id");
			var attrs = data["attributes"] as JObject;

			var name = attrs?.Value<string>("name") ?? "Unknown";
			var description = attrs?.Value<string>("description") ?? "No description";

			return new DogBreedDetailsDto(id ?? breedId, name, description);
		}

		private static async UniTask<string> GetTextAsync(string url, CancellationToken ct)
		{
			using var req = UnityWebRequest.Get(url);
			req.SetRequestHeader("Accept", "application/json");

			await req.SendWebRequest().ToUniTask(cancellationToken: ct);

			if (req.result != UnityWebRequest.Result.Success)
			{
				throw new Exception($"GET {url} failed: {req.responseCode} {req.error}");
			}

			return req.downloadHandler.text;
		}
	}

	public readonly struct DogBreedListItemDto
	{
		public readonly string Id;
		public readonly string Name;

		public DogBreedListItemDto(string id, string name)
		{
			Id = id;
			Name = name;
		}
	}

	public readonly struct DogBreedDetailsDto
	{
		public readonly string Id;
		public readonly string Name;
		public readonly string Description;

		public DogBreedDetailsDto(string id, string name, string description)
		{
			Id = id;
			Name = name;
			Description = description;
		}
	}
}
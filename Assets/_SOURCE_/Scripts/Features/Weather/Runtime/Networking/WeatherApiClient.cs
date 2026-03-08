namespace Features.Weather.Runtime.Networking
{
	using System;
	using System.Threading;
	using Cysharp.Threading.Tasks;
	using Newtonsoft.Json.Linq;
	using UnityEngine;
	using UnityEngine.Networking;

	public sealed class WeatherApiClient
	{
		private const string ForecastUrl = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";

		public async UniTask<WeatherTodayDto> FetchTodayAsync(CancellationToken ct)
		{
			var json = await GetTextAsync(ForecastUrl, ct);

			// periods[0] обычно "Today"
			var root = JObject.Parse(json);
			var periods = root["properties"]?["periods"] as JArray;
			if (periods == null || periods.Count == 0)
				throw new Exception("Weather API: properties.periods is empty");

			var p0 = (JObject)periods[0];

			var temp = p0.Value<int?>("temperature");
			var unit = p0.Value<string>("temperatureUnit");
			var iconUrl = p0.Value<string>("icon");

			if (!temp.HasValue)
				throw new Exception("Weather API: temperature not found");

			// icon can be null/empty иногда
			Sprite sprite = null;
			if (!string.IsNullOrWhiteSpace(iconUrl))
				sprite = await GetIconSpriteAsync(iconUrl, ct);

			return new WeatherTodayDto(
				title: "Сегодня",
				temperature: temp.Value,
				temperatureUnit: string.IsNullOrWhiteSpace(unit) ? "F" : unit,
				icon: sprite
			);
		}

		private static async UniTask<string> GetTextAsync(string url, CancellationToken ct)
		{
			using var req = UnityWebRequest.Get(url);
			req.SetRequestHeader("Accept", "application/geo+json");

			await req.SendWebRequest().ToUniTask(cancellationToken: ct);

			if (req.result != UnityWebRequest.Result.Success)
				throw new Exception($"GET {url} failed: {req.responseCode} {req.error}");

			return req.downloadHandler.text;
		}

		private static async UniTask<Sprite> GetIconSpriteAsync(string url, CancellationToken ct)
		{
			using var req = UnityWebRequestTexture.GetTexture(url);
			await req.SendWebRequest().ToUniTask(cancellationToken: ct);

			if (req.result != UnityWebRequest.Result.Success)
				throw new Exception($"GET icon {url} failed: {req.responseCode} {req.error}");

			var tex = DownloadHandlerTexture.GetContent(req);
			if (tex == null) return null;

			var rect = new Rect(0, 0, tex.width, tex.height);
			return Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f), pixelsPerUnit: 100f);
		}
	}

	public readonly struct WeatherTodayDto
	{
		public readonly string Title;
		public readonly int Temperature;
		public readonly string TemperatureUnit;
		public readonly Sprite Icon;

		public WeatherTodayDto(string title, int temperature, string temperatureUnit, Sprite icon)
		{
			Title = title;
			Temperature = temperature;
			TemperatureUnit = temperatureUnit;
			Icon = icon;
		}
	}
}
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Weather.Data;

/// <summary>
/// Handles HTTP requests to the Open-Meteo API and JSON deserialization.
/// Stateless service — requires a MonoBehaviour host to run coroutines.
/// </summary>
namespace Weather.Service
{
    public static class WeatherService
    {
        private const string BASE_URL = "https://api.open-meteo.com/v1/forecast";

        private const string CURRENT_PARAMS =
            "temperature_2m,relative_humidity_2m,apparent_temperature,precipitation,weather_code,wind_speed_10m";

        private const string DAILY_PARAMS =
            "weather_code,temperature_2m_max,temperature_2m_min,precipitation_sum";

        /// <summary>
        /// Builds the full API URL for the given coordinates.
        /// </summary>
        private static string BuildUrl(float latitude, float longitude)
        {
            return $"{BASE_URL}" +
                   $"?latitude={latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                   $"&longitude={longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}" +
                   $"&current={CURRENT_PARAMS}" +
                   $"&daily={DAILY_PARAMS}" +
                   $"&timezone=Europe%2FParis" +
                   $"&forecast_days=7";
        }

        /// <summary>
        /// Fetches weather data from Open-Meteo. Must be started as a coroutine.
        /// </summary>
        /// <param name="latitude">Location latitude (default: Epitech Rennes)</param>
        /// <param name="longitude">Location longitude (default: Epitech Rennes)</param>
        /// <param name="onSuccess">Callback with parsed weather data</param>
        /// <param name="onError">Callback with error message</param>
        public static IEnumerator FetchWeather(
            float latitude,
            float longitude,
            Action<WeatherResponse> onSuccess,
            Action<string> onError)
        {
            string url = BuildUrl(latitude, longitude);
            Debug.Log($"[WeatherService] Fetching weather from: {url}");

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("User-Agent", "EpitechVR/1.0");
                request.timeout = 10;

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    string errorMsg = $"Network error: {request.error} (URL: {url})";
                    Debug.LogWarning($"[WeatherService] {errorMsg}");
                    onError?.Invoke(errorMsg);
                    yield break;
                }

                string json = request.downloadHandler.text;

                try
                {
                    WeatherResponse response = JsonUtility.FromJson<WeatherResponse>(json);

                    if (response == null || response.current == null)
                    {
                        string parseError = "Failed to parse API response (null result). JSON: " + (json.Length > 200 ? json.Substring(0, 200) + "..." : json);
                        Debug.LogWarning($"[WeatherService] {parseError}");
                        onError?.Invoke(parseError);
                        yield break;
                    }

                    // Fallback for DailyForecast primitive arrays which JsonUtility ignores
                    if (response.daily == null || response.daily.time == null || response.daily.time.Length == 0)
                    {
                        response.daily = new DailyForecast();
                        response.daily.time = ExtractStringArray(json, "\"time\":[");
                        response.daily.weather_code = ExtractIntArray(json, "\"weather_code\":[");
                        response.daily.temperature_2m_max = ExtractFloatArray(json, "\"temperature_2m_max\":[");
                        response.daily.temperature_2m_min = ExtractFloatArray(json, "\"temperature_2m_min\":[");
                        response.daily.precipitation_sum = ExtractFloatArray(json, "\"precipitation_sum\":[");
                    }

                    Debug.Log($"[WeatherService] Weather fetched successfully: {response.current.temperature_2m}\u00b0C");
                    onSuccess?.Invoke(response);
                }
                catch (Exception e)
                {
                    string parseError = $"JSON parse error: {e.Message}";
                    Debug.LogWarning($"[WeatherService] {parseError}");
                    onError?.Invoke(parseError);
                }
            }
        }

        private static string[] ExtractStringArray(string json, string key)
        {
            try
            {
                int idx = json.IndexOf(key);
                if (idx == -1) return new string[0];
                int start = idx + key.Length;
                int end = json.IndexOf("]", start);
                string content = json.Substring(start, end - start);
                string[] parts = content.Split(',');
                for (int i = 0; i < parts.Length; i++)
                    parts[i] = parts[i].Trim(' ', '"', '\n', '\r');
                return parts;
            }
            catch { return new string[0]; }
        }

        private static int[] ExtractIntArray(string json, string key)
        {
            try
            {
                int idx = json.IndexOf(key);
                if (idx == -1) return new int[0];
                int start = idx + key.Length;
                int end = json.IndexOf("]", start);
                string content = json.Substring(start, end - start);
                string[] parts = content.Split(',');
                int[] res = new int[parts.Length];
                for (int i = 0; i < parts.Length; i++)
                    int.TryParse(parts[i].Trim(), out res[i]);
                return res;
            }
            catch { return new int[0]; }
        }

        private static float[] ExtractFloatArray(string json, string key)
        {
            try
            {
                int idx = json.IndexOf(key);
                if (idx == -1) return new float[0];
                int start = idx + key.Length;
                int end = json.IndexOf("]", start);
                string content = json.Substring(start, end - start);
                string[] parts = content.Split(',');
                float[] res = new float[parts.Length];
                for (int i = 0; i < parts.Length; i++)
                    float.TryParse(parts[i].Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out res[i]);
                return res;
            }
            catch { return new float[0]; }
        }
    }
}

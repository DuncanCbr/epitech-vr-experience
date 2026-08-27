using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Quotes.Data;

/// <summary>
/// Handles HTTP requests to the API Ninjas Quotes v2 endpoint.
/// Stateless service -- requires a MonoBehaviour host to run coroutines.
/// </summary>
namespace Quotes.Service
{
    public static class QuoteService
    {
        private const string BASE_URL = "https://api.api-ninjas.com/v2/quotes";
        private const string CATEGORIES = "computers,technology";
        private const int TIMEOUT_SECONDS = 5;

        /// <summary>
        /// Fetches a random quote from API Ninjas. Must be started as a coroutine.
        /// </summary>
        /// <param name="apiKey">API Ninjas API key</param>
        /// <param name="onSuccess">Callback with the parsed quote</param>
        /// <param name="onError">Callback with an error message</param>
        public static IEnumerator FetchRandomQuote(
            string apiKey,
            Action<QuoteResponse> onSuccess,
            Action<string> onError)
        {
            string url = $"{BASE_URL}?categories={CATEGORIES}";
            Debug.Log($"[QuoteService] Fetching quote from: {url}");

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("X-Api-Key", apiKey);
                request.SetRequestHeader("User-Agent", "EpitechVR/1.0");
                request.timeout = TIMEOUT_SECONDS;

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    string errorMsg = $"Network error: {request.error} (URL: {url})";
                    Debug.LogWarning($"[QuoteService] {errorMsg}");
                    onError?.Invoke(errorMsg);
                    yield break;
                }

                string json = request.downloadHandler.text;

                try
                {
                    // API Ninjas returns a JSON array: [{"quote":"...","author":"..."}]
                    QuoteResponse[] quotes = JsonHelper.FromJsonArray<QuoteResponse>(json);

                    if (quotes == null || quotes.Length == 0)
                    {
                        string parseError = "Failed to parse API response (empty result). JSON: " +
                            (json.Length > 200 ? json.Substring(0, 200) + "..." : json);
                        Debug.LogWarning($"[QuoteService] {parseError}");
                        onError?.Invoke(parseError);
                        yield break;
                    }

                    Debug.Log($"[QuoteService] Quote fetched: \"{quotes[0].quote}\" - {quotes[0].author}");
                    onSuccess?.Invoke(quotes[0]);
                }
                catch (Exception e)
                {
                    string parseError = $"JSON parse error: {e.Message}";
                    Debug.LogWarning($"[QuoteService] {parseError}");
                    onError?.Invoke(parseError);
                }
            }
        }
    }
}

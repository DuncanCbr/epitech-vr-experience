using System;
using UnityEngine;

/// <summary>
/// Data models matching the API Ninjas Quotes v2 JSON response.
/// The API returns a root-level JSON array: [{"quote":"...","author":"...","categories":[...]}]
/// Since JsonUtility cannot deserialize root arrays, QuoteResponseArray
/// is used as a wrapper (see JsonHelper).
/// </summary>
namespace Quotes.Data
{
    [Serializable]
    public class QuoteResponse
    {
        public string quote;
        public string author;
        public string work;
        public string[] categories;
    }

    [Serializable]
    public class QuoteResponseArray
    {
        public QuoteResponse[] items;
    }

    /// <summary>
    /// Utility to deserialize root-level JSON arrays with JsonUtility.
    /// Wraps the raw array in an object before parsing.
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// Deserializes a JSON array string (e.g. "[{...},{...}]") into
        /// a C# array by wrapping it in {"items":[...]} first.
        /// </summary>
        public static T[] FromJsonArray<T>(string json)
        {
            // Wrap the raw array so JsonUtility can parse it
            string wrapped = "{\"items\":" + json + "}";

            // We need a concrete wrapper type, but since JsonUtility
            // requires a concrete type, we use QuoteResponseArray directly.
            QuoteResponseArray wrapper = JsonUtility.FromJson<QuoteResponseArray>(wrapped);
            if (wrapper != null && wrapper.items != null)
            {
                // Cast back to T[] if T is QuoteResponse
                return wrapper.items as T[];
            }
            return null;
        }
    }
}

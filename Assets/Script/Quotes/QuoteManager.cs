using UnityEngine;
using Quotes.Data;
using Quotes.Service;
using Quotes.UI;

/// <summary>
/// Singleton manager that orchestrates quote fetching and UI updates.
/// Reads the API key securely from a .env file via EnvReader.
/// Falls back to a local quotes bank when the API is unavailable.
/// </summary>
namespace Quotes
{
    public class QuoteManager : MonoBehaviour
    {
        public static QuoteManager Instance { get; private set; }

        [Header("Refresh Settings")]
        [Tooltip("Interval between quote changes in seconds")]
        [SerializeField] private float refreshIntervalSeconds = 15f;

        [Header("UI Reference")]
        [SerializeField] private QuotePanelUI panelUI;

        private string apiKey;
        private bool isFetching;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Load API key from .env file
            apiKey = EnvReader.Get("API_NINJAS_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                Debug.LogWarning("[QuoteManager] API_NINJAS_KEY not found in .env file. " +
                    "Using local quotes bank only.");
            }
        }

        void Start()
        {
            // Initial fetch
            FetchQuote();

            // Schedule periodic refresh
            InvokeRepeating(nameof(FetchQuote), refreshIntervalSeconds, refreshIntervalSeconds);
        }

        /// <summary>
        /// Triggers a quote fetch. Can be called manually to force refresh.
        /// </summary>
        public void FetchQuote()
        {
            if (isFetching)
            {
                Debug.Log("[QuoteManager] Fetch already in progress, skipping.");
                return;
            }

            // If no API key, go straight to the local bank
            if (string.IsNullOrEmpty(apiKey))
            {
                UseLocalFallback();
                return;
            }

            isFetching = true;
            StartCoroutine(QuoteService.FetchRandomQuote(
                apiKey,
                OnQuoteReceived,
                OnQuoteError
            ));
        }

        private void OnQuoteReceived(QuoteResponse quote)
        {
            isFetching = false;
            Debug.Log($"[QuoteManager] Quote received: \"{quote.quote}\" - {quote.author}");

            if (panelUI != null)
                panelUI.DisplayQuote(quote);
            else
                Debug.LogWarning("[QuoteManager] No QuotePanelUI reference assigned.");
        }

        private void OnQuoteError(string error)
        {
            isFetching = false;
            Debug.LogWarning($"[QuoteManager] Fetch failed: {error}. Using local fallback.");
            UseLocalFallback();
        }

        private void UseLocalFallback()
        {
            QuoteResponse localQuote = LocalQuotesBank.GetRandom();
            if (panelUI != null)
                panelUI.DisplayQuote(localQuote);
        }
    }
}

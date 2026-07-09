using UnityEngine;
using Weather.Data;
using Weather.Service;
using Weather.UI;

/// <summary>
/// Singleton manager that orchestrates weather data fetching and UI updates.
/// Attach to a persistent GameObject (uses DontDestroyOnLoad).
/// </summary>
namespace Weather
{
    public class WeatherManager : MonoBehaviour
    {
        public static WeatherManager Instance { get; private set; }

        [Header("Location (default: Epitech Rennes)")]
        [SerializeField] private float latitude = 48.1185f;
        [SerializeField] private float longitude = -1.6915f;

        [Header("Refresh Settings")]
        [Tooltip("Interval between API calls in minutes")]
        [SerializeField] private float refreshIntervalMinutes = 15f;

        [Header("UI Reference")]
        [SerializeField] private WeatherPanelUI panelUI;

        // Last successfully fetched data (kept for offline fallback)
        private WeatherResponse lastValidData;
        private string lastUpdateTime;
        private bool isFetching;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        void Start()
        {
            // Initial fetch
            FetchWeatherData();

            // Schedule periodic refresh
            float intervalSeconds = refreshIntervalMinutes * 60f;
            InvokeRepeating(nameof(FetchWeatherData), intervalSeconds, intervalSeconds);
        }

        /// <summary>
        /// Triggers a weather data fetch. Can be called manually to force refresh.
        /// </summary>
        public void FetchWeatherData()
        {
            if (isFetching)
            {
                Debug.Log("[WeatherManager] Fetch already in progress, skipping.");
                return;
            }

            isFetching = true;
            StartCoroutine(WeatherService.FetchWeather(
                latitude,
                longitude,
                OnWeatherReceived,
                OnWeatherError
            ));
        }

        private void OnWeatherReceived(WeatherResponse data)
        {
            isFetching = false;
            lastValidData = data;
            lastUpdateTime = System.DateTime.Now.ToString("HH:mm");

            Debug.Log($"[WeatherManager] Weather updated: {data.current.temperature_2m}\u00b0C, " +
                      $"Code: {data.current.weather_code} ({WeatherCodeMapper.GetDescription(data.current.weather_code)})");

            if (panelUI != null)
                panelUI.UpdateDisplay(data);
            else
                Debug.LogWarning("[WeatherManager] No WeatherPanelUI reference assigned.");
        }

        private void OnWeatherError(string error)
        {
            isFetching = false;
            Debug.LogWarning($"[WeatherManager] Fetch failed: {error}");

            // Show offline state but keep displaying last valid data
            if (panelUI != null && lastValidData != null)
                panelUI.ShowOfflineState(lastUpdateTime);
            else if (panelUI != null)
                panelUI.ShowOfflineState(null);
        }

        /// <summary>
        /// Updates the target location and triggers an immediate refresh.
        /// </summary>
        public void SetLocation(float newLatitude, float newLongitude)
        {
            latitude = newLatitude;
            longitude = newLongitude;
            FetchWeatherData();
        }
    }
}

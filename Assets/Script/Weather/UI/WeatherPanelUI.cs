using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using Weather.Data;

/// <summary>
/// Updates the World Space Canvas UI elements with weather data.
/// Attached to the weather panel Canvas GameObject.
/// </summary>
namespace Weather.UI
{
    public class WeatherPanelUI : MonoBehaviour
    {
        [Header("Current Weather")]
        [SerializeField] private Text txtTemperature;       // e.g. "24 C"
        [SerializeField] private Text txtApparentTemp;      // e.g. "Ressenti : 22 C"
        [SerializeField] private Text txtDescription;       // e.g. "Partiellement nuageux"
        [SerializeField] private Text txtHumidity;          // e.g. "65%"
        [SerializeField] private Text txtWind;              // e.g. "12 km/h"
        [SerializeField] private Text txtPrecipitation;     // e.g. "0.0 mm"
        [SerializeField] private Image imgCurrentIcon;      // Current weather sprite
        [SerializeField] private Text txtLastUpdated;       // e.g. "Mis a jour : 14:30"

        [Header("7-Day Forecast")]
        [SerializeField] private Text[] txtDayNames;        // e.g. "Lun", "Mar", ...
        [SerializeField] private Text[] txtDayMaxMin;       // e.g. "24 / 15"
        [SerializeField] private Image[] imgDayIcons;       // Daily weather sprite

        [Header("Status")]
        [SerializeField] private GameObject offlineIndicator; // Shown when data is stale

        // French day name abbreviations
        private static readonly string[] FrenchDayNames = { "Dim", "Lun", "Mar", "Mer", "Jeu", "Ven", "Sam" };

        /// <summary>
        /// Updates all UI elements with fresh weather data.
        /// Called by WeatherManager when new data is received.
        /// </summary>
        public void UpdateDisplay(WeatherResponse data)
        {
            if (data == null) return;

            UpdateCurrentWeather(data.current);
            UpdateDailyForecast(data.daily);
            UpdateTimestamp();

            if (offlineIndicator != null)
                offlineIndicator.SetActive(false);
        }

        /// <summary>
        /// Shows the offline indicator and updates the timestamp to reflect staleness.
        /// </summary>
        public void ShowOfflineState(string lastUpdateTime)
        {
            if (offlineIndicator != null)
                offlineIndicator.SetActive(true);

            if (txtLastUpdated != null && !string.IsNullOrEmpty(lastUpdateTime))
                txtLastUpdated.text = $"Hors-ligne (derniere MAJ : {lastUpdateTime})";
        }

        private void UpdateCurrentWeather(CurrentWeather current)
        {
            if (current == null) return;

            if (txtTemperature != null)
                txtTemperature.text = $"{current.temperature_2m:F0}\u00b0C";

            if (txtApparentTemp != null)
                txtApparentTemp.text = $"Ressenti : {current.apparent_temperature:F0}\u00b0C";

            if (txtDescription != null)
                txtDescription.text = WeatherCodeMapper.GetDescription(current.weather_code);

            if (txtHumidity != null)
                txtHumidity.text = $"{current.relative_humidity_2m}%";

            if (txtWind != null)
                txtWind.text = $"{current.wind_speed_10m:F0} km/h";

            if (txtPrecipitation != null)
                txtPrecipitation.text = $"{current.precipitation:F1} mm";

            if (imgCurrentIcon != null)
            {
                Sprite icon = WeatherCodeMapper.GetIcon(current.weather_code);
                if (icon != null)
                    imgCurrentIcon.sprite = icon;
            }
        }

        private void UpdateDailyForecast(DailyForecast daily)
        {
            if (daily == null || daily.time == null) return;

            int dayCount = Mathf.Min(daily.time.Length, 7);

            for (int i = 0; i < dayCount; i++)
            {
                // Parse ISO date to get day name
                if (i < txtDayNames.Length && txtDayNames[i] != null)
                {
                    string dayName = GetFrenchDayName(daily.time[i]);
                    txtDayNames[i].text = dayName;
                }

                // Temperature range
                if (i < txtDayMaxMin.Length && txtDayMaxMin[i] != null)
                {
                    txtDayMaxMin[i].text = $"{daily.temperature_2m_max[i]:F0}\u00b0 / {daily.temperature_2m_min[i]:F0}\u00b0";
                }

                // Weather icon
                if (i < imgDayIcons.Length && imgDayIcons[i] != null)
                {
                    Sprite icon = WeatherCodeMapper.GetIcon(daily.weather_code[i]);
                    if (icon != null)
                        imgDayIcons[i].sprite = icon;
                }
            }
        }

        private void UpdateTimestamp()
        {
            if (txtLastUpdated != null)
                txtLastUpdated.text = $"Mis a jour : {DateTime.Now:HH:mm}";
        }

        /// <summary>
        /// Converts an ISO date string (e.g. "2026-07-08") to a French day abbreviation.
        /// </summary>
        private static string GetFrenchDayName(string isoDate)
        {
            if (DateTime.TryParseExact(isoDate, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out DateTime date))
            {
                int dayOfWeek = (int)date.DayOfWeek;
                return FrenchDayNames[dayOfWeek];
            }
            return "---";
        }
    }
}

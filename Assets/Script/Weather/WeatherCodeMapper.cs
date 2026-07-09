using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Maps WMO weather codes to French descriptions and sprite resource names.
/// Reference: https://open-meteo.com/en/docs (WMO Weather interpretation codes)
/// </summary>
namespace Weather
{
    public enum WeatherIconType
    {
        Clear,
        PartlyCloudy,
        Cloudy,
        Fog,
        Drizzle,
        Rain,
        Snow,
        Thunderstorm
    }

    public static class WeatherCodeMapper
    {
        private struct WeatherInfo
        {
            public string Description;
            public WeatherIconType IconType;

            public WeatherInfo(string description, WeatherIconType iconType)
            {
                Description = description;
                IconType = iconType;
            }
        }

        private static readonly Dictionary<int, WeatherInfo> CodeMap = new Dictionary<int, WeatherInfo>
        {
            // Clear sky
            { 0,  new WeatherInfo("Ciel d\u00e9gag\u00e9",             WeatherIconType.Clear) },

            // Partly cloudy / Overcast
            { 1,  new WeatherInfo("Principalement d\u00e9gag\u00e9",   WeatherIconType.PartlyCloudy) },
            { 2,  new WeatherInfo("Partiellement nuageux",     WeatherIconType.PartlyCloudy) },
            { 3,  new WeatherInfo("Couvert",                   WeatherIconType.Cloudy) },

            // Fog
            { 45, new WeatherInfo("Brouillard",                WeatherIconType.Fog) },
            { 48, new WeatherInfo("Brouillard givrant",        WeatherIconType.Fog) },

            // Drizzle
            { 51, new WeatherInfo("Bruine l\u00e9g\u00e8re",            WeatherIconType.Drizzle) },
            { 53, new WeatherInfo("Bruine mod\u00e9r\u00e9e",           WeatherIconType.Drizzle) },
            { 55, new WeatherInfo("Bruine dense",              WeatherIconType.Drizzle) },
            { 56, new WeatherInfo("Bruine vergla\u00e7ante l\u00e9g\u00e8re", WeatherIconType.Drizzle) },
            { 57, new WeatherInfo("Bruine vergla\u00e7ante dense",  WeatherIconType.Drizzle) },

            // Rain
            { 61, new WeatherInfo("Pluie l\u00e9g\u00e8re",             WeatherIconType.Rain) },
            { 63, new WeatherInfo("Pluie mod\u00e9r\u00e9e",            WeatherIconType.Rain) },
            { 65, new WeatherInfo("Pluie forte",               WeatherIconType.Rain) },
            { 66, new WeatherInfo("Pluie vergla\u00e7ante l\u00e9g\u00e8re", WeatherIconType.Rain) },
            { 67, new WeatherInfo("Pluie vergla\u00e7ante forte",   WeatherIconType.Rain) },

            // Snow
            { 71, new WeatherInfo("Neige l\u00e9g\u00e8re",             WeatherIconType.Snow) },
            { 73, new WeatherInfo("Neige mod\u00e9r\u00e9e",            WeatherIconType.Snow) },
            { 75, new WeatherInfo("Neige forte",               WeatherIconType.Snow) },
            { 77, new WeatherInfo("Grains de neige",           WeatherIconType.Snow) },

            // Rain showers
            { 80, new WeatherInfo("Averses l\u00e9g\u00e8res",          WeatherIconType.Rain) },
            { 81, new WeatherInfo("Averses mod\u00e9r\u00e9es",         WeatherIconType.Rain) },
            { 82, new WeatherInfo("Averses violentes",         WeatherIconType.Rain) },

            // Snow showers
            { 85, new WeatherInfo("Averses de neige l\u00e9g\u00e8res", WeatherIconType.Snow) },
            { 86, new WeatherInfo("Averses de neige fortes",   WeatherIconType.Snow) },

            // Thunderstorm
            { 95, new WeatherInfo("Orage",                     WeatherIconType.Thunderstorm) },
            { 96, new WeatherInfo("Orage avec gr\u00eale l\u00e9g\u00e8re",  WeatherIconType.Thunderstorm) },
            { 99, new WeatherInfo("Orage avec gr\u00eale forte",    WeatherIconType.Thunderstorm) },
        };

        // Sprite name mapping for Resources.Load
        private static readonly Dictionary<WeatherIconType, string> IconResourceNames = new Dictionary<WeatherIconType, string>
        {
            { WeatherIconType.Clear,          "Weather/Icons/weather_clear" },
            { WeatherIconType.PartlyCloudy,   "Weather/Icons/weather_partly_cloudy" },
            { WeatherIconType.Cloudy,         "Weather/Icons/weather_cloudy" },
            { WeatherIconType.Fog,            "Weather/Icons/weather_fog" },
            { WeatherIconType.Drizzle,        "Weather/Icons/weather_drizzle" },
            { WeatherIconType.Rain,           "Weather/Icons/weather_rain" },
            { WeatherIconType.Snow,           "Weather/Icons/weather_snow" },
            { WeatherIconType.Thunderstorm,   "Weather/Icons/weather_thunderstorm" },
        };

        // Sprite cache to avoid repeated Resources.Load calls
        private static readonly Dictionary<WeatherIconType, Sprite> SpriteCache = new Dictionary<WeatherIconType, Sprite>();

        /// <summary>
        /// Returns a French description for the given WMO weather code.
        /// </summary>
        public static string GetDescription(int wmoCode)
        {
            if (CodeMap.TryGetValue(wmoCode, out WeatherInfo info))
                return info.Description;

            return "Inconnu";
        }

        /// <summary>
        /// Returns the WeatherIconType for the given WMO weather code.
        /// </summary>
        public static WeatherIconType GetIconType(int wmoCode)
        {
            if (CodeMap.TryGetValue(wmoCode, out WeatherInfo info))
                return info.IconType;

            return WeatherIconType.Clear;
        }

        /// <summary>
        /// Loads and caches the weather icon sprite from Resources for the given WMO code.
        /// Returns null if the sprite is not found.
        /// </summary>
        public static Sprite GetIcon(int wmoCode)
        {
            WeatherIconType iconType = GetIconType(wmoCode);

            if (SpriteCache.TryGetValue(iconType, out Sprite cached) && cached != null)
                return cached;

            if (IconResourceNames.TryGetValue(iconType, out string resourcePath))
            {
                Sprite sprite = Resources.Load<Sprite>(resourcePath);
                if (sprite != null)
                {
                    SpriteCache[iconType] = sprite;
                }
                else
                {
                    Debug.LogWarning($"[WeatherCodeMapper] Sprite not found at Resources/{resourcePath}");
                }
                return sprite;
            }

            return null;
        }
    }
}

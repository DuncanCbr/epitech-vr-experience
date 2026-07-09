using System;
using UnityEngine;

/// <summary>
/// Data models matching the Open-Meteo API JSON response.
/// Used by WeatherService for deserialization via JsonUtility.
/// </summary>
namespace Weather.Data
{
    [Serializable]
    public class WeatherResponse
    {
        public float latitude;
        public float longitude;
        public string timezone;
        public CurrentWeather current;
        public CurrentUnits current_units;
        public DailyForecast daily;
    }

    [Serializable]
    public class CurrentUnits
    {
        public string temperature_2m;
        public string relative_humidity_2m;
        public string apparent_temperature;
        public string precipitation;
        public string wind_speed_10m;
    }

    [Serializable]
    public class CurrentWeather
    {
        public string time;
        public float temperature_2m;
        public int relative_humidity_2m;
        public float apparent_temperature;
        public float precipitation;
        public int weather_code;
        public float wind_speed_10m;
    }

    [Serializable]
    public class DailyForecast
    {
        public string[] time;              // 7 ISO date strings
        public int[] weather_code;         // 7 WMO weather codes
        public float[] temperature_2m_max;
        public float[] temperature_2m_min;
        public float[] precipitation_sum;
    }
}

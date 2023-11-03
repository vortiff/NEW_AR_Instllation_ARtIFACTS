using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WeatherDataManager : MonoBehaviour
{
    [Header("OpenWeatheMap Setting")]
    public string apiKey;
    private string city = "freiburg";
    private string baseUrl = "https://api.openweathermap.org/data/2.5/weather";

    private static WeatherInfo currentWeatherInfo;

    private void Start()
    {
        StartCoroutine(FetchWeatherData());
    }

    public WeatherInfo GetCurrentWeatherInfo()
    {
        return currentWeatherInfo;
    }

    private IEnumerator FetchWeatherData()
    {
        string url = $"{baseUrl}?q={city}&appid={apiKey}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = request.downloadHandler.text;
            currentWeatherInfo = JsonUtility.FromJson<WeatherInfo>(jsonResult);
            float tempInCelsius = currentWeatherInfo.main.temp - 273.15f;
            Debug.Log("Fetched Temp in Celsius: " + tempInCelsius);

        }
        else
        {
            Debug.LogError("Errore nella richiesta meteo: " + request.error);
        }
    }

    [System.Serializable]
    public class WeatherInfo
    {
        public MainInfo main;
    }

    [System.Serializable]
    public class MainInfo
    {
        public float temp;
    }
}

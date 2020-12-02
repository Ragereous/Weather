using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Weather.Models;
using Weather.Views;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using QuickType;

namespace Weather.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Forecast(IFormCollection data)
        {
            string street = data["street"];
            string city = data["city"];
            string state = data["state"];
            string zip = data["zip"];

            string url = $"https://geocoding.geo.census.gov/geocoder/locations/address?street={street}&city={city}&state={state}&zip={zip}&benchmark=Public_AR_Census2010&format=json";

            HttpClient client = new HttpClient();
            var result = await client.GetStringAsync(url);

            dynamic T = JsonConvert.DeserializeObject(result);

            dynamic Coords = T?.result?.addressMatches?[0]?.coordinates;
            float x = Coords?.x;
            float y = Coords?.y;
            string weatherUrl = $"https://api.weather.gov/points/{y},{x}";

            client.DefaultRequestHeaders.Add("User-Agent", "C# App");

            var forwardResult = await client.GetStringAsync(weatherUrl);

            dynamic U = JsonConvert.DeserializeObject(forwardResult);

            dynamic Forward = U?.properties;
            string forecastUrl = Forward?.forecast;

            var forecastData = await client.GetStringAsync(forecastUrl);
            Forecast forecast = JsonConvert.DeserializeObject<Forecast>(forecastData, Converter.Settings);

            return View(forecast);
        }
    }
}

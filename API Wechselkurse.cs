using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

class Program
{
    private static readonly string ApiUrl = "http://api.exchangeratesapi.io/v1/latest?access_key=1d8a4f0b9df29aeb58a176c1c99fd7fc";
    private static readonly HttpClient HttpClient = new HttpClient();

    static async Task Main()
    {
        Console.WriteLine("Auf welche Funktion möchten sie zugreifen?");
        Console.WriteLine("------------------------------------------");
        Console.WriteLine("pf = Portfolio");
        Console.WriteLine("cc= Währungsrechner");
        
        try
        {
            var jsonData = await GetApiDataAsync(ApiUrl);
            if (jsonData == null)
            {
                Console.WriteLine("Fehler beim Abrufen von Daten von der API.");
                return;
            }

            var rates = jsonData["rates"] as JObject;
            if (rates == null)
            {
                Console.WriteLine("Fehler beim Lesen der Wechselkurse.");
                return;
            }

            Console.WriteLine("\nGeben Sie die Basiswährung ein.");
            Console.Write("=> ");
            string baseCurrency = Console.ReadLine().ToUpper();

            Console.WriteLine("Geben Sie die Zielwährung ein.");
            Console.Write("=> ");
            string targetCurrency = Console.ReadLine().ToUpper();

            double exchangeRate = GetExchangeRate(rates, baseCurrency, targetCurrency);

            if (exchangeRate != -1)
            {
                Console.WriteLine($"Der Wechselkurs von {baseCurrency} zu {targetCurrency} beträgt: {exchangeRate}");
            }
            else
            {
                Console.WriteLine("Der Wechselkurs für das angegebene Währungspaar wurde nicht gefunden.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ein Fehler ist aufgetreten: {ex.Message}");
        }
    }

    static async Task<JObject> GetApiDataAsync(string apiUrl)
    {
        try
        {
            var response = await HttpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            return JObject.Parse(content);
        }
        catch (HttpRequestException hre)
        {
            Console.WriteLine($"Netzwerkfehler: {hre.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Allgemeiner Fehler: {ex.Message}");
        }
        return null;
    }

    static double GetExchangeRate(JObject rates, string baseCurrency, string targetCurrency)
    {
        if (rates.TryGetValue(targetCurrency, out JToken exchangeRateToken))
        {
            return exchangeRateToken.Value<double>();
        }
        return -1; // Wechselkurs nicht gefunden
    }
}

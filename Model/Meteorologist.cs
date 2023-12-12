using Lab6_Starter;
using RestSharp;
using System.Text.Json;

namespace Lab6_Starter.Model;
/// <summary>
/// Implementation of API for Lab7P2 (Lab8) done by Group 5.
/// BusinessLogic layer file for Weather page.
/// </summary>
public static class Meteorologist
{
    private static readonly string keyAPI = "53b1c7659260454e889e86c373"; //our api key for accessing https://www.checkwxapi.com

    /// <summary>
    /// gets Metar and formats it for the weatherPage
    /// </summary>
    public static string GetMetar(string airportId)
    {
        string metarClientString = "https://api.checkwx.com/metar/" + airportId;
        RestClient client = new(metarClientString);
        RestRequest request = new()
        {
            Method = Method.Get
        };

        request.AddHeader("X-API-Key", keyAPI);
        RestResponse response = client.Execute(request);
        //checks response validity 
        if(response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
        {
            return "Failed to receive a response from www.checkwxapi.com. Try again later.";
        }
        string jsonResponse = response.Content;
        try
        {
            using (JsonDocument document = JsonDocument.Parse(jsonResponse))
            {
                JsonElement root = document.RootElement;

                // Check if the structure is as expected
                if (root.TryGetProperty("data", out JsonElement dataElement) && dataElement.GetArrayLength() > 0)
                {
                    // Get the first item in the 'data' array
                    JsonElement firstDataElement = dataElement.EnumerateArray().First();

                    // Extract the string value
                    return firstDataElement.GetString();
                }
            }
            using (JsonDocument document = JsonDocument.Parse(jsonResponse))
            {
                JsonElement root = document.RootElement;
                if (root.TryGetProperty("results", out JsonElement dataElement))
                {
                    return dataElement.ToString();
                }
            }
            return "Unexpected JSON structure";
        }
        catch (JsonException ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// gets taf and formats it for the weatherPage
    /// </summary>
    public static string GetTaf(string airportId)
    {
        string tafClientString = "https://api.checkwx.com/taf/" + airportId;
        RestClient client = new(tafClientString);
        RestRequest request = new()
        {
            Method = Method.Get
        };

        request.AddHeader("X-API-Key", keyAPI);
        RestResponse response = client.Execute(request);
        string jsonResponse = response.Content;
        try
        {
            using (JsonDocument document = JsonDocument.Parse(jsonResponse))
            {
                JsonElement root = document.RootElement;

                // Check if the structure is as expected
                if (root.TryGetProperty("data", out JsonElement dataElement) && dataElement.GetArrayLength() > 0)
                {
                    // Get the first item in the 'data' array
                    JsonElement firstDataElement = dataElement.EnumerateArray().First();

                    // Extract the string value
                    return firstDataElement.GetString();
                }
            }
            using (JsonDocument document = JsonDocument.Parse(jsonResponse))
            {
                JsonElement root = document.RootElement;
                if (root.TryGetProperty("results", out JsonElement dataElement))
                {
                    return dataElement.ToString();
                }
            }
                return "Unexpected JSON structure";
        }
        catch (JsonException ex)
        {
            throw ex;
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Singer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopSingersController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = "00358d90b2d393caa8047d115a1dd202"; // Your Last.fm API key

        public TopSingersController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public async Task<IActionResult> GetTopSingers()
        {
            string url = $"https://ws.audioscrobbler.com/2.0/?method=geo.gettopartists&country=India&api_key={ApiKey}&format=json";

            try
            {
                var rawJson = await _httpClient.GetStringAsync(url);
                using var json = JsonDocument.Parse(rawJson);

                var topArtists = new List<object>();

                if (json.RootElement.TryGetProperty("topartists", out var topArtistsElement) &&
                    topArtistsElement.TryGetProperty("artist", out var artistsArray))
                {
                    foreach (var artist in artistsArray.EnumerateArray())
                    {
                        // Safely get each property
                        string name = artist.TryGetProperty("name", out var nameProp) ? nameProp.GetString() ?? "" : "";
                        string playcount = artist.TryGetProperty("playcount", out var playProp) ? playProp.GetString() ?? "" : "";
                        string listeners = artist.TryGetProperty("listeners", out var listenersProp) ? listenersProp.GetString() ?? "" : "";
                        string url1 = artist.TryGetProperty("url", out var urlProp) ? urlProp.GetString() ?? "" : "";

                        // Safely get image (medium size, if available)
                        string image = "";
                        if (artist.TryGetProperty("image", out var imageArray) && imageArray.GetArrayLength() > 2)
                        {
                            var imgElement = imageArray[2];
                            image = imgElement.TryGetProperty("#text", out var imgProp) ? imgProp.GetString() ?? "" : "";
                        }

                        topArtists.Add(new
                        {
                            Name = name,
                            PlayCount = playcount,
                            Listeners = listeners,
                            Url = url1,
                            Image = image
                        });
                    }
                }

                return Ok(topArtists);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(502, $"Failed to reach Last.fm API: {ex.Message}");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }
    }
}

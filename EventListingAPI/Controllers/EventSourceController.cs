using System.Net.Http;
using System.Security.Principal;
using EventListingAPI.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;


namespace EventListingAPI.Controllers
{

    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("api/eventsource")]
    public class EventSourceController : ControllerBase
    {
        private readonly ConfigurationSettings _config;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public EventSourceController(IOptions<ConfigurationSettings> config, IHttpClientFactory httpClientFactory, IWebHostEnvironment webHostEnvironment)
        {
            _config = config.Value;
            _httpClientFactory = httpClientFactory;
            _webHostEnvironment = webHostEnvironment;
        }


        [HttpGet("")]
        public async Task<ActionResult<string>> GetEventsFromSource()
        {
            bool isDataFromAPI = false;
            try
            {
                using (var client = _httpClientFactory.CreateClient())
                {
                    if (!string.IsNullOrWhiteSpace(_config.EventSourceUrl))
                    {
                        client.BaseAddress = new Uri(_config.EventSourceUrl ?? string.Empty);
                        var eventsWithVenuesResponse = await client.GetFromJsonAsync<EventsWithVenues>("");
                        if (eventsWithVenuesResponse != null)
                        {
                            isDataFromAPI = true;
                            return Ok(eventsWithVenuesResponse);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isDataFromAPI = false;
                return StatusCode(500, "There is some error occured while fetching the data from event source");
            }

            try
            {
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, string.Empty, _config.LocalEventSourceFileName);

                if (System.IO.File.Exists(filePath) == false)
                    return NotFound();

                string fileContent = await System.IO.File.ReadAllTextAsync(filePath);

                if (string.IsNullOrEmpty(fileContent))
                    return NotFound();

                return Ok(JsonConvert.DeserializeObject<EventsWithVenues>(fileContent));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching fallback events data: {ex.Message}");
                return StatusCode(500, "There is some error occured while fetching the data from event source");
            }
        }
    }

}

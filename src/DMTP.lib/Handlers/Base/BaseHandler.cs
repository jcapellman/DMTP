using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using NLog;

namespace DMTP.lib.Handlers.Base
{
    public class BaseHandler
    {
        private readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly string _rootURL;

        protected BaseHandler(string rootURL)
        {
            if (string.IsNullOrEmpty(rootURL))
            {
                throw new ArgumentNullException(nameof(rootURL));
            }

            _rootURL = rootURL;
        }

        protected async Task<T> GetAsync<T>(string url)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(_rootURL);

                    var response = httpClient.GetAsync(url).Result;

                    var responseBody = await response.Content.ReadAsStringAsync();

                    Log.Debug($"Url: {url} | Response: {responseBody}");

                    return response.IsSuccessStatusCode ? JsonConvert.DeserializeObject<T>(responseBody) : default;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to get {url} due to {ex}");

                return default;
            }
        }

        protected async Task<bool> PostAsync<T>(string url, T data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(_rootURL);

                    var json = JsonConvert.SerializeObject(data);

                    var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = httpClient.PostAsync(url, stringContent).Result;

                    var responseBody = await response.Content.ReadAsStringAsync();

                    Log.Debug($"Url: {url} | Response: {responseBody}");

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to POST to {url} due to {ex}");

                return false;
            }
        }
    }
}
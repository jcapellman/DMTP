using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using NLog;

namespace DMTP.lib.Handlers.Base
{
    public abstract class BaseHandler
    {
        protected abstract string RootAPI { get; }

        private readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly string _rootURL;

        private readonly string _registrationKey;

        protected BaseHandler(string rootURL, string registrationKey)
        {
            if (string.IsNullOrEmpty(rootURL))
            {
                throw new ArgumentNullException(nameof(rootURL));
            }

            if (string.IsNullOrEmpty(registrationKey))
            {
                throw new ArgumentNullException(nameof(registrationKey));
            }

            _rootURL = rootURL;
            _registrationKey = registrationKey;
        }

        private static HttpClientHandler GetHttpClientHandler() => new HttpClientHandler { DefaultProxyCredentials = CredentialCache.DefaultCredentials };

        protected async Task<T> GetAsync<T>(string url)
        {
            try
            {
                using (var httpClient = new HttpClient(GetHttpClientHandler()))
                {
                    httpClient.BaseAddress = new Uri(_rootURL);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _registrationKey);

                    var response = httpClient.GetAsync($"{RootAPI}{url}").Result;

                    var responseBody = await response.Content.ReadAsStringAsync();

                    Log.Debug($"Url: {_rootURL}{RootAPI}{url} | Response: {responseBody}");

                    return response.IsSuccessStatusCode ? JsonConvert.DeserializeObject<T>(responseBody) : default;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to get {_rootURL}{RootAPI}{url} due to {ex}");

                return default;
            }
        }

        protected async Task<bool> PostAsync<T>(T data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            try
            {
                using (var httpClient = new HttpClient(GetHttpClientHandler()))
                {
                    httpClient.BaseAddress = new Uri(_rootURL);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _registrationKey);

                    var json = JsonConvert.SerializeObject(data);

                    var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = httpClient.PostAsync(RootAPI, stringContent).Result;

                    var responseBody = await response.Content.ReadAsStringAsync();

                    Log.Debug($"Url: {RootAPI} | Response: {responseBody}");

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to POST to {RootAPI} due to {ex}");

                return false;
            }
        }
    }
}
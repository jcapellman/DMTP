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

        private readonly string _rootUrl;

        private readonly string _registrationKey;

        protected BaseHandler(string rootUrl, string registrationKey)
        {
            if (string.IsNullOrEmpty(rootUrl))
            {
                throw new ArgumentNullException(nameof(rootUrl));
            }

            if (string.IsNullOrEmpty(registrationKey))
            {
                throw new ArgumentNullException(nameof(registrationKey));
            }

            _rootUrl = rootUrl;
            _registrationKey = registrationKey;
        }

        private static HttpClientHandler GetHttpClientHandler() => new HttpClientHandler { DefaultProxyCredentials = CredentialCache.DefaultCredentials };

        protected async Task<T> GetAsync<T>(string url)
        {
            try
            {
                using (var httpClient = new HttpClient(GetHttpClientHandler()))
                {
                    var fullUrl = $"{RootAPI}{url}";

                    httpClient.BaseAddress = new Uri(_rootUrl);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _registrationKey);

                    var response = await httpClient.GetAsync(fullUrl);

                    Log.Debug($"URL: {_rootUrl}{fullUrl} | Response: {response.StatusCode}");

                    var responseBody = await response.Content.ReadAsStringAsync();

                    Log.Debug($"Url: {_rootUrl}{fullUrl} | Response: {responseBody}");

                    return response.IsSuccessStatusCode ? JsonConvert.DeserializeObject<T>(responseBody) : default;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to get {_rootUrl}{RootAPI}{url} due to {ex}");

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
                    httpClient.BaseAddress = new Uri(_rootUrl);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _registrationKey);

                    var json = JsonConvert.SerializeObject(data);

                    var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                    Log.Debug($"URL: {RootAPI} | JSON: {json}");

                    var response = await httpClient.PostAsync(RootAPI, stringContent);

                    Log.Debug($"URL: {RootAPI} | StatusCode: {response.StatusCode}");

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
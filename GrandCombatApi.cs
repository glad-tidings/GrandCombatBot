using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GrandCombatBot
{

    public class GrandCombatApi
    {
        private readonly HttpClient client;

        public GrandCombatApi(string queryID, int queryIndex, int tapsCount, int updateTaps, DateTime lastSync, DateTime lastTap, HttpContent Payload, ProxyType[] Proxy)
        {
            string RequestDate = PayloadEncrypt(Payload).Result;

            var FProxy = Proxy.Where(x => x.Index == queryIndex);
            if (FProxy.Count() != 0)
            {
                if (!string.IsNullOrEmpty(FProxy.ElementAtOrDefault(0)?.Proxy))
                {
                    var handler = new HttpClientHandler() { Proxy = new WebProxy() { Address = new Uri(FProxy.ElementAtOrDefault(0)?.Proxy ?? string.Empty) } };
                    client = new HttpClient(handler) { Timeout = new TimeSpan(0, 0, 30) };
                }
                else
                    client = new HttpClient() { Timeout = new TimeSpan(0, 0, 30) };
            }
            else
                client = new HttpClient() { Timeout = new TimeSpan(0, 0, 30) };
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true, NoStore = true, MaxAge = TimeSpan.FromSeconds(0d) };
            client.DefaultRequestHeaders.Add("Authorization", $"tma {queryID}");
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US");
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            client.DefaultRequestHeaders.Add("Pragma", "no-cache");
            client.DefaultRequestHeaders.Add("Priority", "u=1, i");
            client.DefaultRequestHeaders.Add("Origin", "https://app.grandcombat.io");
            client.DefaultRequestHeaders.Add("Referer", "https://app.grandcombat.io/");
            client.DefaultRequestHeaders.Add("Request-Date", RequestDate);
            client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
            client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-site");
            client.DefaultRequestHeaders.Add("Sec-Ch-Ua", "\"Google Chrome\";v=\"125\", \"Chromium\";v=\"125\", \"Not.A/Brand\";v=\"24\"");
            client.DefaultRequestHeaders.Add("User-Agent", Tools.getUserAgents(queryIndex));
            client.DefaultRequestHeaders.Add("Last-Sync-At", lastSync.ToString());
            client.DefaultRequestHeaders.Add("Last-Tap-At", lastTap.ToString());
            client.DefaultRequestHeaders.Add("accept", "application/json, text/plain, */*");
            client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
            client.DefaultRequestHeaders.Add("taps-count", tapsCount.ToString());
            client.DefaultRequestHeaders.Add("update-taps", updateTaps.ToString());
            client.DefaultRequestHeaders.Add("sec-ch-ua-platform", $"\"{Tools.getUserAgents(queryIndex, true)}\"");
        }

        private async Task<string> PayloadEncrypt(HttpContent Payload)
        {
            var client = new HttpClient() { Timeout = new TimeSpan(0, 0, 30) };
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true, NoStore = true, MaxAge = TimeSpan.FromSeconds(0d) };
            HttpResponseMessage httpResponse = null;
            try
            {
                httpResponse = await client.PostAsync("http://gcb.parselecom.com", Payload);
            }
            catch { }
            if (httpResponse is not null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<GrandCombatPayloadResponse>(responseStream);
                    return responseJson?.Data ?? string.Empty;
                }
            }

            return "";
        }

        public async Task<HttpResponseMessage> GCAPIGet(string requestUri)
        {
            try
            {
                return await client.GetAsync(requestUri);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.ExpectationFailed, ReasonPhrase = ex.Message };
            }
        }

        public async Task<HttpResponseMessage> GCAPIPost(string requestUri, HttpContent content)
        {
            try
            {
                return await client.PostAsync(requestUri, content);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.ExpectationFailed, ReasonPhrase = ex.Message };
            }
        }
    }
}
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GrandCombatBot
{

    public class GrandCombatBots
    {

        public readonly GrandCombatQuery PubQuery;
        private readonly ProxyType[] PubProxy;
        public readonly GrandCombatAuthResponse UserDetail;
        public readonly long[] Level = new[] { 2500L, 450000L, 4000000L, 9000000L, 500000000L, 5000000000L };
        public readonly bool HasError;
        public readonly string ErrorMessage;
        public readonly string IPAddress;

        public GrandCombatBots(GrandCombatQuery Query, ProxyType[] Proxy)
        {
            PubQuery = Query;
            PubProxy = Proxy;
            IPAddress = GetIP().Result;
            PubQuery.Auth = getSession().Result;
            var GetUser = GrandCombatAuthAsync().Result;
            if (GetUser != null)
            {
                UserDetail = GetUser;
                HasError = false;
                ErrorMessage = "";
            }
            else
            {
                UserDetail = new();
                HasError = true;
                ErrorMessage = "login failed";
            }
        }

        private async Task<string> GetIP()
        {
            HttpClient client;
            var FProxy = PubProxy.Where(x => x.Index == PubQuery.Index);
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
            HttpResponseMessage httpResponse = null;
            try
            {
                httpResponse = await client.GetAsync($"https://httpbin.org/ip");
            }
            catch { }
            if (httpResponse != null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<Httpbin>(responseStream);
                    return responseJson?.Origin ?? string.Empty;
                }
            }

            return "";
        }

        private async Task<string> getSession()
        {
            var vw = new TelegramMiniApp.WebView(PubQuery.API_ID, PubQuery.API_HASH, PubQuery.Name, PubQuery.Phone, "grandcombat_bot", "https://app.grandcombat.io/");
            string url = await vw.Get_URL();
            if (!string.IsNullOrEmpty(url))
                return url.Split(new string[] { "tgWebAppData=" }, StringSplitOptions.None)[1].Split(new string[] { "&tgWebAppVersion" }, StringSplitOptions.None)[0];
            else
                return "";
        }

        private async Task<GrandCombatAuthResponse?> GrandCombatAuthAsync()
        {
            var GCAPI = new GrandCombatApi(PubQuery.Auth, PubQuery.Index, 0, 0, DateTime.Now.ToLocalTime(), DateTime.Now.ToLocalTime(), null, PubProxy);
            var httpResponse = await GCAPI.GCAPIPost("https://api.grandcombat.io/auth/sign", null);
            if (httpResponse != null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var options = new JsonSerializerOptions() { NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString };
                    var responseJson = await JsonSerializer.DeserializeAsync<GrandCombatAuthResponse>(responseStream, options);
                    return responseJson;
                }
            }

            return null;
        }

        public async Task<GrandCombatAuthResponse?> GrandCombatUserDetailAsync()
        {
            var GCAPI = new GrandCombatApi(PubQuery.Auth, PubQuery.Index, 0, 0, DateTime.Now.ToLocalTime(), DateTime.Now.ToLocalTime(), null, PubProxy);
            var httpResponse = await GCAPI.GCAPIPost("https://api.grandcombat.io/auth/sign", null);
            if (httpResponse != null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var options = new JsonSerializerOptions() { NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString };
                    var responseJson = await JsonSerializer.DeserializeAsync<GrandCombatAuthResponse>(responseStream, options);
                    return responseJson;
                }
            }

            return null;
        }

        public async Task<bool> GrandCombatTapAsync(int tapsCount, bool updateTaps)
        {
            var request = new GrandCombatTapRequest() { UpdateTaps = updateTaps };
            string serializedRequest = JsonSerializer.Serialize(request);
            var serializedRequestContent = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
            var GCAPI = new GrandCombatApi(PubQuery.Auth, PubQuery.Index, tapsCount, Convert.ToInt32(updateTaps), DateTime.Now.ToLocalTime(), DateTime.Now.ToLocalTime(), serializedRequestContent, PubProxy);
            var httpResponse = await GCAPI.GCAPIPost("https://api.grandcombat.io/game/update-tap-balance", serializedRequestContent);
            if (httpResponse != null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<GrandCombatTapsResponse>(responseStream);
                    return responseJson.Success;
                }
            }

            return false;
        }

        public async Task<bool> GrandCombatDailyRewardAsync()
        {
            var GCAPI = new GrandCombatApi(PubQuery.Auth, PubQuery.Index, 0, 0, DateTime.Now.ToLocalTime(), DateTime.Now.ToLocalTime(), null, PubProxy);
            var httpResponse = await GCAPI.GCAPIPost("https://api.grandcombat.io/game/claim-daily-bonus", null);
            if (httpResponse != null)
                return httpResponse.IsSuccessStatusCode;
            else
                return false;
        }

        public async Task<bool> GrandCombatClaimChestAsync(string chestId)
        {
            var GCAPI = new GrandCombatApi(PubQuery.Auth, PubQuery.Index, 0, 0, DateTime.Now.ToLocalTime(), DateTime.Now.ToLocalTime(), null, PubProxy);
            var httpResponse = await GCAPI.GCAPIPost($"https://api.grandcombat.io/game/claim-chest/{chestId}", null);
            if (httpResponse != null)
                return httpResponse.IsSuccessStatusCode;
            else
                return false;
        }

        public async Task<bool> GrandCombatUpgradeLevelAsync()
        {
            var GCAPI = new GrandCombatApi(PubQuery.Auth, PubQuery.Index, 0, 0, DateTime.Now.ToLocalTime(), DateTime.Now.ToLocalTime(), null, PubProxy);
            var httpResponse = await GCAPI.GCAPIPost("https://api.grandcombat.io/game/level-up", null);
            if (httpResponse != null)
                return httpResponse.IsSuccessStatusCode;
            else
                return false;
        }

        public async Task<bool> GrandCombatUpgradeBoostAsync(GrandCombatBoostRequest request)
        {
            string serializedRequest = JsonSerializer.Serialize(request);
            var serializedRequestContent = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
            var GCAPI = new GrandCombatApi(PubQuery.Auth, PubQuery.Index, 0, 0, (DateTime)request.LastSyncDate, (DateTime)request.LastTapDate, serializedRequestContent, PubProxy);
            var httpResponse = await GCAPI.GCAPIPost($"https://api.grandcombat.io/game/upgrade-boost", serializedRequestContent);
            if (httpResponse != null)
                return httpResponse.IsSuccessStatusCode;
            else
                return false;
        }

        public async Task<bool> GrandCombatUseBoostAsync(GrandCombatBoostRequest request)
        {
            string serializedRequest = JsonSerializer.Serialize(request);
            var serializedRequestContent = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
            var GCAPI = new GrandCombatApi(PubQuery.Auth, PubQuery.Index, 0, 0, (DateTime)request.LastSyncDate, (DateTime)request.LastTapDate, serializedRequestContent, PubProxy);
            var httpResponse = await GCAPI.GCAPIPost("https://api.grandcombat.io/game/use-boost", serializedRequestContent);
            if (httpResponse != null)
                return httpResponse.IsSuccessStatusCode;
            else
                return false;
        }

        public async Task<List<GrandCombatUserCards>> GrandCombatUserCardsAsync()
        {
            var GCUC = new List<GrandCombatUserCards>();
            var GCAPI = new GrandCombatApi(PubQuery.Auth, PubQuery.Index, 0, 0, DateTime.Now.ToLocalTime(), DateTime.Now.ToLocalTime(), null, PubProxy);
            var httpResponse = await GCAPI.GCAPIGet("https://api.grandcombat.io/user-cards/team");
            if (httpResponse != null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var options = new JsonSerializerOptions() { NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString };
                    var responseJson = await JsonSerializer.DeserializeAsync<List<GrandCombatUserCardsResponse>>(responseStream, options);
                    foreach (var resp in responseJson)
                        GCUC.Add(new GrandCombatUserCards()
                        {
                            Available = resp.Tasks.Count == 0,
                            Category = resp.Category,
                            Hidden = resp.Hidden,
                            Id = resp.Id,
                            IncomePerHour = resp.IncomePerHour,
                            LastUpgradeDate = resp.LastUpgradeDate,
                            Level = resp.Level,
                            NextIncomePerHour = resp.NextLevelStats.IncomePerHour,
                            NextPrice = resp.NextLevelStats.Price,
                            TimeoutDeadline = resp.TimeoutDeadline,
                            Title = resp.Title.En
                        });
                }
            }
            Thread.Sleep(2000);
            httpResponse = await GCAPI.GCAPIGet("https://api.grandcombat.io/user-cards/market");
            if (httpResponse != null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var options = new JsonSerializerOptions() { NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString };
                    var responseJson = await JsonSerializer.DeserializeAsync<List<GrandCombatUserCardsResponse>>(responseStream, options);
                    foreach (var resp in responseJson)
                        GCUC.Add(new GrandCombatUserCards()
                        {
                            Available = resp.Tasks.Count == 0,
                            Category = resp.Category,
                            Hidden = resp.Hidden,
                            Id = resp.Id,
                            IncomePerHour = resp.IncomePerHour,
                            LastUpgradeDate = resp.LastUpgradeDate,
                            Level = resp.Level,
                            NextIncomePerHour = resp.NextLevelStats.IncomePerHour,
                            NextPrice = resp.NextLevelStats.Price,
                            TimeoutDeadline = resp.TimeoutDeadline,
                            Title = resp.Title.En
                        });
                }
            }
            Thread.Sleep(2000);
            httpResponse = await GCAPI.GCAPIGet("https://api.grandcombat.io/user-cards/legal");
            if (httpResponse != null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var options = new JsonSerializerOptions() { NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString };
                    var responseJson = await JsonSerializer.DeserializeAsync<List<GrandCombatUserCardsResponse>>(responseStream, options);
                    foreach (var resp in responseJson)
                        GCUC.Add(new GrandCombatUserCards()
                        {
                            Available = resp.Tasks.Count == 0,
                            Category = resp.Category,
                            Hidden = resp.Hidden,
                            Id = resp.Id,
                            IncomePerHour = resp.IncomePerHour,
                            LastUpgradeDate = resp.LastUpgradeDate,
                            Level = resp.Level,
                            NextIncomePerHour = resp.NextLevelStats.IncomePerHour,
                            NextPrice = resp.NextLevelStats.Price,
                            TimeoutDeadline = resp.TimeoutDeadline,
                            Title = resp.Title.En
                        });
                }
            }
            Thread.Sleep(2000);
            httpResponse = await GCAPI.GCAPIGet("https://api.grandcombat.io/user-cards/specials");
            if (httpResponse != null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var options = new JsonSerializerOptions() { NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString };
                    var responseJson = await JsonSerializer.DeserializeAsync<GrandCombatSpecialsUserCardsResponse>(responseStream, options);
                    foreach (var resp in responseJson.NewCards)
                        GCUC.Add(new GrandCombatUserCards()
                        {
                            Available = resp.Tasks.Count == 0,
                            Category = resp.Category,
                            Hidden = resp.Hidden,
                            Id = resp.Id,
                            IncomePerHour = resp.IncomePerHour,
                            LastUpgradeDate = resp.LastUpgradeDate,
                            Level = resp.Level,
                            NextIncomePerHour = resp.NextLevelStats.IncomePerHour,
                            NextPrice = resp.NextLevelStats.Price,
                            TimeoutDeadline = resp.TimeoutDeadline,
                            Title = resp.Title.En
                        });
                }
            }

            return GCUC;
        }

        public async Task<bool> GrandCombatUpgradeCardAsync(string category, string cardId)
        {
            var GCAPI = new GrandCombatApi(PubQuery.Auth, PubQuery.Index, 0, 0, DateTime.Now.ToLocalTime(), DateTime.Now.ToLocalTime(), null, PubProxy);
            var httpResponse = await GCAPI.GCAPIPost($"https://api.grandcombat.io/user-cards/level-up/{cardId}/{category}", null);
            if (httpResponse != null)
                return httpResponse.IsSuccessStatusCode;
            else
                return false;
        }

        public async Task<GrandCombatComboAnswerResponse?> GrandCombatComboAnswerAsync()
        {
            var client = new HttpClient() { Timeout = new TimeSpan(0, 0, 30) };
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true, NoStore = true, MaxAge = TimeSpan.FromSeconds(0d) };
            HttpResponseMessage httpResponse = null;
            try
            {
                httpResponse = await client.GetAsync($"https://raw.githubusercontent.com/glad-tidings/GrandCombatBot/refs/heads/main/combo.json");
            }
            catch (Exception ex)
            {
            }
            if (httpResponse != null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<GrandCombatComboAnswerResponse>(responseStream);
                    return responseJson;
                }
            }

            return null;
        }

        public async Task<bool> GrandCombatDailyComboAsync()
        {
            var GCAPI = new GrandCombatApi(PubQuery.Auth, PubQuery.Index, 0, 0, DateTime.Now.ToLocalTime(), DateTime.Now.ToLocalTime(), null, PubProxy);
            var httpResponse = await GCAPI.GCAPIPost($"https://api.grandcombat.io/combo/claim", null);
            if (httpResponse != null)
                return httpResponse.IsSuccessStatusCode;
            else
                return false;
        }

        public async Task<bool> GrandCombatJoinClanAsync(string clanId)
        {
            string serializedRequest = JsonSerializer.Serialize(new GrandCombatJoinClanRequest() { ClanId = clanId });
            var serializedRequestContent = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
            var GCAPI = new GrandCombatApi(PubQuery.Auth, PubQuery.Index, 0, 0, DateTime.Now.ToLocalTime(), DateTime.Now.ToLocalTime(), serializedRequestContent, PubProxy);
            var httpResponse = await GCAPI.GCAPIPost("https://api.grandcombat.io/clan/join", serializedRequestContent);
            if (httpResponse != null)
                return httpResponse.IsSuccessStatusCode;
            else
                return false;
        }

        public async Task<bool> GrandCombatLeaveClanAsync()
        {
            var GCAPI = new GrandCombatApi(PubQuery.Auth, PubQuery.Index, 0, 0, DateTime.Now.ToLocalTime(), DateTime.Now.ToLocalTime(), null, PubProxy);
            var httpResponse = await GCAPI.GCAPIPost("https://api.grandcombat.io/clan/leave", null);
            if (httpResponse != null)
                return httpResponse.IsSuccessStatusCode;
            else
                return false;
        }

        public async Task<GrandCombatBonusResponse?> GrandCombatBonusAsync()
        {
            var GCAPI = new GrandCombatApi(PubQuery.Auth, PubQuery.Index, 0, 0, DateTime.Now.ToLocalTime(), DateTime.Now.ToLocalTime(), null, PubProxy);
            var httpResponse = await GCAPI.GCAPIGet("https://api.grandcombat.io/game/get-bonus-balance");
            if (httpResponse != null)
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseJson = await JsonSerializer.DeserializeAsync<GrandCombatBonusResponse>(responseStream);
                    return responseJson;
                }
            }

            return null;
        }

        public async Task<bool> GrandCombatClaimBonusAsync()
        {
            var GCAPI = new GrandCombatApi(PubQuery.Auth, PubQuery.Index, 0, 0, DateTime.Now.ToLocalTime(), DateTime.Now.ToLocalTime(), null, PubProxy);
            var httpResponse = await GCAPI.GCAPIPost("https://api.grandcombat.io/game/claim-bonuses", null);
            if (httpResponse != null)
                return httpResponse.IsSuccessStatusCode;
            else
                return false;
        }
    }
}
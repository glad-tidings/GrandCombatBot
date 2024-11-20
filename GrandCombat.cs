using System.Text.Json.Serialization;

namespace GrandCombatBot
{
    public class GrandCombatQuery
    {
        public int Index { get; set; }
        public string Name { get; set; } = string.Empty;
        public string API_ID { get; set; } = string.Empty;
        public string API_HASH { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Auth { get; set; } = string.Empty;
        public bool Active { get; set; }
        public bool Tap { get; set; }
        public int[]? Taps { get; set; }
        public int[]? TapSleep { get; set; }
        public bool DailyReward { get; set; }
        public bool FriendBonus { get; set; }
        public bool Chest { get; set; }
        public bool DailyCombo { get; set; }
        public bool UpgradeLevel { get; set; }
        public bool UpgradeCard { get; set; }
        public int[]? UpgradeCardSleep { get; set; }
        public bool Boost { get; set; }
        public bool UpgradeBoost { get; set; }
        public int MaxMultitapLevel { get; set; }
        public int MaxEnergyLimitLevel { get; set; }
        public bool Task { get; set; }
        public int[]? TaskSleep { get; set; }
        public int[]? DaySleep { get; set; }
        public int[]? NightSleep { get; set; }
    }

    public class GrandCombatPayloadResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        [JsonPropertyName("msg")]
        public string Msg { get; set; } = string.Empty;
        [JsonPropertyName("data")]
        public string Data { get; set; } = string.Empty;
    }

    public class GrandCombatAuthResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("telegramId")]
        public long TelegramId { get; set; }
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;
        [JsonPropertyName("totalTaps")]
        public double TotalTaps { get; set; }
        [JsonPropertyName("currentEnergy")]
        public double CurrentEnergy { get; set; }
        [JsonPropertyName("lastTapDate")]
        public DateTime? LastTapDate { get; set; }
        [JsonPropertyName("lastSyncDate")]
        public DateTime? LastSyncDate { get; set; }
        [JsonPropertyName("lastEnergySet")]
        public DateTime? LastEnergySet { get; set; }
        [JsonPropertyName("lastIncomePerHour")]
        public DateTime? LastIncomePerHour { get; set; }
        [JsonPropertyName("lastLoginDate")]
        public DateTime? LastLoginDate { get; set; }
        [JsonPropertyName("level")]
        public int Level { get; set; }
        [JsonPropertyName("state")]
        public GrandCombatAuthState? State { get; set; }
    }

    public class GrandCombatAuthState
    {
        [JsonPropertyName("balance")]
        public GrandCombatAuthStateBalance? Balance { get; set; }
        [JsonPropertyName("clan")]
        public GrandCombatAuthStateClan? Clan { get; set; }
        [JsonPropertyName("stats")]
        public GrandCombatAuthStateStats? Stats { get; set; }
        [JsonPropertyName("dates")]
        public GrandCombatAuthStateDates? Dates { get; set; }
        [JsonPropertyName("boost")]
        public GrandCombatAuthStateBoost? Boost { get; set; }
        [JsonPropertyName("dailyBonus")]
        public GrandCombatAuthStateDailyBonus? DailyBonus { get; set; }
        [JsonPropertyName("tasks")]
        public List<GrandCombatAuthStateTask>? Tasks { get; set; }
        [JsonPropertyName("chests")]
        public GrandCombatAuthStateChests? Chests { get; set; }
        [JsonPropertyName("currentUserCombo")]
        public GrandCombatAuthStateCurrentUserCombo? CurrentUserCombo { get; set; }
    }

    public class GrandCombatAuthStateBalance
    {
        [JsonPropertyName("current")]
        public double Current { get; set; }
        [JsonPropertyName("total")]
        public double Total { get; set; }
    }

    public class GrandCombatAuthStateClan
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("owner")]
        public string Owner { get; set; } = string.Empty;
        [JsonPropertyName("level")]
        public int Level { get; set; }
        [JsonPropertyName("totalIncomePerHour")]
        public int TotalIncomePerHour { get; set; }
        [JsonPropertyName("membersCount")]
        public int MembersCount { get; set; }
    }

    public class GrandCombatAuthStateStats
    {
        [JsonPropertyName("incomePerTap")]
        public int IncomePerTap { get; set; }
        [JsonPropertyName("energyPerSecond")]
        public int EnergyPerSecond { get; set; }
        [JsonPropertyName("maxEnergy")]
        public int MaxEnergy { get; set; }
        [JsonPropertyName("level")]
        public int Level { get; set; }
        [JsonPropertyName("incomePerHour")]
        public int IncomePerHour { get; set; }
        [JsonPropertyName("totalTaps")]
        public double TotalTaps { get; set; }
        [JsonPropertyName("currentEnergy")]
        public double CurrentEnergy { get; set; }
        [JsonPropertyName("earnedBalance")]
        public double EarnedBalance { get; set; }
    }

    public class GrandCombatAuthStateDates
    {
        [JsonPropertyName("lastTapDate")]
        public DateTime? LastTapDate { get; set; }
        [JsonPropertyName("lastSyncDate")]
        public DateTime? LastSyncDate { get; set; }
        [JsonPropertyName("lastEnergySet")]
        public DateTime? LastEnergySet { get; set; }
        [JsonPropertyName("lastActionDate")]
        public DateTime? LastActionDate { get; set; }
        [JsonPropertyName("lastIncomePerHour")]
        public DateTime? LastIncomePerHour { get; set; }
    }

    public class GrandCombatAuthStateBoost
    {
        [JsonPropertyName("fullEnergy")]
        public int FullEnergy { get; set; }
        [JsonPropertyName("fullEnergyUsed")]
        public int FullEnergyUsed { get; set; }
        [JsonPropertyName("lastFullEnergyUsedDate")]
        public DateTime? LastFullEnergyUsedDate { get; set; }
        [JsonPropertyName("turbo")]
        public int Turbo { get; set; }
        [JsonPropertyName("turboUsed")]
        public int TurboUsed { get; set; }
        [JsonPropertyName("lastTurboUsedDate")]
        public DateTime? LastTurboUsedDate { get; set; }
        [JsonPropertyName("multitapLevel")]
        public int MultitapLevel { get; set; }
        [JsonPropertyName("energyLimitLevel")]
        public int EnergyLimitLevel { get; set; }
        [JsonPropertyName("lastFullEnergyMidnightRefreshDate")]
        public DateTime? LastFullEnergyMidnightRefreshDate { get; set; }
    }

    public class GrandCombatAuthStateDailyBonus
    {
        [JsonPropertyName("bonusClaimed")]
        public bool BonusClaimed { get; set; }
        [JsonPropertyName("nextBonus")]
        public int NextBonus { get; set; }
    }

    public class GrandCombatAuthStateTask
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
        [JsonPropertyName("title")]
        public GrandCombatAuthStateTaskTitle? Title { get; set; }
        [JsonPropertyName("amount")]
        public int Amount { get; set; }
    }

    public class GrandCombatAuthStateTaskTitle
    {
        [JsonPropertyName("en")]
        public string En { get; set; } = string.Empty;
    }

    public class GrandCombatAuthStateChests
    {
        [JsonPropertyName("lvl1")]
        public GrandCombatAuthStateChestsLvls? Lvl1 { get; set; }
        [JsonPropertyName("lvl2")]
        public GrandCombatAuthStateChestsLvls? Lvl2 { get; set; }
        [JsonPropertyName("lvl3")]
        public GrandCombatAuthStateChestsLvls? Lvl3 { get; set; }
    }

    public class GrandCombatAuthStateChestsLvls
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("title")]
        public GrandCombatAuthStateTaskTitle? Title { get; set; }
        [JsonPropertyName("claimed")]
        public bool Claimed { get; set; }
        [JsonPropertyName("available")]
        public bool Available { get; set; }
    }

    public class GrandCombatAuthStateCurrentUserCombo
    {
        [JsonPropertyName("comboActive")]
        public bool ComboActive { get; set; }
        [JsonPropertyName("claimed")]
        public bool Claimed { get; set; }
        [JsonPropertyName("cards")]
        public List<GrandCombatAuthStateCurrentUserComboCards>? Cards { get; set; }
    }

    public class GrandCombatAuthStateCurrentUserComboCards
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; } = string.Empty;
    }

    public class GrandCombatTapsResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }

    public class GrandCombatUserCards
    {
        public string Id { get; set; } = string.Empty;
        public DateTime? TimeoutDeadline { get; set; }
        public int Level { get; set; }
        public DateTime? LastUpgradeDate { get; set; }
        public int IncomePerHour { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool Available { get; set; }
        public bool Hidden { get; set; }
        public string Category { get; set; } = string.Empty;
        public int NextPrice { get; set; }
        public int NextIncomePerHour { get; set; }
    }

    public class GrandCombatSpecialsUserCardsResponse
    {
        [JsonPropertyName("newCards")]
        public List<GrandCombatUserCardsResponse>? NewCards { get; set; }
        [JsonPropertyName("wastedCards")]
        public List<GrandCombatUserCardsResponse>? WastedCards { get; set; }
        [JsonPropertyName("claimedCards")]
        public List<GrandCombatUserCardsResponse>? ClaimedCards { get; set; }
    }

    public class GrandCombatUserCardsResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("timeoutDeadline")]
        public DateTime? TimeoutDeadline { get; set; }
        [JsonPropertyName("level")]
        public int Level { get; set; }
        [JsonPropertyName("lastUpgradeDate")]
        public DateTime? LastUpgradeDate { get; set; }
        [JsonPropertyName("incomePerHour")]
        public int IncomePerHour { get; set; }
        [JsonPropertyName("title")]
        public GrandCombatAuthStateTaskTitle? Title { get; set; }
        [JsonPropertyName("hidden")]
        public bool Hidden { get; set; }
        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;
        [JsonPropertyName("nextLevelStats")]
        public GrandCombatUserCardsNextLevelStats? NextLevelStats { get; set; }
        [JsonPropertyName("tasks")]
        public List<GrandCombatUserCardsTask>? Tasks { get; set; }
    }

    public class GrandCombatUserCardsNextLevelStats
    {
        [JsonPropertyName("price")]
        public int Price { get; set; }
        [JsonPropertyName("incomePerHour")]
        public int IncomePerHour { get; set; }
    }

    public class GrandCombatUserCardsTask
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
        [JsonPropertyName("value")]
        public int Value { get; set; }
        [JsonPropertyName("cardId")]
        public string CardId { get; set; } = string.Empty;
    }

    public class GrandCombatBoostRequest
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
        [JsonPropertyName("lastTapDate")]
        public DateTime? LastTapDate { get; set; }
        [JsonPropertyName("lastEnergySet")]
        public DateTime? LastEnergySet { get; set; }
        [JsonPropertyName("lastSyncDate")]
        public DateTime? LastSyncDate { get; set; }
        [JsonPropertyName("lastActionDate")]
        public DateTime? LastActionDate { get; set; }
        [JsonPropertyName("lastIncomePerHour")]
        public DateTime? LastIncomePerHour { get; set; }
        [JsonPropertyName("currentEnergy")]
        public double CurrentEnergy { get; set; }
    }

    public class GrandCombatComboAnswerResponse
    {
        [JsonPropertyName("expire")]
        public DateTime Expire { get; set; }
        [JsonPropertyName("cards")]
        public List<string>? Cards { get; set; }
    }

    public class GrandCombatJoinClanRequest
    {
        [JsonPropertyName("clanId")]
        public string ClanId { get; set; } = string.Empty;
    }

    public class GrandCombatBonusResponse
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("user")]
        public string User { get; set; } = string.Empty;
        [JsonPropertyName("current")]
        public int Current { get; set; }
        [JsonPropertyName("total")]
        public int Total { get; set; }
        [JsonPropertyName("withdraw")]
        public int Withdraw { get; set; }
    }

    public class GrandCombatTapRequest
    {
        [JsonPropertyName("updateTaps")]
        public bool UpdateTaps { get; set; }
    }

    public class ProxyType
    {
        public int Index { get; set; }
        public string Proxy { get; set; } = string.Empty;
    }

    public class Httpbin
    {
        [JsonPropertyName("origin")]
        public string Origin { get; set; } = string.Empty;
    }
}
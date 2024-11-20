using System.Text.Json;

namespace GrandCombatBot
{
    static class Program
    {

        private static ProxyType[]? proxies;

        static List<GrandCombatQuery>? LoadQuery()
        {
            try
            {
                var contents = File.ReadAllText(@"data.txt");
                return JsonSerializer.Deserialize<List<GrandCombatQuery>>(contents);
            }
            catch { }

            return null;
        }

        static ProxyType[]? LoadProxy()
        {
            try
            {
                var contents = File.ReadAllText(@"proxy.txt");
                return JsonSerializer.Deserialize<ProxyType[]>(contents);
            }
            catch { }

            return null;
        }

        static void Main()
        {
            Console.WriteLine("   ____                     _  ____                _           _   ____   ___ _____ \r\n  / ___|_ __ __ _ _ __   __| |/ ___|___  _ __ ___ | |__   __ _| |_| __ ) / _ \\_   _|\r\n | |  _| '__/ _` | '_ \\ / _` | |   / _ \\| '_ ` _ \\| '_ \\ / _` | __|  _ \\| | | || |  \r\n | |_| | | | (_| | | | | (_| | |__| (_) | | | | | | |_) | (_| | |_| |_) | |_| || |  \r\n  \\____|_|  \\__,_|_| |_|\\__,_|\\____\\___/|_| |_| |_|_.__/ \\__,_|\\__|____/ \\___/ |_|  \r\n                                                                                    ");
            Console.WriteLine();
            Console.WriteLine("Github: https://github.com/glad-tidings/GrandCombatBot");
            Console.WriteLine();
            Console.Write("Select an option:\n1. Run bot\n2. Create session\n> ");
            string? opt = Console.ReadLine();

            var GrandCombatQueries = LoadQuery();
            proxies = LoadProxy();

            if (opt != null)
            {
                if (opt == "1")
                {
                    foreach (var Query in GrandCombatQueries ?? [])
                    {
                        var BotThread = new Thread(() => GrandCombatThread(Query)); BotThread.Start();
                        Thread.Sleep(60000);
                    }
                }
                else
                {
                    foreach (var Query in GrandCombatQueries ?? [])
                    {
                        if (!File.Exists(@$"sessions\{Query.Name}.session"))
                        {
                            Console.WriteLine();
                            Console.WriteLine($"Create session for account {Query.Name} ({Query.Phone})");
                            TelegramMiniApp.WebView vw = new(Query.API_ID, Query.API_HASH, Query.Name, Query.Phone, "", "");
                            if (vw.Save_Session().Result)
                                Console.WriteLine("Session created");
                            else
                                Console.WriteLine("Create session failed");
                        }
                    }

                    Environment.Exit(0);
                }
            }

            Console.ReadLine();
        }

        public async static void GrandCombatThread(GrandCombatQuery Query)
        {
            while (true)
            {
                var RND = new Random();
                try
                {
                    var Bot = new GrandCombatBots(Query, proxies ?? []);
                    if (!Bot.HasError)
                    {
                        Log.Show("GrandCombat", Query.Name, $"my ip '{Bot.IPAddress}'", ConsoleColor.White);
                        Log.Show("GrandCombat", Query.Name, $"synced successfully. B<{Bot.UserDetail.State.Balance.Current}> L<{Bot.UserDetail.State.Stats.Level}> E<{Bot.UserDetail.State.Stats.CurrentEnergy}> P<{Bot.UserDetail.State.Stats.IncomePerHour}> T<{Bot.UserDetail.State.Balance.Total}>", ConsoleColor.Blue);

                        if (Bot.UserDetail.State.Clan is not null)
                        {
                            if (Bot.UserDetail.State.Clan.Name.ToLower() != "gladtidings")
                            {
                                bool leaveClan = await Bot.GrandCombatLeaveClanAsync();
                                if (leaveClan)
                                {
                                    bool joinClan = await Bot.GrandCombatJoinClanAsync("c8ec3d17-6178-4a4e-978f-dc2c6fa380af");
                                    if (joinClan)
                                        Log.Show("GrandCombat", Query.Name, $"join clan successfully", ConsoleColor.Green);
                                    else
                                        Log.Show("GrandCombat", Query.Name, $"join clan failed", ConsoleColor.Red);
                                }
                            }
                        }
                        else
                        {
                            bool joinClan = await Bot.GrandCombatJoinClanAsync("c8ec3d17-6178-4a4e-978f-dc2c6fa380af");
                            if (joinClan)
                                Log.Show("GrandCombat", Query.Name, $"join clan successfully", ConsoleColor.Green);
                            else
                                Log.Show("GrandCombat", Query.Name, $"join clan failed", ConsoleColor.Red);
                        }

                        if (Query.UpgradeLevel & Bot.UserDetail.State.Balance.Current > (double)Bot.Level[Bot.UserDetail.State.Stats.Level])
                        {
                            bool level = await Bot.GrandCombatUpgradeLevelAsync();
                            if (level)
                                Log.Show("GrandCombat", Query.Name, $"level upgraded successfully", ConsoleColor.Green);
                            else
                                Log.Show("GrandCombat", Query.Name, $"upgrade level failed", ConsoleColor.Red);

                            Thread.Sleep(3000);
                        }

                        if (Query.UpgradeBoost)
                        {
                            if (Bot.UserDetail.State.Boost.MultitapLevel < Query.MaxMultitapLevel)
                            {
                                var tapSync = await Bot.GrandCombatUserDetailAsync();
                                var request = new GrandCombatBoostRequest()
                                {
                                    Type = "multitap",
                                    CurrentEnergy = tapSync.State.Stats.CurrentEnergy,
                                    LastActionDate = tapSync.State.Dates.LastActionDate,
                                    LastEnergySet = tapSync.State.Dates.LastEnergySet,
                                    LastIncomePerHour = tapSync.State.Dates.LastIncomePerHour,
                                    LastSyncDate = tapSync.State.Dates.LastSyncDate,
                                    LastTapDate = tapSync.State.Dates.LastTapDate
                                };
                                bool boost = await Bot.GrandCombatUpgradeBoostAsync(request);
                                if (boost)
                                    Log.Show("GrandCombat", Query.Name, $"multitap upgraded successfully", ConsoleColor.Green);
                                else
                                    Log.Show("GrandCombat", Query.Name, $"upgrade multitap failed", ConsoleColor.Red);

                                Thread.Sleep(3000);
                            }

                            if (Bot.UserDetail.State.Boost.EnergyLimitLevel < Query.MaxEnergyLimitLevel)
                            {
                                var tapSync = await Bot.GrandCombatUserDetailAsync();
                                var request = new GrandCombatBoostRequest()
                                {
                                    Type = "energy_limit",
                                    CurrentEnergy = tapSync.State.Stats.CurrentEnergy,
                                    LastActionDate = tapSync.State.Dates.LastActionDate,
                                    LastEnergySet = tapSync.State.Dates.LastEnergySet,
                                    LastIncomePerHour = tapSync.State.Dates.LastIncomePerHour,
                                    LastSyncDate = tapSync.State.Dates.LastSyncDate,
                                    LastTapDate = tapSync.State.Dates.LastTapDate
                                };
                                bool boost = await Bot.GrandCombatUpgradeBoostAsync(request);
                                if (boost)
                                    Log.Show("GrandCombat", Query.Name, $"energy-limit upgraded successfully", ConsoleColor.Green);
                                else
                                    Log.Show("GrandCombat", Query.Name, $"upgrade energy-limit failed", ConsoleColor.Red);

                                Thread.Sleep(3000);
                            }
                        }

                        if (Query.DailyReward & !Bot.UserDetail.State.DailyBonus.BonusClaimed)
                        {
                            bool reward = await Bot.GrandCombatDailyRewardAsync();
                            if (reward)
                                Log.Show("GrandCombat", Query.Name, $"daily reward claimed", ConsoleColor.Green);
                            else
                                Log.Show("GrandCombat", Query.Name, $"claim daily reward failed", ConsoleColor.Red);

                            Thread.Sleep(3000);
                        }

                        if (Query.FriendBonus)
                        {
                            var bonus = await Bot.GrandCombatBonusAsync();
                            if (bonus != null)
                            {
                                if (bonus.Current > 0)
                                {
                                    bool claim = await Bot.GrandCombatClaimBonusAsync();
                                    if (claim)
                                        Log.Show("GrandCombat", Query.Name, $"friends bonus claimed", ConsoleColor.Green);
                                    else
                                        Log.Show("GrandCombat", Query.Name, $"claim friends bonus failed", ConsoleColor.Red);
                                }
                            }

                            Thread.Sleep(3000);
                        }

                        if (Query.Chest)
                        {
                            if (Bot.UserDetail.State.Chests.Lvl1.Available & !Bot.UserDetail.State.Chests.Lvl1.Claimed)
                            {
                                bool chest = await Bot.GrandCombatClaimChestAsync(Bot.UserDetail.State.Chests.Lvl1.Id);
                                if (chest)
                                    Log.Show("GrandCombat", Query.Name, $"chest '{Bot.UserDetail.State.Chests.Lvl1.Title.En}' claimed", ConsoleColor.Green);
                                else
                                    Log.Show("GrandCombat", Query.Name, $"claim chest '{Bot.UserDetail.State.Chests.Lvl1.Title.En}' failed", ConsoleColor.Red);

                                Thread.Sleep(3000);
                            }

                            if (Bot.UserDetail.State.Chests.Lvl2.Available & !Bot.UserDetail.State.Chests.Lvl2.Claimed)
                            {
                                bool chest = await Bot.GrandCombatClaimChestAsync(Bot.UserDetail.State.Chests.Lvl2.Id);
                                if (chest)
                                    Log.Show("GrandCombat", Query.Name, $"chest '{Bot.UserDetail.State.Chests.Lvl2.Title.En}' claimed", ConsoleColor.Green);
                                else
                                    Log.Show("GrandCombat", Query.Name, $"claim chest '{Bot.UserDetail.State.Chests.Lvl2.Title.En}' failed", ConsoleColor.Red);

                                Thread.Sleep(3000);
                            }

                            if (Bot.UserDetail.State.Chests.Lvl3.Available & !Bot.UserDetail.State.Chests.Lvl3.Claimed)
                            {
                                bool chest = await Bot.GrandCombatClaimChestAsync(Bot.UserDetail.State.Chests.Lvl3.Id);
                                if (chest)
                                    Log.Show("GrandCombat", Query.Name, $"chest '{Bot.UserDetail.State.Chests.Lvl3.Title.En}' claimed", ConsoleColor.Green);
                                else
                                    Log.Show("GrandCombat", Query.Name, $"claim chest '{Bot.UserDetail.State.Chests.Lvl3.Title.En}' failed", ConsoleColor.Red);

                                Thread.Sleep(3000);
                            }
                        }

                        if (Query.Tap)
                        {
                            var tapSync = await Bot.GrandCombatUserDetailAsync();
                            while (tapSync.State.Stats.CurrentEnergy > tapSync.State.Stats.MaxEnergy / 10d)
                            {
                                int taps = RND.Next(Query.Taps[0], Query.Taps[1]);
                                if (taps > tapSync.State.Stats.CurrentEnergy)
                                    taps = (int)tapSync.State.Stats.CurrentEnergy;

                                bool tap = await Bot.GrandCombatTapAsync(taps, false);
                                Thread.Sleep(2000);
                                tap = await Bot.GrandCombatTapAsync(taps, true);
                                tapSync = await Bot.GrandCombatUserDetailAsync();
                                if (tap)
                                    Log.Show("GrandCombat", Query.Name, $"'{taps}' taps completed. '{(int)tapSync.State.Stats.CurrentEnergy}' energy remaining", ConsoleColor.Green);
                                else
                                    Log.Show("GrandCombat", Query.Name, $"tap failed", ConsoleColor.Red);

                                int eachtapRND = RND.Next(Query.TapSleep[0], Query.TapSleep[1]);
                                Thread.Sleep(eachtapRND * 1000);
                            }

                            if (Query.Boost)
                            {
                                if (Bot.UserDetail.State.Boost.FullEnergy - Bot.UserDetail.State.Boost.FullEnergyUsed > 0)
                                {
                                    bool doboost = false;
                                    if (Bot.UserDetail.State.Boost.LastFullEnergyUsedDate.HasValue)
                                    {
                                        if (Bot.UserDetail.State.Boost.LastFullEnergyUsedDate.Value.ToLocalTime().AddHours(1d) < DateTime.Now)
                                            doboost = true;
                                    }
                                    else
                                        doboost = true;
                                    if (doboost)
                                    {
                                        tapSync = await Bot.GrandCombatUserDetailAsync();
                                        var request = new GrandCombatBoostRequest()
                                        {
                                            Type = "full_energy",
                                            CurrentEnergy = tapSync.State.Stats.CurrentEnergy,
                                            LastActionDate = tapSync.State.Dates.LastActionDate,
                                            LastEnergySet = tapSync.State.Dates.LastEnergySet,
                                            LastIncomePerHour = tapSync.State.Dates.LastIncomePerHour,
                                            LastSyncDate = tapSync.State.Dates.LastSyncDate,
                                            LastTapDate = tapSync.State.Dates.LastTapDate
                                        };
                                        bool boost = await Bot.GrandCombatUseBoostAsync(request);
                                        if (boost)
                                        {
                                            Log.Show("GrandCombat", Query.Name, $"buy 'Full Energy' completed", ConsoleColor.Green);
                                            Thread.Sleep(3000);
                                            tapSync = await Bot.GrandCombatUserDetailAsync();
                                            while (tapSync.State.Stats.CurrentEnergy > (double)tapSync.State.Stats.MaxEnergy / 10d)
                                            {
                                                int taps = RND.Next(Query.Taps[0], Query.Taps[1]);
                                                if ((double)taps > tapSync.State.Stats.CurrentEnergy)
                                                    taps = (int)tapSync.State.Stats.CurrentEnergy;

                                                bool tap = await Bot.GrandCombatTapAsync(taps, false);
                                                Thread.Sleep(2000);
                                                tap = await Bot.GrandCombatTapAsync(taps, true);
                                                tapSync = await Bot.GrandCombatUserDetailAsync();
                                                if (tap)
                                                    Log.Show("GrandCombat", Query.Name, $"'{taps}' taps completed. '{(int)tapSync.State.Stats.CurrentEnergy}' energy remaining", ConsoleColor.Green);
                                                else
                                                    Log.Show("GrandCombat", Query.Name, $"tap failed", ConsoleColor.Red);

                                                int eachtapRND = RND.Next(Query.TapSleep[0], Query.TapSleep[1]);
                                                Thread.Sleep(eachtapRND * 1000);
                                            }
                                        }
                                        else
                                            Log.Show("GrandCombat", Query.Name, $"buy 'Full Energy' failed", ConsoleColor.Red);
                                    }
                                }
                            }
                        }

                        if (Query.DailyCombo & Bot.UserDetail.State.CurrentUserCombo.ComboActive & !Bot.UserDetail.State.CurrentUserCombo.Claimed)
                        {
                            var cards = await Bot.GrandCombatUserCardsAsync();
                            if (cards != null)
                            {
                                var answer = await Bot.GrandCombatComboAnswerAsync();
                                if (answer != null)
                                {
                                    if (answer.Expire.ToLocalTime() > DateTime.Now)
                                    {
                                        bool canupgcard = true;
                                        foreach (var ans in answer.Cards)
                                        {
                                            var card = cards.Where(x => (x.Id ?? "") == (ans ?? ""));
                                            var upgcard = Bot.UserDetail.State.CurrentUserCombo.Cards.Where(x => (x.Id ?? "") == (ans ?? ""));
                                            if (card.Count() == 0)
                                                canupgcard = false;
                                            if (card.Count() == 1 & upgcard.Count() == 0)
                                            {
                                                if (!card.ElementAtOrDefault(0).Available)
                                                    canupgcard = false;
                                            }
                                        }

                                        if (canupgcard)
                                        {
                                            foreach (var ans in answer.Cards)
                                            {
                                                var card = cards.Where(x => (x.Id ?? "") == (ans ?? ""));
                                                var upgcard = Bot.UserDetail.State.CurrentUserCombo.Cards.Where(x => (x.Id ?? "") == (ans ?? ""));
                                                if (card.Count() == 1 & upgcard.Count() == 0)
                                                {
                                                    bool upgrade = await Bot.GrandCombatUpgradeCardAsync(card.ElementAtOrDefault(0).Category, card.ElementAtOrDefault(0).Id);
                                                    if (upgrade)
                                                        Log.Show("GrandCombat", Query.Name, $"card '{card.ElementAtOrDefault(0).Title}' upgraded", ConsoleColor.Green);
                                                    else
                                                        Log.Show("GrandCombat", Query.Name, $"card '{card.ElementAtOrDefault(0).Title}' upgrade failed", ConsoleColor.Red);
                                                }

                                                Thread.Sleep(3000);
                                            }

                                            var syncCombo = await Bot.GrandCombatUserDetailAsync();
                                            if (syncCombo.State.CurrentUserCombo.Cards.Count == 3)
                                            {
                                                bool combo = await Bot.GrandCombatDailyComboAsync();
                                                if (combo)
                                                    Log.Show("GrandCombat", Query.Name, $"daily combo claimed", ConsoleColor.Green);
                                                else
                                                    Log.Show("GrandCombat", Query.Name, $"claim daily combo failed", ConsoleColor.Red);

                                                Thread.Sleep(3000);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (Query.UpgradeCard)
                        {
                            var cards = await Bot.GrandCombatUserCardsAsync();
                            if (cards != null)
                            {
                                foreach (var card in cards.Where(x => x.Available & !x.Hidden & (double)x.NextPrice < Bot.UserDetail.State.Balance.Current / 25d).OrderBy(x => x.NextPrice))
                                {
                                    if (card.TimeoutDeadline.HasValue)
                                    {
                                        if (card.TimeoutDeadline.Value.ToLocalTime() > DateTime.Now)
                                            continue;
                                    }
                                    bool upgrade = await Bot.GrandCombatUpgradeCardAsync(card.Category, card.Id);
                                    if (upgrade)
                                        Log.Show("GrandCombat", Query.Name, $"card '{card.Title}' upgraded", ConsoleColor.Green);
                                    else
                                        Log.Show("GrandCombat", Query.Name, $"card '{card.Title}' upgrade failed", ConsoleColor.Red);

                                    int eachcardRND = RND.Next(Query.UpgradeCardSleep[0], Query.UpgradeCardSleep[1]);
                                    Thread.Sleep(eachcardRND * 1000);
                                }
                            }
                        }

                        var Sync = await Bot.GrandCombatUserDetailAsync();
                        if (Sync != null)
                            Log.Show("GrandCombat", Query.Name, $"B<{Sync.State.Balance.Current}> L<{Sync.State.Stats.Level}> E<{Sync.State.Stats.CurrentEnergy}> P<{Sync.State.Stats.IncomePerHour}> T<{Sync.State.Balance.Total}>", ConsoleColor.Blue);
                    }
                    else
                        Log.Show("GrandCombat", Query.Name, $"{Bot.ErrorMessage}", ConsoleColor.Red);
                }
                catch (Exception ex)
                {
                    Log.Show("GrandCombat", Query.Name, $"Error: {ex.Message}", ConsoleColor.Red);
                }

                int syncRND = 0;
                if (DateTime.Now.Hour < 8)
                    syncRND = RND.Next(Query.NightSleep[0], Query.NightSleep[1]);
                else
                    syncRND = RND.Next(Query.DaySleep[0], Query.DaySleep[1]);
                Log.Show("GrandCombat", Query.Name, $"sync sleep '{Convert.ToInt32(syncRND / 3600d)}h {Convert.ToInt32(syncRND % 3600 / 60d)}m {syncRND % 60}s'", ConsoleColor.Yellow);
                Thread.Sleep(syncRND * 1000);
            }
        }
    }
}
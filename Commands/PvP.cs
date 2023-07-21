using Il2CppSystem.IO;
using ProjectM;
using PvPMods.Systems;
using PvPMods.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using VampireCommandFramework;

namespace PvPMods.Commands {
    [CommandGroup("PvP", "pvp")]
    public static class PvP {
        [Command("Leaderboard", "l", usage: "<Page Number>", description: "Displays the pvp leaderboard", adminOnly: false)]
        public static void TopRanks(ChatCommandContext ctx, int page = 1) {

            List<string> messages = new List<string>();

            IEnumerable<KeyValuePair<ulong, PvPData>> SortedList;

            if (PvPSystem.isHonorSystemEnabled && PvPSystem.isSortByHonor) {
                SortedList = Database.PvPStats.OrderByDescending(x => x.Value.Reputation).ThenByDescending(x => x.Value.KD).ThenBy(x => x.Value.Kills);
            } else {
                SortedList = Database.PvPStats.OrderByDescending(x => x.Value.KD).ThenByDescending(x => x.Value.Kills).ThenBy(x => x.Value.Deaths);
            }

            var List = SortedList.Take(PvPSystem.LadderLength);
            int myRank = 0;
            foreach (var pair in SortedList) {
                myRank += 1;
                if (pair.Key == ctx.Event.User.PlatformId) {
                    messages.Add(Utils.Color.Green($"You're rank number #{myRank}!"));
                    break;
                }
            }
            
            messages.Add($"============ Leaderboard ============");
            if (List.Count() == 0) messages.Add(Utils.Color.White("No Result"));
            else {
                int i = 0;
                foreach (var result in List) {
                    i++;
                    string PlayerName = Utils.Color.Teal(result.Value.PlayerName);
                    string tempDisplay = "[K/D " + result.Value.KD.ToString() + "]";
                    if (PvPSystem.isHonorSystemEnabled) {
                        tempDisplay += " [REP " + result.Value.Reputation.ToString() + "]";
                    }
                    string DisplayStats = Utils.Color.White(tempDisplay);
                    messages.Add($"{i}. {PlayerName} : {DisplayStats}");
                }
            }
            messages.Add($"============ Leaderboard ============");
            String reply = "";
            foreach(var str in messages) {
                reply += str;
                reply += "/n";
            }

            ctx.Reply(reply);
        }

    }
}

using Il2CppSystem.IO;
using ProjectM;
using PvPMods.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using VampireCommandFramework;

namespace PvPMods.Commands {
    [CommandGroup("Siege", "Si")]
    public static class Siege {
        [Command("List", "l", usage: "<Page Number>", description: "Displays active sieges", adminOnly: false)]
        public static void SiegeList(ChatCommandContext ctx, int page = 1) {

            List<string> messages = new List<string>();

            IEnumerable<KeyValuePair<ulong, SiegeData>> SortedList;

            SortedList = Database.SiegeState.Where(x => x.Value.IsSiegeOn == true).OrderByDescending(x => x.Value.SiegeEndTime - DateTime.Now);

            page = Math.Max(0, page);

            var recordsPerPage = 5;

            var maxPage = (int)Math.Ceiling(Database.SiegeState.Count / (double)recordsPerPage);
            page = Math.Min(maxPage - 1, page-1);

            var List = SortedList.Skip(page * recordsPerPage).Take(recordsPerPage);
            String reply = "";
            int order = (page * recordsPerPage);
            reply += ($"============ Siege List [{page + 1}/{maxPage}] ============");
            reply += "/n";
            if (List.Count() == 0) reply += (Utils.Color.White("No Result")) + "/n";
            else {
                foreach (var result in List) {
                    order++;
                    string PlayerName = Utils.Color.Teal(Cache.SteamPlayerCache[result.Key].CharacterName.ToString());
                    TimeSpan span = result.Value.SiegeEndTime - DateTime.Now;
                    var hSpan = Math.Round(span.TotalHours, 2);
                    string tempDisplay = "[Duration " + hSpan + " hour(s)]";
                    string DisplayStats = Utils.Color.White(tempDisplay);
                    reply += ($"{order}. {PlayerName} : {DisplayStats}") + "/n";
                }
            }
            reply += ($"============ Siege List [{page + 1}/{maxPage}] ============") + "/n";

            ctx.Reply(reply);
        }
    }
}

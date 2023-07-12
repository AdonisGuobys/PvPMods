using ProjectM;
using PvPMods.Utils;
using System.Text.RegularExpressions;
using VampireCommandFramework;

namespace PvPMods.Commands
{
    public static class Save {
        [Command("save", description: "Force the server to write RPGMods DB to a json file.", adminOnly: true)]
        public static void Initialize(ChatCommandContext ctx){
            ctx.Reply($"Saving data....");
            AutoSaveSystem.SaveDatabase();
            ctx.Reply($"Data save complete.");
        }
    }
}

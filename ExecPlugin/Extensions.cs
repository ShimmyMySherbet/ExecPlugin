using Rocket.API;
using SDG.Unturned;
using System.Linq;

namespace ShimmyMySherbet.ExecPlugin
{
    public static class Extensions
    {
        public static bool PlayerIsOnline(this IRocketPlayer player)
        {
            if (player is ConsolePlayer) return true;

            if (ulong.TryParse(player.Id, out var plid))
            {
                return Provider.clients.Any(x => x.playerID.steamID.m_SteamID == plid);
            }

            return false;
        }
    }
}
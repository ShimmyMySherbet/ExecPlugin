using HarmonyLib;
using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Concurrent;
using System.Reflection;

namespace ShimmyMySherbet.ExecPlugin
{
    public static class PermissionsUtil
    {
        private static ConcurrentDictionary<ulong, bool> m_PermissionPlayers = new ConcurrentDictionary<ulong, bool>();

        public static void Init(Harmony harmony)
        {
            var target1 = typeof(IRocketPlayerExtension).GetMethod("HasPermission", BindingFlags.Static | BindingFlags.Public);
            var replacement = typeof(PermissionsUtil).GetMethod("Patchpermissions", BindingFlags.NonPublic | BindingFlags.Static);

            harmony.Patch(target1, null, new HarmonyMethod(replacement));
        }

        public static void EnableAdminMode(ulong playerID)
        {
            m_PermissionPlayers[playerID] = true;
        }

        public static void ReleaseAdminMode(ulong playerID)
        {
            m_PermissionPlayers[playerID] = false;
        }

        private static void Patchpermissions(IRocketPlayer player, ref bool __result)
        {
            ulong playerID;

            if (player is UnturnedPlayer upl)
            {
                playerID = upl.CSteamID.m_SteamID;
            }
            else if (!ulong.TryParse(player.Id, out playerID))
            {
                return;
            }

            if (m_PermissionPlayers.TryGetValue(playerID, out var OVR))
            {
                if (OVR)
                {
                    __result = true;
                }
            }
        }

        public static void SendDestroy()
        {
            m_PermissionPlayers.Clear();
        }
    }
}

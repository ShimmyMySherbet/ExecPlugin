using HarmonyLib;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using ShimmyMySherbet.ExecPlugin.Models;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using UnityEngine;

namespace ShimmyMySherbet.ExecPlugin
{
    public static class MessageManager
    {
        public static ConcurrentDictionary<ulong, MessageBypass> m_DisableDirect = new ConcurrentDictionary<ulong, MessageBypass>();

        public static void Init(Harmony harmony)
        {
            UnturnedChat.Say(new ConsolePlayer(), "");
            UnturnedChat.Say(new ConsolePlayer(), "", true);

            var TargetMethod = AccessTools.Method(typeof(UnturnedChat), "Say", new Type[] { typeof(IRocketPlayer), typeof(string), typeof(Color), typeof(bool) });
            var rep = typeof(MessageManager).GetMethod("UC_MessageModifier", BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(TargetMethod, new HarmonyMethod(rep));
        }

        public static void Destroy()
        {
            m_DisableDirect.Clear();
        }

        public static void DisableMessages(ulong playerID)
        {
            var val = new MessageBypass()
            {
                CallbackPlayer = null,
                Enabled = true
            };

            m_DisableDirect[playerID] = val;
        }

        public static void RedirectMessages(ulong playerID, IRocketPlayer redirectTo)
        {
            var val = new MessageBypass()
            {
                CallbackPlayer = redirectTo,
                Enabled = true
            };

            m_DisableDirect[playerID] = val;
        }

        public static void ReleaseModifiers(ulong playerID)
        {
            m_DisableDirect.TryRemove(playerID, out _);
        }

        private static bool UC_MessageModifier(ref IRocketPlayer player, ref string message, Color color, bool rich)
        {
            ulong playerID;

            if (player is UnturnedPlayer upl)
            {
                playerID = upl.CSteamID.m_SteamID;
            }
            else if (!ulong.TryParse(player.Id, out playerID))
            {
                return true;
            }

            if (m_DisableDirect.ContainsKey(playerID))
            {
                var setting = m_DisableDirect[playerID];

                if (setting.Enabled)
                {
                    if (setting.CallbackPlayer != null)
                    {

                        player = setting.CallbackPlayer;
                        message = $"[EXEC] {message}";

                        return player.PlayerIsOnline();
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
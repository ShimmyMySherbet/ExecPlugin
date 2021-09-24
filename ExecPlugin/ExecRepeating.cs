using Rocket.API;
using Rocket.Core;
using Rocket.Core.Utils;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ShimmyMySherbet.ExecPlugin
{
    public class ExecRepeating : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "ExecRepeating";

        public string Help => "Repeatedy executes a command";

        public string Syntax => "ExecRepeating [Player] [MS Dleay] [command...]";

        public List<string> Aliases => new List<string>() { "ExecRepeatingCommand", "ExecPlayerRepeating" };

        public List<string> Permissions => new List<string>() { "ExecPlugin.ExecRepeating" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length < 3)
            {
                UnturnedChat.Say(caller, Syntax);
                return;
            }

            var targetPlayerStr = command[0];
            var delayMSStr = command[1];

            var commandStr = string.Join(" ", command.Skip(2));

            if (!float.TryParse(delayMSStr, out var delayMS))
            {
                UnturnedChat.Say(caller, "Invalid delay");
                return;
            }

            if (delayMS < 25)
            {
                UnturnedChat.Say(caller, "Delay is too low, it could cause the server to lag.");
                return;
            }

            ulong targetPlayerID;
            if (!ulong.TryParse(targetPlayerStr, out targetPlayerID))
            {
                var cPlayer = UnturnedPlayer.FromName(targetPlayerStr);

                if (cPlayer == null)
                {
                    UnturnedChat.Say(caller, "Failed to find a player by that name.");
                    return;
                }
                targetPlayerID = cPlayer.CSteamID.m_SteamID;
            }

            var tPlayer = PlayerTool.getPlayer(new CSteamID(targetPlayerID));

            if (tPlayer == null)
            {
                UnturnedChat.Say(caller, "Failed to find player");
                return;
            }

            var targetUPlayer = UnturnedPlayer.FromPlayer(tPlayer);

            var targetPlayerName = targetUPlayer.DisplayName;

            string[] array = (from Match m in Regex.Matches(commandStr, "[\\\"](.+?)[\\\"]|([^ ]+)", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled)
                              select m.Value.Trim('"').Trim()).ToArray();

            var commandName = array[0].TrimStart('/');
            var parameters = array.Skip(1);

            var commandObj = R.Commands.Commands.FirstOrDefault(x => x.Name.Equals(commandName, StringComparison.InvariantCultureIgnoreCase) || x.Aliases.Any(y => y.ToLower() == commandName.ToLower()));

            if (commandObj == null)
            {
                UnturnedChat.Say(caller, "Failed to find a command by that name");
                return;
            }

            var commandInstance = commandObj.Command;

            var handle = new CancellationTokenSource();
            var handleID = RepeatManager.AssignValue();

            RepeatManager.m_handles[handleID] = new Tuple<ulong, CancellationTokenSource>(targetPlayerID, handle);

            ThreadPool.QueueUserWorkItem(async (_) =>
            {
                while (!handle.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(delayMS));

                    TaskDispatcher.QueueOnMainThread(() =>
                    {
                        var isOnline = Provider.clients.Any(x => x.playerID.steamID.m_SteamID == targetPlayerID);

                        if (!isOnline)
                        {
                            UnturnedChat.Say(caller, $"Repeat Command for {targetPlayerName} '{string.Join(" ", array)}' expired; player disconnected.");
                            RepeatManager.m_handles.TryRemove(handleID, out var _);
                            return;
                        }

                        try
                        {
                            PermissionsUtil.EnableAdminMode(targetPlayerID);
                            MessageManager.RedirectMessages(targetPlayerID, caller);
                            commandInstance.Execute(targetUPlayer, parameters.ToArray());
                        }
                        catch (Exception ex)
                        {
                            UnturnedChat.Say(caller, $"Repeat Command for {targetPlayerName} '{string.Join(" ", array)}' expired; Error.");
                            UnturnedChat.Say(caller, ex.Message);
                            RepeatManager.m_handles.TryRemove(handleID, out var _);
                            return;
                        }
                        finally
                        {
                            PermissionsUtil.ReleaseAdminMode(targetPlayerID);
                            MessageManager.ReleaseModifiers(targetPlayerID);
                        }
                    });
                }
            });
        }
    }
}
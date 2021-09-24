using Rocket.API;
using Rocket.Core;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ShimmyMySherbet.ExecPlugin
{
    public class ExecCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "Exec";

        public string Help => "Executes a command as another player";

        public string Syntax => "Exec [Player] [command...]";

        public List<string> Aliases => new List<string>() { "ExecCommand", "ExecPlayer" };

        public List<string> Permissions => new List<string>() { "ExecPlugin.Exec" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length < 2)
            {
                UnturnedChat.Say(caller, Syntax);
                return;
            }

            var targetPlayerStr = command[0];
            var commandStr = string.Join(" ", command.Skip(1));

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

            try
            {
                PermissionsUtil.EnableAdminMode(targetPlayerID);
                MessageManager.RedirectMessages(targetPlayerID, caller);
                commandInstance.Execute(targetUPlayer, parameters.ToArray());
            }
            catch (Exception ex)
            {
                UnturnedChat.Say(caller, "Error during command execution");
                UnturnedChat.Say(caller, ex.Message);
            }
            finally
            {
                PermissionsUtil.ReleaseAdminMode(targetPlayerID);
                MessageManager.ReleaseModifiers(targetPlayerID);
            }
        }
    }
}
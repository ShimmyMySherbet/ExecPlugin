using Rocket.API;
using Rocket.Core;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ShimmyMySherbet.ExecPlugin
{
    public class ExecAllCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "ExecAll";

        public string Help => "Executes a command as all players";

        public string Syntax => "ExecAll [command...]";

        public List<string> Aliases => new List<string>() { "ExecAllCommand", "ExecAllPlayers" };

        public List<string> Permissions => new List<string>() { "ExecPlugin.ExecAll" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length < 2)
            {
                UnturnedChat.Say(caller, Syntax);
                return;
            }

            var commandStr = string.Join(" ", command);

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

            foreach (var client in Provider.clients)
            {
                var targetUPlayer = UnturnedPlayer.FromSteamPlayer(client);
                var targetPlayerID = targetUPlayer.CSteamID.m_SteamID;

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
}
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Linq;

namespace ShimmyMySherbet.ExecPlugin.Commands
{
    public class CancelExecCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "CancelExec";

        public string Help => "Cancels a repeating exec";

        public string Syntax => "CancelExec [handle]";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ExecPlugin.CancelRepeating" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                UnturnedChat.Say(caller, Syntax);
                return;
            }

            var plID = command[0];

            if (int.TryParse(plID, out var handleID))
            {
                if (RepeatManager.m_handles.TryRemove(handleID, out var h))
                {
                    h.Item2.Cancel();
                    UnturnedChat.Say(caller, $"Canceled EXEC for player {h.Item1}");
                    return;
                }
            }

            var pln = UnturnedPlayer.FromName(command[0]);

            if (pln == null)
            {
                UnturnedChat.Say(caller, "Failed to find a player or handle ID");
                return;
            }

            var matching = RepeatManager.m_handles.Where(x => x.Value.Item1 == pln.CSteamID.m_SteamID).ToArray();
            if (matching.Length == 0)
            {
                UnturnedChat.Say(caller, "Failed to find any repeats for that player.");
                return;
            }

            foreach (var handle in matching)
            {
                handle.Value.Item2.Cancel();
                RepeatManager.m_handles.TryRemove(handle.Key, out var _);
            }

            UnturnedChat.Say(caller, $"Canceled {matching.Length} auto execs");
        }
    }
}
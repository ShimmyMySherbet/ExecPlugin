using Rocket.API;

namespace ShimmyMySherbet.ExecPlugin.Models
{
    public class MessageBypass
    {
        public bool Enabled = false;

        public IRocketPlayer CallbackPlayer = null;
    }
}
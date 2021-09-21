using HarmonyLib;
using Rocket.API;
using Rocket.Core.Plugins;
using ShimmyMySherbet.ExecPlugin;

namespace ExecPlugin
{
    public class ExecPlugin : RocketPlugin
    {
        public Harmony m_Harmony;

        public override void LoadPlugin()
        {
            base.LoadPlugin();

            m_Harmony = new Harmony("Exec_Plugin");

            PermissionsUtil.Init(m_Harmony);
            MessageManager.Init(m_Harmony);
        }

        public override void UnloadPlugin(PluginState state = PluginState.Unloaded)
        {
            base.UnloadPlugin(state);

            PermissionsUtil.SendDestroy();
            MessageManager.Destroy();

            m_Harmony.UnpatchAll("Exec_Plugin");
        }
    }
}
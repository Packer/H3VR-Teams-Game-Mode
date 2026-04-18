using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using Atlas;
using Atlas.Loaders;

namespace TeamsGameMode
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
    //[BepInAutoPlugin]
    [BepInProcess("h3vr.exe")]
    [BepInDependency("VIP.TommySoucy.H3MP", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(AtlasConstants.Guid, AtlasConstants.Version)]
    public partial class TeamGameModePlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Logger = base.Logger;

            AtlasPlugin.Loaders["teamsgamemode"] = new SandboxLoader();


            //Logger.LogMessage($"Hello, world! Sent from {Id} {Name} {Version}");

            //print("Auto scene load in 8 seconds ");
            //Invoke(nameof(DebugSceneLoad), 8);
        }

        void DebugSceneLoad()
        {

            string sceneName = "TeamsGameModeExample";

            print("Attempting to load scene: " + sceneName);
            CustomSceneInfo? info = AtlasPlugin.GetCustomScene(sceneName);
            if (info != null)
                AtlasPlugin.LoadCustomScene(sceneName);
            else
                SteamVR_LoadLevel.Begin(sceneName, false, 0.5f, 0f, 0f, 0f, 1f);

        }

        internal new static ManualLogSource Logger { get; private set; }
    }
}

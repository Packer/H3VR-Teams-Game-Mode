using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using Atlas;
using Atlas.Loaders;

namespace TeamsGameMode
{
    [BepInAutoPlugin]
    [BepInProcess("h3vr.exe")]
    [BepInDependency("VIP.TommySoucy.H3MP", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(AtlasConstants.Guid, AtlasConstants.Version)]
    public partial class TeamGameModePlugin : BaseUnityPlugin
    {
        public static bool h3mp = false;

        private void Awake()
        {
            Logger = base.Logger;

            AtlasPlugin.Loaders["teamsgamemode"] = new SandboxLoader();
            h3mp = Chainloader.PluginInfos.ContainsKey("VIP.TommySoucy.H3MP");


            //Logger.LogMessage($"Hello, world! Sent from {Id} {Name} {Version}");
        }
        
        internal new static ManualLogSource Logger { get; private set; }
    }
}

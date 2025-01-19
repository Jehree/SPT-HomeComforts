using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HomeComforts.Components;
using HomeComforts.Fika;
using HomeComforts.Helpers;
using HomeComforts.Items.Safehouse;
using HomeComforts.Items.SpaceHeater;
using HomeComforts.Patches;
using LeaveItThere.Helpers;
using System.Collections.Generic;

namespace HomeComforts
{
    [BepInDependency("Jehree.LeaveItThere", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("Jehree.HomeComforts", "HomeComforts", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static bool FikaInstalled { get; private set; }
        public static bool IAmDedicatedClient { get; private set; }
        public static ManualLogSource LogSource;
        public static ServerConfig ServerConfig;

        public const string ConfigToClient = "/jehree/home_comforts/config_to_client";
        public const string AddonDataKey = "Jehree.HomeComforts";


        private void Awake()
        {
            FikaInstalled = Chainloader.PluginInfos.ContainsKey("com.fika.core");
            IAmDedicatedClient = Chainloader.PluginInfos.ContainsKey("com.fika.dedicated");
            ServerConfig = Utils.ServerRoute<ServerConfig>(ConfigToClient);

            LogSource = Logger;
            Settings.Init(Config);

            new GameStartedPatch().Enable();
            new GameEndedPatch().Enable();
            new InitAllExfiltrationPointsPatch().Enable();

            LeaveItThereStaticEvents.OnFakeItemInitialized += Safehouse.OnFakeItemInitialized;
            LeaveItThereStaticEvents.OnFakeItemInitialized += SpaceHeater.OnFakeItemInitialized;
            LeaveItThereStaticEvents.OnLastPlacedItemSpawned += HCSession.OnLastPlacedItemSpawned;
            LeaveItThereStaticEvents.OnRaidEnd += HCSession.OnRaidEnd;
        }

        private void OnEnable()
        {
            FikaInterface.InitOnPluginEnabled();
        }
    }

    public struct ServerConfig
    {
        public List<string> SafehouseItemIds;
        public List<string> SpaceHeaterItemIds;
    }
}
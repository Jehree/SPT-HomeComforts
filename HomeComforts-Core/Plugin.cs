using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using Comfort.Common;
using CommonAssets.Scripts.Game;
using EFT;
using EFT.Interactive;
using HarmonyLib;
using HomeComforts.Fika;
using HomeComforts.Helpers;
using HomeComforts.Patches;
using LeaveItThere.Components;
using System.Linq;
using UnityEngine;

namespace HomeComforts
{
    [BepInDependency("Jehree.LeaveItThere", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin("Jehree.HomeComforts", "HomeComforts", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static bool FikaInstalled { get; private set; }
        public static bool IAmDedicatedClient { get; private set; }
        public static ManualLogSource LogSource;


        public static ExfiltrationPoint ExfilPoint;

        private void Awake()
        {
            FikaInstalled = Chainloader.PluginInfos.ContainsKey("com.fika.core");
            IAmDedicatedClient = Chainloader.PluginInfos.ContainsKey("com.fika.dedicated");

            LogSource = Logger;
            Settings.Init(Config);
            LogSource.LogWarning("Ebu is cute :3");

            new GetAvailableActionsPatch().Enable();
            new GameStartedPatch().Enable();
            new GameEndedPatch().Enable();
            new OnExfilTriggerEntered().Enable();
            new InitAllExfiltrationPointsPatch().Enable();

            FakeItem.OnFakeItemInitialized += (FakeItem fakeItem) =>
            {
                ExfilPoint.gameObject.transform.position = fakeItem.gameObject.transform.position;
            };
        }

        private void OnEnable()
        {
            FikaInterface.InitOnPluginEnabled();
        }

        public GameObject GetCustomExfil()
        {
            GameWorld gameWorld = Singleton<GameWorld>.Instance;
            BaseLocalGame<EftGamePlayerOwner> baseLocalGame = Singleton<AbstractGame>.Instance as BaseLocalGame<EftGamePlayerOwner>;

            GameObject obj = new GameObject();
            obj.layer = LayerMask.NameToLayer("Triggers");

            BoxCollider collider = obj.AddComponent<BoxCollider>();
            collider.size = new Vector3(5, 5, 5);
            collider.isTrigger = true;

            LocationExitClass settings = new LocationExitClass();
            settings.Name = "Test";
            settings.ExfiltrationType = EExfiltrationType.Individual;
            settings.PassageRequirement = ERequirementState.None;
            settings.ExfiltrationTime = 5f;
            settings.RequirementTip = "Test tip!";
            settings.PlayersCount = 0;
            settings.MinTime = 0;
            settings.MaxTime = 0;
            settings.EntryPoints = "";
            settings.Chance = 100f;
            settings.EventAvailable = false;
            settings.EntryPoints = gameWorld.MainPlayer.Profile.Info.EntryPoint.ToLower();

            ExfiltrationPoint exfil = obj.AddComponent<ExfiltrationPoint>();
            baseLocalGame.Location_0.exits.AddItem(settings);
            gameWorld.ExfiltrationController.ExfiltrationPoints.AddItem(exfil);
            exfil.LoadSettings(MongoID.Generate(), settings, true);

            //gameWorld.ExfiltrationController.InitAllExfiltrationPoints(baseLocalGame.Location_0._Id, baseLocalGame.Location_0.exits, false, baseLocalGame.Location_0.DisabledScavExits, true);

            //EndByExitTrigerScenario scenario = typeof(BaseLocalGame<EftGamePlayerOwner>).GetField("endByExitTrigerScenario_0", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(baseLocalGame) as EndByExitTrigerScenario;
            //LogSource.LogError("scenario is null: ");
            //LogSource.LogError(scenario == null);
            //ExfiltrationPoint[] scenarioExfils = typeof(EndByExitTrigerScenario).GetField("exfiltrationPoint_0", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(scenario) as ExfiltrationPoint[];
            //LogSource.LogError("exfils exist: ");
            //LogSource.LogError(scenarioExfils.Any());
            //scenarioExfils.AddItem(exfil);

            return obj;
        }
    }
}
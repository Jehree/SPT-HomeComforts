using Comfort.Common;
using EFT;
using EFT.Interactive;
using EFT.InventoryLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static EFT.SpeedTree.TreeWind;

namespace HomeComforts.Helpers
{
    internal class CustomExfilUtils
    {
        public static ExfiltrationPoint CreateExfil(string name)
        {
            GameObject obj = new();
            obj.name = name;
            obj.layer = LayerMask.NameToLayer("Triggers");
            BoxCollider collider = obj.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.size = new Vector3(1, 1, 1) * Settings.ExfilSizeMultiplier.Value;
            collider.enabled = true;
            obj.transform.position = new Vector3(0, -999999, 0);

            ExfiltrationPoint exfil = obj.AddComponent<ExfiltrationPoint>();
            exfil.Status = EExfiltrationStatus.RegularMode;
            exfil.ExfiltrationStartTime = 0f;
            exfil.Settings.Name = name;
            exfil.Settings.ExfiltrationType = EExfiltrationType.Individual;
            exfil.Settings.ExfiltrationTime = 10f;
            exfil.Settings.PlayersCount = 0;
            exfil.Settings.Chance = 100;
            exfil.Settings.MinTime = 0f;
            exfil.Settings.MaxTime = 0f;
            exfil.Settings.StartTime = 0;
            exfil.Requirements = [];
            exfil.Settings.EntryPoints = "factory";

            Plugin.LogSource.LogError($"collider enabled {collider.enabled}");
            return exfil;
        }

        public static string GetCurrentLocationId()
        {
            TarkovApplication tarkovApplication = (TarkovApplication)Singleton<ClientApplication<ISession>>.Instance;

            RaidSettings currentRaidSettings = (RaidSettings)typeof(TarkovApplication)
                .GetField("_raidSettings", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(tarkovApplication);

            return currentRaidSettings?.SelectedLocation?.Id;
        }
    }
}

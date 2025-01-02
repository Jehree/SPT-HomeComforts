using EFT;
using EFT.Interactive;
using HomeComforts.Helpers;
using SPT.Reflection.Patching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HomeComforts.Patches
{
    internal class InitAllExfiltrationPointsPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(ExfiltrationControllerClass).GetMethod(nameof(ExfiltrationControllerClass.InitAllExfiltrationPoints));
        }

        [PatchPrefix]
        private static void Prefix(MongoID locationId, LocationExitClass[] settings, ref ExfiltrationControllerClass __instance)
        {
            List<ExfiltrationPoint> exfils = LocationScene.GetAllObjects<ExfiltrationPoint>(false).ToList();
            var customExfil = CustomExfilUtils.CreateExfil("test_exfil");
            Plugin.LogSource.LogError($"collider enabled 2 {customExfil.gameObject.GetComponent<BoxCollider>().enabled}");
            Plugin.ExfilPoint = customExfil;
            exfils.Add(customExfil);

            __instance.ExfiltrationPoints = [..exfils];
        }
    }
}

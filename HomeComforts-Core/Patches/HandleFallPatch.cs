using EFT.HealthSystem;
using SPT.Reflection.Patching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HomeComforts.Patches
{
    internal class HandleFallPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(ActiveHealthController).GetMethod(nameof(ActiveHealthController.HandleFall));
        }

        [PatchPrefix]
        public static bool PatchPrefix(ActiveHealthController __instance)
        {
            //if (!__instance.Player.IsYourPlayer) return true;
            if (Settings.DisableFallDamage.Value) return false;
            return true;
        }
    }
}

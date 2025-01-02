using Comfort.Common;
using EFT;
using EFT.Interactive;
using HomeComforts.Fika;
using HomeComforts.Helpers;
using SPT.Reflection.Patching;
using System.Reflection;

namespace HomeComforts.Patches
{
    public class OnExfilTriggerEntered : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(ExfiltrationPoint).GetMethod(nameof(ExfiltrationPoint.InfiltrationMatch));
        }

        [PatchPostfix]
        static void PatchPrefix(ref bool __result)
        {
            Plugin.LogSource.LogError(__result);
        }
    }
}

using SPT.Reflection.Patching;
using System.Reflection;

namespace HomeComforts.Patches
{
    internal class InitAllExfiltrationPointsPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(ExfiltrationControllerClass).GetMethod(nameof(ExfiltrationControllerClass.InitAllExfiltrationPoints));
        }

        [PatchPostfix]
        private static void Postfix(ref ExfiltrationControllerClass __instance)
        {
            SafehouseSession.InitializeCustomExfil(__instance);
        }
    }
}

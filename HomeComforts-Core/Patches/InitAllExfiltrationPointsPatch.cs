using HomeComforts.Components;
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
            var customExfil = SafehouseExfil.Create("homecomforts_safehouse");
            customExfil.InitCustomExfil();

            ModSession.Instance.CustomSafehouseExfil = customExfil;

            __instance.ExfiltrationPoints = [.. __instance.ExfiltrationPoints, ModSession.Instance.CustomSafehouseExfil];
        }
    }
}

using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using SPT.Reflection.Utils;
using System;
using System.Linq;
using System.Reflection;

namespace HomeComforts.Patches
{
    public class GameEndedPatch : ModulePatch
    {
        private static Type _targetClassType;
        private static PropertyInfo _exitNameInfo;

        protected override MethodBase GetTargetMethod()
        {
            _targetClassType = PatchConstants.EftTypes.Single(targetClass =>
                !targetClass.IsInterface &&
                !targetClass.IsNested &&
                targetClass.GetMethods().Any(method => method.Name == "LocalRaidEnded") &&
                targetClass.GetMethods().Any(method => method.Name == "ReceiveInsurancePrices")
            );

            var targetMethod = AccessTools.Method(_targetClassType.GetTypeInfo(), "LocalRaidEnded");

            _exitNameInfo = targetMethod.GetParameters()[1].ParameterType.GetProperty("exitName");

            return targetMethod;
        }

        // LocalRaidSettings settings, GClass1924 results, GClass1301[] lostInsuredItems, Dictionary<string, GClass1301[]> transferItems
        [PatchPostfix]
        static void Postfix(LocalRaidSettings settings, object results, ref object[] lostInsuredItems, object transferItems)
        {
        }
    }
}

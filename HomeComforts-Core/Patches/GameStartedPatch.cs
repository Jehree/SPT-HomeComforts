using Comfort.Common;
using EFT;
using EFT.Interactive;
using HomeComforts.Fika;
using HomeComforts.Helpers;
using SPT.Reflection.Patching;
using System.Reflection;
using UnityEngine;

namespace HomeComforts.Patches
{
    public class GameStartedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(GameWorld).GetMethod(nameof(GameWorld.OnGameStarted));
        }

        [PatchPrefix]
        static void PatchPrefix()
        {
            //Plugin.ExfilPoint.Settings.EntryPoints = Singleton<GameWorld>.Instance.MainPlayer.Profile.Info.EntryPoint.ToLower();
            Plugin.ExfilPoint.EligibleEntryPoints = [Singleton<GameWorld>.Instance.MainPlayer.Profile.Info.EntryPoint.ToLower()];
            Plugin.LogSource.LogError($"collider enabled 3 {Plugin.ExfilPoint.gameObject.GetComponent<BoxCollider>().enabled}");

        }
    }
}

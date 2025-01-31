using EFT;
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
            if (Plugin.IEAPIInstalled)
            {
                // I KNOW this is awful, but it's only once on raid start. I tested it on Streets and it takes very little time for this to run.
                // TODO: talk with Trap on a way to make it easier to target the IEAPI custom trigger.
                GameObject.Find("homecomforts_safehouse_IEAPIIgnore_custom_trigger")?.SetActive(false);

                // PR made for IEAPI to add that feature :) IEAPIIgnore added to game object name
            }
        }
    }
}

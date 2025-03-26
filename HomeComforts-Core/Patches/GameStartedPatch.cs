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
        }
    }
}

using Comfort.Common;
using EFT;
using HomeComforts.Items.Safehouse;
using HomeComforts.Items.SpaceHeater;

namespace HomeComforts.Fika
{
    public class FikaInterface
    {
        public static bool IAmHost()
        {
            if (!Plugin.FikaInstalled) return true;
            return FikaWrapper.IAmHost();
        }

        public static string GetRaidId()
        {
            if (!Plugin.FikaInstalled) return Singleton<GameWorld>.Instance.MainPlayer.ProfileId;
            return FikaWrapper.GetRaidId();
        }

        public static void InitOnPluginEnabled()
        {
            if (!Plugin.FikaInstalled) return;
            FikaWrapper.InitOnPluginEnabled();
        }

        public static void SendHostSafehouseProfileDataPacket(bool removeProfile)
        {
            if (!Plugin.FikaInstalled) return;
            SafehouseFikaWrapper.SendHostSafehouseProfileDataPacket(removeProfile);
        }
    }
}

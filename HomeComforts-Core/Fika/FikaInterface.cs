using Comfort.Common;
using EFT;
using EFT.Interactive;
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

        public static void SendSpaceHeaterStatePacket(bool enabled, string spaceHeaterId)
        {
            if (!Plugin.FikaInstalled) return;
            SpaceHeaterFikaWrapper.SendSpaceHeaterStatePacket(enabled, spaceHeaterId);
        }

        public static void Extract(ExfiltrationPoint exfil)
        {
            if (!Plugin.FikaInstalled) return;
            SafehouseFikaWrapper.Extract(exfil);
        }

        public static void SendSafehouseEnabledStatePacket(bool enabled, string safehouseId)
        {
            if (!Plugin.FikaInstalled) return;
            SafehouseFikaWrapper.SendSafehouseEnabledStatePacket(enabled, safehouseId);
        }
    }
}

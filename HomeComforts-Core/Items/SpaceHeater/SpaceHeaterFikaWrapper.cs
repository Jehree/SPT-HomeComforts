using Comfort.Common;
using Fika.Core.Networking;
using HomeComforts.Components;
using HomeComforts.Fika;
using LiteNetLib;
using LockableDoors.Packets;

namespace HomeComforts.Items.SpaceHeater
{
    public static class SpaceHeaterFikaWrapper
    {
        public static void SendSpaceHeaterStatePacket(bool enabled, string spaceHeaterId)
        {
            var packet = new SpaceHeaterStatePacket
            {
                Enabled = enabled,
                SpaceHeaterId = spaceHeaterId
            };

            if (FikaWrapper.IAmHost())
            {
                Singleton<FikaServer>.Instance.SendDataToAll(ref packet, DeliveryMethod.ReliableOrdered);
            }
            else
            {
                Singleton<FikaClient>.Instance.SendData(ref packet, DeliveryMethod.ReliableOrdered);
            }
        }

        public static void OnSpaceHeaterStatePacketReceived(SpaceHeaterStatePacket packet, NetPeer peer)
        {
#if DEBUG
            Plugin.LogSource.LogError("OnSpaceHeaterStatePacketReceived");
#endif

            if (FikaWrapper.IAmHost())
            {
                Singleton<FikaServer>.Instance.SendDataToAll(ref packet, LiteNetLib.DeliveryMethod.ReliableOrdered);
            }

            var heater = HCSession.Instance.SpaceHeaterSession.GetSpaceHeaterOrNull(packet.SpaceHeaterId);
            if (heater == null) return;

            heater.AOEEnabled = packet.Enabled;
            heater.FakeItem.PutAddonData(Plugin.AddonDataKey, SpaceHeaterAddonData.CreateData(packet.Enabled));
        }
    }
}

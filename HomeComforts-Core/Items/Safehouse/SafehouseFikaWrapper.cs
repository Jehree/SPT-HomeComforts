using Comfort.Common;
using Fika.Core.Networking;
using HomeComforts.Components;
using HomeComforts.Fika;
using LiteNetLib;
using HomeComforts.Packets;


namespace HomeComforts.Items.Safehouse
{
    public static class SafehouseFikaWrapper
    {
        public static void SendHostSafehouseProfileDataPacket(bool removeProfile)
        {
            if (FikaWrapper.IAmHost()) return;

            SafehouseProfileDataPacket packet;

            if (removeProfile)
            {
                packet = new SafehouseProfileDataPacket
                {
                    ProfileId = HCSession.Instance.Player.ProfileId,
                    RemoveProfile = true
                };
            }
            else
            {
                // if there LastSafehouseThatUsedMe is null, there's nothing to update
                if (HCSession.Instance.CustomSafehouseExfil.LastSafehouseThatUsedMe == null) return;

                packet = new SafehouseProfileDataPacket
                {
                    ProfileId = HCSession.Instance.Player.ProfileId,
                    InfilPosition = HCSession.Instance.CustomSafehouseExfil.transform.position,
                    SafehouseId = HCSession.Instance.CustomSafehouseExfil.LastSafehouseThatUsedMe.FakeItem.LootItem.ItemId,
                    RemoveProfile = false
                };
            }

            Singleton<FikaClient>.Instance.SendData(ref packet, DeliveryMethod.ReliableOrdered);
        }

        public static void OnSafehouseProfileDataPacketReceived(SafehouseProfileDataPacket packet, NetPeer peer)
        {
#if DEBUG
            Plugin.DebugLog("OnSafehouseProfileDataPacketReceived");
#endif

            if (!FikaWrapper.IAmHost()) return;
            if (packet.RemoveProfile)
            {
                HCSession.Instance.SafehouseSession.AddonData.RemoveProfile(packet.ProfileId);
            }
            else
            {
                HCSession.Instance.SafehouseSession.AddonData.AddProfile(packet.ProfileId, packet.InfilPosition, packet.SafehouseId);
            }

        }
    }
}

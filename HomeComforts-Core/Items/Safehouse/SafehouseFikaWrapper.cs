using Comfort.Common;
using Fika.Core.Networking;
using HomeComforts.Components;
using HomeComforts.Fika;
using LiteNetLib;
using LockableDoors.Packets;


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

        public static void SendSafehouseEnabledStatePacket(bool enabled, string safehouseId)
        {
            if (FikaWrapper.IAmHost()) return;

            SafehouseEnabledStatePacket packet = new SafehouseEnabledStatePacket
            {
                Enabled = enabled,
                SafehouseId = safehouseId,
                ProfileId = HCSession.Instance.Player.ProfileId,
            };

            Singleton<FikaClient>.Instance.SendData(ref packet, DeliveryMethod.ReliableOrdered);
        }

        public static void OnSafehouseEnabledStatePacketReceived(SafehouseEnabledStatePacket packet, NetPeer peer)
        {
#if DEBUG
            Plugin.DebugLog("OnSafehouseEnabledStatePacketReceived");
#endif

            Safehouse safehouse = HCSession.Instance.SafehouseSession.GetSafehouseOrNull(packet.SafehouseId);
            if (safehouse == null) return;

            if (packet.Enabled)
            {
                safehouse.AddonData.AddProfileId(packet.ProfileId);
            }
            else
            {
                safehouse.AddonData.RemoveProfileId(packet.ProfileId);
            }
        }
    }
}

﻿using Comfort.Common;
using Fika.Core.Coop.Utils;
using Fika.Core.Modding;
using Fika.Core.Modding.Events;
using Fika.Core.Networking;
using HomeComforts.Items.Safehouse;
using HomeComforts.Items.SpaceHeater;
using LiteNetLib;
using HomeComforts.Packets;

namespace HomeComforts.Fika
{
    public static class FikaWrapper
    {
        public static bool IAmHost()
        {
            return Singleton<FikaServer>.Instantiated;
        }

        public static string GetRaidId()
        {

            return FikaBackendUtils.GroupId;
        }

        public static void OnFikaNetManagerCreated(FikaNetworkManagerCreatedEvent managerCreatedEvent)
        {
            managerCreatedEvent.Manager.RegisterPacket<SafehouseProfileDataPacket, NetPeer>(SafehouseFikaWrapper.OnSafehouseProfileDataPacketReceived);
        }

        public static void InitOnPluginEnabled()
        {
            FikaEventDispatcher.SubscribeEvent<FikaNetworkManagerCreatedEvent>(OnFikaNetManagerCreated);
        }
    }
}

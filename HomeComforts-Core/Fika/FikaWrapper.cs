using Comfort.Common;
using Fika.Core.Coop.Utils;
using Fika.Core.Modding;
using Fika.Core.Modding.Events;
using Fika.Core.Networking;

namespace HomeComforts.Fika
{
    public class FikaWrapper
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
            //managerCreatedEvent.Manager.RegisterPacket<DoorLockedStatePacket, NetPeer>(OnDoorLockedStatePacketReceived);
        }

        public static void InitOnPluginEnabled()
        {
            FikaEventDispatcher.SubscribeEvent<FikaNetworkManagerCreatedEvent>(OnFikaNetManagerCreated);
        }
    }
}

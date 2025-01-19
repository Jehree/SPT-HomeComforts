using Fika.Core.Networking;
using LiteNetLib.Utils;
using UnityEngine;

namespace LockableDoors.Packets
{
    public struct SafehouseProfileDataPacket : INetSerializable
    {
        public string ProfileId;
        public Vector3 InfilPosition;
        public string SafehouseId;
        public bool RemoveProfile;

        public void Deserialize(NetDataReader reader)
        {
            ProfileId = reader.GetString();
            InfilPosition = reader.GetVector3();
            SafehouseId = reader.GetString();
            RemoveProfile = reader.GetBool();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(ProfileId);
            writer.Put(InfilPosition);
            writer.Put(SafehouseId);
            writer.Put(RemoveProfile);
        }
    }

    public struct SafehouseEnabledStatePacket : INetSerializable
    {
        public bool Enabled;
        public string SafehouseId;
        public string ProfileId;

        public void Deserialize(NetDataReader reader)
        {
            Enabled = reader.GetBool();
            SafehouseId = reader.GetString();
            ProfileId = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Enabled);
            writer.Put(SafehouseId);
            writer.Put(ProfileId);
        }
    }

    public struct SpaceHeaterStatePacket : INetSerializable
    {
        public bool Enabled;
        public string SpaceHeaterId;

        public void Deserialize(NetDataReader reader)
        {
            Enabled = reader.GetBool();
            SpaceHeaterId = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Enabled);
            writer.Put(SpaceHeaterId);
        }
    }
}

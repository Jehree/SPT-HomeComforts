using HomeComforts.Components;
using System.Collections.Generic;
using UnityEngine;

namespace HomeComforts.Common
{
    internal class SafehouseDataPack
    {
        public string HostProfileId;
        public string MapId;
        public Dictionary<string, Vector3> InfilPositionLookup = [];
        public List<string> SafehouseItemIds;

        public static SafehouseDataPack Empty
        {
            get
            {
                string profileId = ModSession.Instance.Player.ProfileId;
                string locationId = ModSession.Instance.GameWorld.LocationId;
                var data = new SafehouseDataPack(profileId, locationId);
                return data;
            }
        }

        public SafehouseDataPack(string hostProfileId, string mapId, Vector3 infilPosition = default)
        {
            HostProfileId = hostProfileId;
            MapId = mapId;
            InfilPositionLookup[ModSession.Instance.Player.ProfileId] = infilPosition;
        }
        public SafehouseDataPack() { }
    }
}

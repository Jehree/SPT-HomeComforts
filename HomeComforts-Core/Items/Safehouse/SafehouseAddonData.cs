using HomeComforts.Components;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace HomeComforts.Items.Safehouse
{
    internal class SafehouseAddonData
    {
        [JsonProperty("_profilesWithSafehouseEnabled")]
        private List<string> _profilesWithSafehouseEnabled = [];

        public void AddProfileId(string profileId)
        {
            if (_profilesWithSafehouseEnabled.Contains(profileId)) return;
            _profilesWithSafehouseEnabled.Add(profileId);
        }

        public void AddProfileId()
        {
            AddProfileId(HCSession.Instance.Player.ProfileId);
        }

        public void RemoveProfileId(string profileId)
        {
            _profilesWithSafehouseEnabled.Remove(profileId);
        }
        public void RemoveProfileId()
        {
            RemoveProfileId(HCSession.Instance.Player.ProfileId);
        }

        public bool ContainsMainPlayer()
        {
            return _profilesWithSafehouseEnabled.Contains(HCSession.Instance.Player.ProfileId);
        }
    }

    internal class SafehouseGlobalAddonData
    {
        [JsonProperty("_profileDataLookup")]
        private Dictionary<string, ProfileData> _profileDataLookup = [];

        internal class ProfileData
        {
            public string ProfileId;
            public Vector3 InfilPosition;
            public string SafehouseId;
        }

        public void AddProfile(string profileId, Vector3 infilPos, string safehouseId)
        {
            _profileDataLookup[profileId] = new ProfileData
            {
                ProfileId = profileId,
                InfilPosition = infilPos,
                SafehouseId = safehouseId
            };
        }

        public void AddProfile()
        {
            string profileId = HCSession.Instance.Player.ProfileId;
            Vector3 infilPos = HCSession.Instance.CustomSafehouseExfil.gameObject.transform.position;
            string itemId = HCSession.Instance.CustomSafehouseExfil.LastSafehouseThatUsedMe.FakeItem.LootItem.ItemId;
            AddProfile(profileId, infilPos, itemId);
        }

        public ProfileData GetProfile(string profileId)
        {
            if (!_profileDataLookup.ContainsKey(profileId)) return null;
            return _profileDataLookup[profileId];
        }

        public ProfileData GetProfile()
        {
            return GetProfile(HCSession.Instance.Player.ProfileId);
        }

        public void RemoveProfile(string profileId)
        {
            _profileDataLookup.Remove(profileId);
        }

        public void RemoveProfile()
        {
            RemoveProfile(HCSession.Instance.Player.ProfileId);
        }

        public bool ContainsProfile(string profileId)
        {
            return _profileDataLookup.ContainsKey(profileId);
        }

        public bool ContainsProfile()
        {
            return ContainsProfile(HCSession.Instance.Player.ProfileId);
        }
    }
}

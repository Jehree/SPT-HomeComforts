using HomeComforts.Components;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HomeComforts.Common
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
            AddProfileId(ModSession.Instance.Player.ProfileId);
        }

        public void RemoveProfileId(string profileId)
        {
            _profilesWithSafehouseEnabled.Remove(profileId);
        }
        public void RemoveProfileId()
        {
            RemoveProfileId(ModSession.Instance.Player.ProfileId);
        }

        public bool ContainsMainPlayer()
        {
            return _profilesWithSafehouseEnabled.Contains(ModSession.Instance.Player.ProfileId);
        }
    }

    internal class RaidSessionAddonData
    {
        [JsonProperty("_profileDataLookup")]
        private Dictionary<string, ProfileData> _profileDataLookup = [];

        internal class ProfileData
        {
            public string ProfileId;
            public Vector3 InfilPosition;
            public string SafehouseItemId;
        }

        public void ProfileDataCleanup()
        {
            List<string> profilesToRemove = [];
            foreach (var kvp in _profileDataLookup)
            {
                if (ModSession.Instance.GetSafehouseItemOrNull(kvp.Value.SafehouseItemId) == null)
                {
                    profilesToRemove.Add(kvp.Key);
                }
            }

            profilesToRemove.ExecuteForEach(profileId => RemoveProfile(profileId));
        }

        public void AddProfile(string profileId, Vector3 infilPos, string safehouseItemId)
        {
            _profileDataLookup[profileId] = new ProfileData
            {
                ProfileId = profileId,
                InfilPosition = infilPos,
                SafehouseItemId = safehouseItemId
            };
        }

        public void AddProfile()
        {
            string profileId = ModSession.Instance.Player.ProfileId;
            Vector3 infilPos = ModSession.Instance.CustomSafehouseExfil.gameObject.transform.position;
            string itemId = ModSession.Instance.CustomSafehouseExfil.LastSafehouseItemThatUsedMe.FakeItem.LootItem.ItemId;
            AddProfile(profileId, infilPos, itemId);
        }

        public ProfileData GetProfile(string profileId)
        {
            if (!_profileDataLookup.ContainsKey(profileId)) return null;
            return _profileDataLookup[profileId];
        }

        public ProfileData GetProfile()
        {
            return GetProfile(ModSession.Instance.Player.ProfileId);
        }

        public void RemoveProfile(string profileId)
        {
            _profileDataLookup.Remove(profileId);
        }

        public void RemoveProfile()
        {
            RemoveProfile(ModSession.Instance.Player.ProfileId);
        }

        public bool ContainsProfile(string profileId)
        {
            return _profileDataLookup.ContainsKey(profileId);
        }

        public bool ContainsProfile()
        {
            return ContainsProfile(ModSession.Instance.Player.ProfileId);
        }
    }
}

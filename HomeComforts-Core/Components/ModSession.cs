using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using HomeComforts.Common;
using HomeComforts.Helpers;
using LeaveItThere.Components;
using LeaveItThere.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HomeComforts.Components
{
    internal class ModSession : MonoBehaviour
    {
        public GameWorld GameWorld { get; private set; }
        public Player Player { get; private set; }
        public GamePlayerOwner GamePlayerOwner { get; private set; }
        public NeedsRateReductions NeedsRateReductions { get; private set; } = new();
        private ModSession() { }
        private static ModSession _instance = null;
        public static ModSession Instance
        {
            get
            {
                if (!Singleton<GameWorld>.Instantiated)
                {
                    throw new Exception("Tried to get ModSession when game world was not instantiated!");
                }
                if (_instance == null)
                {
                    _instance = Singleton<GameWorld>.Instance.MainPlayer.gameObject.GetOrAddComponent<ModSession>();
                }
                return _instance;
            }
        }

        public SafehouseExfil CustomSafehouseExfil;
        public List<SafehouseItem> EnabledSafehouseItems { get; private set; } = [];

        private RaidSessionAddonData _addonData;
        public RaidSessionAddonData AddonData
        {
            get
            {
                var leaveItThereSession = LeaveItThere.Components.ModSession.Instance;

                // first check will be null the first time we get AddonData
                if (_addonData == null)
                {
                    _addonData = leaveItThereSession.GetGlobalAddonDataOrNull<RaidSessionAddonData>(Plugin.AddonDataKey);
                }

                // second check will still be null if there is no AddonData for our key
                // in that case, we add an empty one
                if (_addonData == null)
                {
                    var newAddonData = new RaidSessionAddonData();
                    leaveItThereSession.PutGlobalAddonData(Plugin.AddonDataKey, newAddonData);
                    _addonData = newAddonData;
                }

                return _addonData;
            }
        }

        public SpaceHeaterSession SpaceHeater { get; private set; } = new();

        public bool SafehouseEnableAllowed
        {
            get
            {
                int enabledSafehouseCount = 0;
                foreach (var safehouse in EnabledSafehouseItems)
                {
                    if (safehouse.SafehouseEnabled) enabledSafehouseCount++;
                }

                return Settings.ThisMapSafehouseLimit > enabledSafehouseCount;
            }
        }

        private void Awake()
        {
            GameWorld = Singleton<GameWorld>.Instance;
            Player = GameWorld.MainPlayer;
            GamePlayerOwner = Player.GetComponent<GamePlayerOwner>();
        }

        public static void OnHostRaidEnd(LocalRaidSettings settings, object results, object lostInsuredItems, object transferItems, string exitName)
        {
            if (exitName == "homecomforts_safehouse")
            {
                Instance.AddonData.AddProfile();
            }
            Instance.AddonData.ProfileDataCleanup();
        }

        public static void OnLastPlacedItemSpawned(FakeItem fakeItem)
        {
            // Once the last placed item is spawned, it is safe to check if a safehouse item exists

            if (!Instance.AddonData.ContainsProfile()) return;

            var profileData = Instance.AddonData.GetProfile();

            if (Instance.SafehouseItemExists(profileData.SafehouseItemId))
            {
                Instance.Player.Teleport(profileData.InfilPosition);
            }
            else
            {
                Instance.AddonData.RemoveProfile();
            }
        }

        public SafehouseItem GetSafehouseItemOrNull(string itemId)
        {
            return EnabledSafehouseItems.FirstOrDefault(i => i.FakeItem.ItemId == itemId);
        }

        public bool SafehouseItemExists(string itemId)
        {
            return GetSafehouseItemOrNull(itemId) != null;
        }

        public void RemoveSafehouseItem(SafehouseItem safehouseItem)
        {
            EnabledSafehouseItems.Remove(safehouseItem);
        }

        public void AddSafehouseItem(SafehouseItem safehouseItem)
        {
            if (EnabledSafehouseItems.Contains(safehouseItem))
            {
                throw new Exception("Tried to add a SafehouseItem to ModSession when it already existed in the list!");
            }
            EnabledSafehouseItems.Add(safehouseItem);
        }

        internal class SpaceHeaterSession
        {
            public List<SpaceHeater> SpaceHeaters = [];
            private List<string> _playerIsInSpaceHeaterItemIds = [];
            public List<string> PlayerIsInSpaceHeaterItemIds
            {
                get
                {
                    return _playerIsInSpaceHeaterItemIds;
                }
            }
            public bool PlayerIsInSpaceHeaterZone
            {
                get
                {
                    return _playerIsInSpaceHeaterItemIds.Count != 0;
                }
            }
            public void AddSpaceHeaterIdToPlayerIsIn(string id)
            {
                if (_playerIsInSpaceHeaterItemIds.Contains(id)) return;
                _playerIsInSpaceHeaterItemIds.Add(id);
            }

            public void RemoveSpaceHeaterIdFromPlayerIsIn(string id)
            {
                _playerIsInSpaceHeaterItemIds.Remove(id);
            }

            public SpaceHeater GetSpaceHeaterOrNull(string itemId)
            {
                return SpaceHeaters.FirstOrDefault(heater => heater.FakeItem.ItemId == itemId);
            }

            public bool SpaceHeaterIsEnabled(string itemId)
            {
                var heater = GetSpaceHeaterOrNull(itemId);
                if (heater == null) return false;
                return heater.AOEEnabled;
            }
        }
    }
}

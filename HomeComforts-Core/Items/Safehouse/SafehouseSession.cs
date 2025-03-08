using EFT;
using HomeComforts;
using HomeComforts.Components;
using HomeComforts.Helpers;
using HomeComforts.Items.Safehouse;
using LeaveItThere.Addon;
using LeaveItThere.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class SafehouseSession
{
    private static SafehouseSession _session
    {
        get
        {
            return HCSession.Instance.SafehouseSession;
        }
    }
    public List<Safehouse> EnabledSafehouses { get; private set; } = [];

    private SafehouseGlobalAddonData _addonData;
    public SafehouseGlobalAddonData AddonData
    {
        get
        {
            // first check will be null the first time we get AddonData
            if (_addonData == null)
            {
                _addonData = LITSession.Instance.GetGlobalAddonDataOrNull<SafehouseGlobalAddonData>(Plugin.AddonDataKey);
            }

            // second check will still be null if there is no AddonData for our key
            // in that case, we add an empty one
            if (_addonData == null)
            {
                SafehouseGlobalAddonData newAddonData = new SafehouseGlobalAddonData();
                LITSession.Instance.PutGlobalAddonData(Plugin.AddonDataKey, newAddonData);
                _addonData = newAddonData;
            }

            return _addonData;
        }
    }

    public bool SafehouseEnableAllowed
    {
        get
        {
            int enabledSafehouseCount = 0;
            foreach (Safehouse safehouse in EnabledSafehouses)
            {
                if (safehouse.SafehouseEnabled) enabledSafehouseCount++;
            }

            return Settings.ThisMapSafehouseLimit > enabledSafehouseCount;
        }
    }

    public Safehouse GetSafehouseOrNull(string itemId)
    {
        return EnabledSafehouses.FirstOrDefault(i => i.FakeItem.ItemId == itemId);
    }

    public bool SafehouseExists(string itemId)
    {
        return GetSafehouseOrNull(itemId) != null;
    }

    public void RemoveSafehouse(Safehouse safehouse)
    {
        EnabledSafehouses.Remove(safehouse);
    }

    public void AddSafehouse(Safehouse safehouse)
    {
        if (EnabledSafehouses.Contains(safehouse))
        {
            throw new Exception("Tried to add a Safehouse to HomeComfortsSession when it already existed in the list!");
        }
        EnabledSafehouses.Add(safehouse);
    }

    public static void OnRaidEnd(LocalRaidSettings settings, object results, object lostInsuredItems, object transferItems, string exitName)
    {
        bool profileCleanupNeeded = exitName != "homecomforts_safehouse" && !Settings.AlwaysInfilAtSafehouse.Value;

        if (exitName == "homecomforts_safehouse")
        {
            _session.AddonData.AddProfile();
        }

        if (profileCleanupNeeded)
        {
            _session.AddonData.RemoveProfile();
        }

        SafehouseProfileDataToHostPacket.Instance.Send(profileCleanupNeeded);
    }

    public static void InitializeCustomExfil(ExfiltrationControllerClass exfilController)
    {
        HCSession.Instance.CustomSafehouseExfil = SafehouseExfil.Create("homecomforts_safehouse");
        HCSession.Instance.CustomSafehouseExfil.transform.position = HCSession.Instance.InitialExfilPosition;
        HCSession.Instance.CustomSafehouseExfil.InitCustomExfil();

        exfilController.ExfiltrationPoints = [.. exfilController.ExfiltrationPoints, HCSession.Instance.CustomSafehouseExfil];
    }

    public static void OnLastPlacedItemSpawned(FakeItem fakeItem)
    {
        // Once the last placed item is spawned, it is safe to check if a safehouse item exists

        if (!_session.AddonData.ContainsProfile()) return;

        SafehouseGlobalAddonData.ProfileData profileData = _session.AddonData.GetProfile();

        Safehouse safehouse = _session.GetSafehouseOrNull(profileData.SafehouseId);

        if (safehouse != null && safehouse.SafehouseEnabled)
        {
            HCSession.Instance.Player.Teleport(profileData.InfilPosition);
        }
        else
        {
            if (_session.AddonData.ContainsProfile())
            {
                _session.AddonData.RemoveProfile();
                SafehouseProfileDataToHostPacket.Instance.Send(true);
            }
        }
    }

    public class SafehouseProfileDataToHostPacket : LITPacketRegistration
    {
        public class Data
        {
            public Vector3 InfilPosition;
            public string SafehouseId;
            public bool RemoveProfile;
        }

        public static SafehouseProfileDataToHostPacket Instance { get => Get<SafehouseProfileDataToHostPacket>(); }
        public override EPacketDestination Destination => EPacketDestination.HostOnly;

        public override void OnPacketReceived(Packet packet)
        {
            Data data = packet.GetData<Data>();

            if (data.RemoveProfile)
            {
                HCSession.Instance.SafehouseSession.AddonData.RemoveProfile(packet.SenderProfileId);
            }
            else
            {
                HCSession.Instance.SafehouseSession.AddonData.AddProfile(packet.SenderProfileId, data.InfilPosition, data.SafehouseId);
            }

            Plugin.DebugLog("eor pak received");
        }

        public void Send(bool removeProfile)
        {
            // host will never need to send this packet
            if (LITFikaTools.IAmHost()) return;

            // if the LastSafehouseThatUsedMe is null, and we aren't removing a profile, there's nothing to update
            if (!removeProfile && HCSession.Instance.CustomSafehouseExfil.LastSafehouseThatUsedMe == null) return;

            Data data = new();

            // sometimes (like when disconnecting via esc menu) the exfil is destroyed too quickly to get data from it
            // fortunately, these times that data isn't needed anyway because the profile just needs to be removed anyway
            if (removeProfile)
            {
                data.InfilPosition = Vector3.zero;
                data.SafehouseId = "n/a";
                data.RemoveProfile = true;
            }
            else
            {
                data.InfilPosition = HCSession.Instance.CustomSafehouseExfil.transform.position;
                data.SafehouseId = HCSession.Instance.CustomSafehouseExfil.LastSafehouseThatUsedMe.FakeItem.LootItem.ItemId;
                data.RemoveProfile = false;
            }

            SendData(data);
        }
    }

}


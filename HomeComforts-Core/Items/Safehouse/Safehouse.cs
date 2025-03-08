using HomeComforts.Components;
using HomeComforts.Helpers;
using LeaveItThere.Addon;
using LeaveItThere.Common;
using LeaveItThere.Components;
using UnityEngine;

namespace HomeComforts.Items.Safehouse
{
    internal class Safehouse : MonoBehaviour
    {
        public FakeItem FakeItem { get; private set; }
        public bool SafehouseEnabled = false;

        private SafehouseAddonData _addonData;
        public SafehouseAddonData AddonData
        {
            get
            {
                // first check will be null the first time we get AddonData
                if (_addonData == null)
                {
                    _addonData = FakeItem.GetAddonDataOrNull<SafehouseAddonData>(Plugin.AddonDataKey);
                }

                // second check will still be null if there is no AddonData for our key
                // in that case, we add an empty one
                if (_addonData == null)
                {
                    SafehouseAddonData newAddonData = new SafehouseAddonData();
                    FakeItem.PutAddonData(Plugin.AddonDataKey, newAddonData);
                    _addonData = newAddonData;
                }

                return _addonData;
            }
        }

        private void OnPlacedStateChanged(bool isPlaced)
        {
            if (!isPlaced)
            {
                SetSafehouseEnabled(false);
                HCSession.Instance.SafehouseSession.RemoveSafehouse(this);
            }
        }

        private void OnPlacedItemSpawned()
        {
            if (AddonData.ContainsMainPlayer())
            {
                // moving the exfil here now makes it show correctly in Dynamic Maps
                // unfortunately, if the safehouse radio is moved, the map won't update until the next raid
                // we do have to delay it though because this code will run before the exfil is created
                HCSession.Instance.InitialExfilPosition = transform.position;
                SetSafehouseEnabled(true);
            }
        }

        public static void OnFakeItemInitialized(FakeItem fakeItem)
        {
            if (!Plugin.ServerConfig.SafehouseItemIds.Contains(fakeItem.LootItem.Item.TemplateId)) return;

            fakeItem.gameObject.transform.localScale *= 2;
            if (!Settings.ScavsCanUseSafehouse.Value && HCSession.Instance.Player.Side == EFT.EPlayerSide.Savage) return;

            Safehouse safehouse = fakeItem.gameObject.AddComponent<Safehouse>();
            safehouse.Init(fakeItem);
            fakeItem.Interactions.Add(new ToggleSafehouseEnabledInteraction(fakeItem, safehouse));
            fakeItem.Interactions.Add(new ToggleExfilEnabledInteraction(fakeItem, safehouse));
        }

        private void Init(FakeItem fakeItem)
        {
            FakeItem = fakeItem;
            HCSession.Instance.SafehouseSession.AddSafehouse(this);

            FakeItem.OnPlacedStateChanged += OnPlacedStateChanged;
            FakeItem.OnSpawned += OnPlacedItemSpawned;
        }

        public void SetSafehouseEnabled(bool enabled)
        {
            if (enabled == SafehouseEnabled) return;
            SafehouseEnabled = enabled;
            FakeItem.Flags.MoveModeDisabled = enabled;
            FakeItem.Flags.ReclaimInteractionDisabled = enabled;

            if (enabled)
            {
                AddonData.AddProfileId();
            }
            else
            {
                HCSession.Instance.CustomSafehouseExfil.SetCustomExfilEnabled(false);
                AddonData.RemoveProfileId();
            }

            SafehouseEnabledStatePacket.Instance.Send(FakeItem.LootItem.ItemId, enabled);
            NotificationManagerClass.DisplayMessageNotification($"Safehouse Enabled: {SafehouseEnabled}");
        }

        public class SafehouseEnabledStatePacket : LITPacketRegistration
        {
            private class Data
            {
                public string SafehouseId;
                public bool Enabled;
            }

            public static SafehouseEnabledStatePacket Instance { get => Get<SafehouseEnabledStatePacket>(); }
            public override EPacketDestination Destination => EPacketDestination.HostOnly;
            public override void OnPacketReceived(Packet packet)
            {
                Data data = packet.GetData<Data>();

                Safehouse safehouse = HCSession.Instance.SafehouseSession.GetSafehouseOrNull(data.SafehouseId);
                if (safehouse == null) return;

                if (data.Enabled)
                {
                    safehouse.AddonData.AddProfileId(packet.SenderProfileId);
                }
                else
                {
                    safehouse.AddonData.RemoveProfileId(packet.SenderProfileId);
                }
            }

            public void Send(string safehouseId, bool enabled)
            {
                Data data = new()
                {
                    SafehouseId = safehouseId,
                    Enabled = enabled
                };
                SendData(data);
            }
        }

        public class ToggleSafehouseEnabledInteraction(FakeItem fakeItem, Safehouse safehouse) : CustomInteraction(fakeItem)
        {
            public Safehouse Safehouse { get; private set; } = safehouse;

            public override string Name => Safehouse.SafehouseEnabled
                                                ? "Disable Safehouse"
                                                : "Activate Safehouse";
            public override bool Enabled => Safehouse.SafehouseEnabled
                                                ? true //always enable interaction when safehouse is enabled so that it can be disabled
                                                : HCSession.Instance.SafehouseSession.SafehouseEnableAllowed;
            public override bool AutoPromptRefresh => true;
            public override void OnInteract() => Safehouse.SetSafehouseEnabled(!Safehouse.SafehouseEnabled);
        }

        public class ToggleExfilEnabledInteraction(FakeItem fakeItem, Safehouse safehouse) : CustomInteraction(fakeItem)
        {
            public Safehouse Safehouse { get; private set; } = safehouse;
            public SafehouseExfil Exfil { get => HCSession.Instance.CustomSafehouseExfil; }
            public override string Name => Exfil.ExfilIsEnabled
                                                    ? "Stop Extracting"
                                                    : "Extract";
            public override bool Enabled => Safehouse.SafehouseEnabled;
            public override bool AutoPromptRefresh => true;
            public override void OnInteract()
            {
                Exfil.SetCustomExfilEnabled(!Exfil.ExfilIsEnabled);

                if (Exfil.ExfilIsEnabled)
                {
                    Exfil.gameObject.transform.position = HCSession.Instance.Player.Transform.position;
                    Exfil.LastSafehouseThatUsedMe = Safehouse;
                }
            }
        }
    }
}

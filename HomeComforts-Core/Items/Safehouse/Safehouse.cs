using HomeComforts.Components;
using HomeComforts.Fika;
using LeaveItThere.Common;
using LeaveItThere.Components;
using LeaveItThere.Helpers;
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
                    var newAddonData = new SafehouseAddonData();
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
                SetSafehouseEnabled(true);
            }
        }

        public static void OnFakeItemInitialized(FakeItem fakeItem)
        {
            if (!Plugin.ServerConfig.SafehouseItemIds.Contains(fakeItem.LootItem.Item.TemplateId)) return;
            var safehouse = fakeItem.gameObject.AddComponent<Safehouse>();
            safehouse.Init(fakeItem);
            fakeItem.Actions.Add(GetToggleSafehouseEnabledAction(fakeItem.ItemId));
            fakeItem.Actions.Add(GetEnableExfilInteractionAction(fakeItem.ItemId));
            fakeItem.gameObject.transform.localScale *= 2;
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
            FakeItem.AddonFlags.MoveModeDisabled = enabled;

            if (enabled)
            {
                AddonData.AddProfileId();
            }
            else
            {
                HomeComforts.Components.HCSession.Instance.CustomSafehouseExfil.SetCustomExfilEnabled(false);
                AddonData.RemoveProfileId();
            }

            FikaInterface.SendSafehouseEnabledStatePacket(enabled, FakeItem.LootItem.ItemId);
            NotificationManagerClass.DisplayMessageNotification($"Safehouse Enabled: {SafehouseEnabled}");
            InteractionHelper.RefreshPrompt();
        }

        private static CustomInteraction GetToggleSafehouseEnabledAction(string itemId)
        {
            return new CustomInteraction(
                () =>
                {
                    var safehouse = HCSession.Instance.SafehouseSession.GetSafehouseOrNull(itemId);

                    if (safehouse.SafehouseEnabled)
                    {
                        return "Disable Safehouse";
                    }
                    else
                    {
                        return "Enable Safehouse";
                    }
                },
                () =>
                {
                    var safehouse = HCSession.Instance.SafehouseSession.GetSafehouseOrNull(itemId);
                    if (safehouse.SafehouseEnabled) return false; //always enable interaction when safehouse is enabled so that it can be disabled
                    return !HCSession.Instance.SafehouseSession.SafehouseEnableAllowed;
                },
                () =>
                {
                    var safehouse = HCSession.Instance.SafehouseSession.GetSafehouseOrNull(itemId);
                    safehouse.SetSafehouseEnabled(!safehouse.SafehouseEnabled);
                }
            );
        }

        //TODO: add 'Enable as Guest' option when safehouse limit is reached

        private static CustomInteraction GetEnableExfilInteractionAction(string itemId)
        {
            return new CustomInteraction
            (
                () =>
                {
                    if (HCSession.Instance.CustomSafehouseExfil.ExfilIsEnabled)
                    {
                        return "Stop Extracting";
                    }
                    else
                    {
                        return "Extract";
                    }
                },
                () =>
                {
                    var safehouse = HCSession.Instance.SafehouseSession.GetSafehouseOrNull(itemId);
                    return !safehouse.SafehouseEnabled;
                },
                () =>
                {
                    var exfil = HCSession.Instance.CustomSafehouseExfil;

                    exfil.SetCustomExfilEnabled(!exfil.ExfilIsEnabled);

                    if (exfil.ExfilIsEnabled)
                    {
                        exfil.gameObject.transform.position = HCSession.Instance.Player.gameObject.transform.position;
                        exfil.LastSafehouseThatUsedMe = HCSession.Instance.SafehouseSession.GetSafehouseOrNull(itemId);
                    }

                    // extraction is instant with Fika
                    FikaInterface.Extract(exfil);
                }
            );
        }
    }
}

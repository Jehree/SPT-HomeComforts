using HomeComforts.Common;
using HomeComforts.Fika;
using LeaveItThere.Common;
using LeaveItThere.Components;
using LeaveItThere.Helpers;
using UnityEngine;

namespace HomeComforts.Components
{
    public class TestClass
    {
        public Vector3 PlayerPos = ModSession.Instance.Player.gameObject.transform.position;
    }

    internal class SafehouseItem : MonoBehaviour
    {
        public FakeItem FakeItem { get; private set; }
        public SafehouseAreaOfAffect AOE { get; private set; }
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
                ModSession.Instance.RemoveSafehouseItem(this);
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

            var safehouseItem = fakeItem.gameObject.AddComponent<SafehouseItem>();
            safehouseItem.Init(fakeItem);
            safehouseItem.AOE = SafehouseAreaOfAffect.ApplyAOEObjectToSafehouseItem(safehouseItem);
            fakeItem.Actions.Add(GetToggleSafehouseItemEnabledAction(fakeItem.ItemId));
            fakeItem.Actions.Add(SafehouseAreaOfAffect.GetToggleAOEBoundsVisibleAction(fakeItem.ItemId));
            fakeItem.Actions.Add(GetEnableExfilInteractionAction(fakeItem.ItemId));
        }
        private void Init(FakeItem fakeItem)
        {
            FakeItem = fakeItem;
            ModSession.Instance.AddSafehouseItem(this);

            FakeItem.OnPlacedStateChanged += OnPlacedStateChanged;
            FakeItem.OnSpawned += OnPlacedItemSpawned;
        }

        public void SetSafehouseEnabled(bool enabled)
        {
            if (enabled == SafehouseEnabled) return;
            SafehouseEnabled = enabled;
            AOE.AOEEnabled = enabled;
            FakeItem.AddonFlags.MoveModeDisabled = enabled;

            if (enabled)
            {
                if (FikaInterface.IAmHost())
                {
                    AddonData.AddProfileId();
                }
                else
                {
                    // handle FikaClient(s)
                }
            }
            else
            {
                ModSession.Instance.CustomSafehouseExfil.SetCustomExfilEnabled(false);
                if (FikaInterface.IAmHost())
                {
                    AddonData.RemoveProfileId();
                }
                else
                {
                    // handle FikaClient(s)
                }
            }

            NotificationManagerClass.DisplayMessageNotification($"Safehouse Enabled: {SafehouseEnabled}");
            InteractionHelper.RefreshPrompt();
        }

        private static CustomInteraction GetToggleSafehouseItemEnabledAction(string itemId)
        {
            return new CustomInteraction(
                () =>
                {
                    var safehouseItem = ModSession.Instance.GetSafehouseItemOrNull(itemId);

                    if (safehouseItem.SafehouseEnabled)
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
                    var safehouseItem = ModSession.Instance.GetSafehouseItemOrNull(itemId);
                    if (safehouseItem.SafehouseEnabled) return false; //always enable interaction when safehouse is enabled so that it can be disabled
                    return !ModSession.Instance.SafehouseEnableAllowed;
                },
                () =>
                {
                    var safehouseItem = ModSession.Instance.GetSafehouseItemOrNull(itemId);
                    safehouseItem.SetSafehouseEnabled(!safehouseItem.SafehouseEnabled);
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
                    if (ModSession.Instance.CustomSafehouseExfil.ExfilIsEnabled)
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
                    var safehouseItem = ModSession.Instance.GetSafehouseItemOrNull(itemId);
                    return !safehouseItem.SafehouseEnabled;
                },
                () =>
                {
                    var exfil = ModSession.Instance.CustomSafehouseExfil;
                    exfil.SetCustomExfilEnabled(!exfil.ExfilIsEnabled);

                    if (exfil.ExfilIsEnabled)
                    {
                        exfil.gameObject.transform.position = ModSession.Instance.Player.gameObject.transform.position;
                        exfil.LastSafehouseItemThatUsedMe = ModSession.Instance.GetSafehouseItemOrNull(itemId);
                    }
                }
            );
        }
    }
}

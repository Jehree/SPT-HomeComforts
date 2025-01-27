using Comfort.Common;
using EFT.UI;
using HomeComforts.Components;
using HomeComforts.Fika;
using HomeComforts.Helpers;
using LeaveItThere.Common;
using LeaveItThere.Components;
using LeaveItThere.Helpers;
using UnityEngine;

namespace HomeComforts.Items.SpaceHeater
{
    internal class SpaceHeater : MonoBehaviour, IPhysicsTrigger
    {
        public string Description { get; } = "Space Heater";
        public FakeItem FakeItem;

        private Collider _collider;
        public bool AOEEnabled
        {
            get
            {
                return _collider.enabled;
            }
            set
            {
                _collider.enabled = value;
            }
        }

        public static void OnFakeItemInitialized(FakeItem fakeItem)
        {
            if (!Plugin.ServerConfig.SpaceHeaterItemIds.Contains(fakeItem.LootItem.Item.TemplateId)) return;

            SpaceHeater heater = AddSpaceHeaterBehavior(fakeItem);
            HCSession.Instance.SpaceHeaterSession.SpaceHeaters.Add(heater);
            fakeItem.OnPlacedStateChanged += heater.OnItemPlacedStateChanged;

            fakeItem.Actions.Add(GetToggleSpaceHeaterAction(fakeItem.ItemId));

            SpaceHeaterAddonData addonData = fakeItem.GetAddonDataOrNull<SpaceHeaterAddonData>(Plugin.AddonDataKey);
            if (addonData != null && addonData.HeaterEnabled)
            {
                heater.AOEEnabled = true;
            }
        }

        private void OnItemPlacedStateChanged(bool isPlaced)
        {
            if (isPlaced) return;
            HCSession.Instance.SpaceHeaterSession.SpaceHeaters.Remove(this);
            AOEEnabled = false;
        }

        public void OnTriggerEnter(Collider collider)
        {
            if (HCSession.Instance.GameWorld.GetPlayerByCollider(collider) != HCSession.Instance.Player) return;

            // add buff if player wasn't in any zone prior to this one
            if (HCSession.Instance.SpaceHeaterSession.PlayerIsInSpaceHeaterZone == false)
            {
                HCSession.Instance.NeedsRateReductions.SetEnabled(true);
                NotificationManagerClass.DisplayMessageNotification("Comfort Buff Active!");
            }

            HCSession.Instance.SpaceHeaterSession.AddSpaceHeaterIdToPlayerIsIn(FakeItem.ItemId);
        }

        public void OnTriggerExit(Collider collider)
        {
            if (HCSession.Instance.GameWorld.GetPlayerByCollider(collider) != HCSession.Instance.Player) return;

            HCSession.Instance.SpaceHeaterSession.RemoveSpaceHeaterIdFromPlayerIsIn(FakeItem.ItemId);

            // remove buff if after exiting the zone the player is still not in any other zone
            if (HCSession.Instance.SpaceHeaterSession.PlayerIsInSpaceHeaterZone == false)
            {
                HCSession.Instance.NeedsRateReductions.SetEnabled(false);
                NotificationManagerClass.DisplayWarningNotification("Comfort Buff Removed.");
            }
        }

        public static SpaceHeater AddSpaceHeaterBehavior(FakeItem fakeItem)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.name = fakeItem.name + "_spaceheater_LITKeepLayer";
            obj.layer = LayerMask.NameToLayer("Triggers");
            obj.transform.localScale = Vector3.one * Settings.SpaceHeaterAOESizeMultiplier.Value;
            obj.GetComponent<Renderer>().enabled = false;

            SphereCollider collider = obj.GetComponent<SphereCollider>();
            collider.isTrigger = true;

            SpaceHeater heater = obj.AddComponent<SpaceHeater>();
            heater.Init(fakeItem, collider);
            return heater;
        }

        private void Init(FakeItem fakeItem, SphereCollider collider)
        {
            FakeItem = fakeItem;
            gameObject.transform.SetParent(fakeItem.gameObject.transform);
            gameObject.transform.position = fakeItem.gameObject.transform.position;
            _collider = collider;
            AOEEnabled = false;
        }

        private static CustomInteraction GetToggleSpaceHeaterAction(string itemId)
        {
            return new CustomInteraction(
                () =>
                {
                    if (HCSession.Instance.SpaceHeaterSession.SpaceHeaterIsEnabled(itemId))
                    {
                        return "Turn Off";
                    }
                    else
                    {
                        return "Turn On";
                    }
                },
                false,
                () =>
                {
                    var heater = HCSession.Instance.SpaceHeaterSession.GetSpaceHeaterOrNull(itemId);
                    heater.AOEEnabled = !heater.AOEEnabled;

                    if (heater.AOEEnabled)
                    {
                        Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.GeneratorTurnOn);
                    }
                    else
                    {
                        Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.GeneratorTurnOff);
                    }

                    InteractionHelper.RefreshPrompt();
                    FikaInterface.SendSpaceHeaterStatePacket(heater.AOEEnabled, itemId);
                    heater.FakeItem.PutAddonData(Plugin.AddonDataKey, SpaceHeaterAddonData.CreateData(heater.AOEEnabled));
                }
            );
        }
    }
}

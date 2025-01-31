using Comfort.Common;
using EFT.InventoryLogic;
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

            fakeItem.Actions.Add(new ToggleSpaceHeaterInteraction(fakeItem, heater));

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

        public class ToggleSpaceHeaterInteraction(FakeItem fakeItem, SpaceHeater heater) : CustomInteraction(fakeItem)
        {
            public SpaceHeater Heater { get; private set; } = heater;
            public override string Name => Heater.AOEEnabled
                                                    ? "Turn Off"
                                                    : "Turn On";
            public override bool AutoPromptRefresh => true;
            public override void OnInteract()
            {
                Heater.AOEEnabled = !Heater.AOEEnabled;

                if (Heater.AOEEnabled)
                {
                    Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.GeneratorTurnOn);
                }
                else
                {
                    Singleton<GUISounds>.Instance.PlayUISound(EUISoundType.GeneratorTurnOff);
                }

                FikaInterface.SendSpaceHeaterStatePacket(Heater.AOEEnabled, FakeItem.ItemId);
                Heater.FakeItem.PutAddonData(Plugin.AddonDataKey, SpaceHeaterAddonData.CreateData(Heater.AOEEnabled));
            }
        }
    }
}

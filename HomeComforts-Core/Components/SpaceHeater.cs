using HomeComforts.Helpers;
using LeaveItThere.Common;
using LeaveItThere.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HomeComforts.Components
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
            
            var heater = AddSpaceHeaterBehavior(fakeItem);
            ModSession.Instance.SpaceHeater.SpaceHeaters.Add(heater);
            fakeItem.OnPlacedStateChanged += heater.OnItemPlacedStateChanged;
        }

        private void OnItemPlacedStateChanged(bool isPlaced)
        {
            if (isPlaced) return;
            ModSession.Instance.SpaceHeater.SpaceHeaters.Remove(this);
        }

        public void OnTriggerEnter(Collider collider)
        {
            if (ModSession.Instance.GameWorld.GetPlayerByCollider(collider) != ModSession.Instance.Player) return;

            // add buff if player wasn't in any zone prior to this one
            if (ModSession.Instance.SpaceHeater.PlayerIsInSpaceHeaterZone == false)
            {
                ModSession.Instance.NeedsRateReductions.SetEnabled(true);
                NotificationManagerClass.DisplayMessageNotification("Comfort Buff Active!");
            }

            ModSession.Instance.SpaceHeater.AddSpaceHeaterIdToPlayerIsIn(FakeItem.ItemId);
        }

        public void OnTriggerExit(Collider collider)
        {
            if (ModSession.Instance.GameWorld.GetPlayerByCollider(collider) != ModSession.Instance.Player) return;

            ModSession.Instance.SpaceHeater.RemoveSpaceHeaterIdFromPlayerIsIn(FakeItem.ItemId);

            // remove buff if after exiting the zone the player is still not in any other zone
            if (ModSession.Instance.SpaceHeater.PlayerIsInSpaceHeaterZone == false)
            {
                ModSession.Instance.NeedsRateReductions.SetEnabled(false);
                NotificationManagerClass.DisplayWarningNotification("Comfort Buff Removed.");
            }
        }

        public static SpaceHeater AddSpaceHeaterBehavior(FakeItem fakeItem)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.name = fakeItem.name + "_spaceheater";
            obj.layer = LayerMask.NameToLayer("Triggers");
            obj.transform.localScale = Vector3.one * Settings.SpaceHeaterAOESizeMultiplier.Value;
            obj.GetComponent<Renderer>().enabled = false;

            SphereCollider collider = obj.GetComponent<SphereCollider>();
            collider.isTrigger = true;

            var heater = obj.AddComponent<SpaceHeater>();
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
                    if (ModSession.Instance.SpaceHeater.SpaceHeaterIsEnabled(itemId))
                    {
                        return "Disable";
                    }
                    else
                    {
                        return "Enable";
                    }
                },
                false,
                () =>
                {
                    var heater = ModSession.Instance.SpaceHeater.GetSpaceHeaterOrNull(itemId);
                    heater.AOEEnabled = !heater.AOEEnabled;
                }
            );
        }
    }
}

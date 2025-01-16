using Comfort.Common;
using EFT;
using HomeComforts.Helpers;
using LeaveItThere.Common;
using LeaveItThere.Helpers;
using System;
using System.Collections;
using UnityEngine;

namespace HomeComforts.Components
{
    internal class SafehouseAreaOfAffect : MonoBehaviour, IPhysicsTrigger
    {
        public string Description { get; } = "Safehouse AOE Trigger";
        public SafehouseItem SafehouseItem { get; private set; }
        public bool AOEEnabled
        {
            get
            {
                return Collider.enabled;
            }
            set
            {
                Collider.enabled = value;
            }
        }

        private SphereCollider Collider;

        private Renderer _innerRenderer;
        private Renderer _outerRenderer;
        public bool AOEBoundsVisible
        {
            get
            {
                return _outerRenderer.enabled;
            }
            set
            {
                _innerRenderer.enabled = value;
                _outerRenderer.enabled = value;
            }
        }

        public void OnTriggerEnter(Collider collider)
        {
            if (ModSession.Instance.GameWorld.GetPlayerByCollider(collider) == ModSession.Instance.Player)
            {
                ModSession.Instance.NeedsRateReductions.SetEnabled(true);
                NotificationManagerClass.DisplayMessageNotification("Safehouse Zone Entered!");
            }
        }

        public void OnTriggerExit(Collider collider)
        {
            if (ModSession.Instance.GameWorld.GetPlayerByCollider(collider) == ModSession.Instance.Player)
            {
                ModSession.Instance.NeedsRateReductions.SetEnabled(false);
                NotificationManagerClass.DisplayWarningNotification("Safehouse Zone Exited.");
            }
        }

        private void Init(SafehouseItem safehouseItem, SphereCollider collider)
        {
            SafehouseItem = safehouseItem;
            gameObject.transform.SetParent(safehouseItem.gameObject.transform);
            gameObject.transform.position = safehouseItem.gameObject.transform.position;
            Collider = collider;
            AOEEnabled = false;
        }

        public static SafehouseAreaOfAffect ApplyAOEObjectToSafehouseItem(SafehouseItem safehouseItem)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.name = safehouseItem.name + "_AOE";
            obj.layer = LayerMask.NameToLayer("Triggers");
            obj.transform.localScale = Vector3.one * Settings.SafehouseAOESizeMultiplier.Value;

            var renderer = obj.GetComponent<Renderer>();
            renderer.material.color = Color.magenta;

            // create inverted copy so that it can be seen from inside the sphere as well
            GameObject innerObj = GameObject.Instantiate(obj);
            innerObj.transform.SetParent(obj.transform);
            innerObj.GetComponent<SphereCollider>().enabled = false;
            var oldInnerMeshFilter = innerObj.GetComponent<MeshFilter>();
            var newInnerMesh = Utils.GetInvertedMesh(oldInnerMeshFilter.mesh);
            oldInnerMeshFilter.mesh = newInnerMesh;

            SphereCollider collider = obj.GetComponent<SphereCollider>();
            collider.isTrigger = true;

            var aoe = obj.AddComponent<SafehouseAreaOfAffect>();
            aoe.Init(safehouseItem, collider);
            aoe._outerRenderer = renderer;
            aoe._innerRenderer = innerObj.GetComponent<Renderer>();
            aoe.AOEBoundsVisible = false;
            return aoe;
        }

        public static CustomInteraction GetToggleAOEBoundsVisibleAction(string itemId)
        {
            return new CustomInteraction(
                () =>
                {
                    var safehouseItem = ModSession.Instance.GetSafehouseItemOrNull(itemId);

                    return $"AOE Bounds Visible: {safehouseItem.AOE.AOEBoundsVisible}";
                },
                () =>
                {
                    return !ModSession.Instance.GetSafehouseItemOrNull(itemId).SafehouseEnabled;
                },
                () =>
                {
                    var safehouseItem = ModSession.Instance.GetSafehouseItemOrNull(itemId);
                    safehouseItem.AOE.AOEBoundsVisible = !safehouseItem.AOE.AOEBoundsVisible;
                    InteractionHelper.RefreshPrompt();
                }
            );
        }
    }
}

using EFT;
using EFT.Interactive;
using HomeComforts.Items.Safehouse;
using UnityEngine;

namespace HomeComforts.Components
{
    internal class SafehouseExfil : ExfiltrationPoint
    {
        // need this here to avoid an error during an SPT patch that sets it via reflection
        private bool _authorityToChangeStatusExternally = false;

        public Safehouse LastSafehouseThatUsedMe = null;
        public BoxCollider Collider { get; private set; }
        public bool ExfilIsEnabled
        {
            get
            {
                return Collider.enabled;
            }
        }

        public static SafehouseExfil Create(string name)
        {
            GameObject obj = new()
            {
                name = name,
                layer = LayerMask.NameToLayer("Triggers")
            };

            BoxCollider collider = obj.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.size = new Vector3(1, 1, 1) * Helpers.Settings.ExfilSizeMultiplier.Value;
            collider.enabled = false;

            obj.transform.position = new Vector3(0, -999999, 0);

            SafehouseExfil exfil = obj.AddComponent<SafehouseExfil>();
            exfil.Collider = collider;
            // despite this being a 'setting', the LoadSettings call where LocationExitClass is passed in does not set it for us, so we have to do it here
            exfil.Settings.Name = name;

            return exfil;
        }

        private LocationExitClass GetSettings()
        {
            return new LocationExitClass
            {
                Name = Settings.Name,
                PassageRequirement = ERequirementState.None,
                EventAvailable = false,
                // We could add the EntryPoints here, but that would make the exfil show up in the available exfils list.
                // Instead, we add them at the time that the exfil is enabled. This works as expected but keeps the exfil name out
                // of the available exfils list.
                EntryPoints = string.Join(",", Plugin.AllEntryPoints),
                ExfiltrationType = EExfiltrationType.Individual,
                ExfiltrationTime = 7f,
                PlayersCount = 0,
                Chance = 100,
                MinTime = 0f,
                MaxTime = 0f,
                RequirementTip = "",
            };
        }

        public void InitCustomExfil()
        {
            LoadSettings(MongoID.Generate(), GetSettings(), true);
        }

        public void SetCustomExfilEnabled(bool enabled)
        {
            Collider.enabled = enabled;

            if (enabled && EligibleEntryPoints.IsNullOrEmpty())
            {
                EligibleEntryPoints = [HCSession.Instance.Player.Profile.Info.EntryPoint.ToLower()];
            }
        }
    }
}

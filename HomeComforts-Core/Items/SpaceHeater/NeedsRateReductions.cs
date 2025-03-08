using Comfort.Common;
using EFT;
using EFT.HealthSystem;
using HomeComforts.Components;
using HomeComforts.Helpers;
using System.Collections;
using UnityEngine;

namespace HomeComforts.Items.SpaceHeater
{
    public class NeedsRateReductions
    {
        public bool Enabled { get; private set; } = false;
        private Coroutine _routine = null;

        // the reason we divide these by four is to soften the case where the player is in and out of the safe zone somewhat frequently, causing them to never get a tick of the buff
        // this happens because, previously, the tick only occured if the player was inside the zone for an entire 60 seconds. If the player exited at 55 seconds and re-entered, they wouldn't experience a tick
        // I think this behavior is fine and keeps the heater balanced.. but I think needing to be inside the zone for 60 seconds is a bit harsh. 15 is more fair.
        private float _hydrationBuffPerTick = Settings.SpaceHeaterHydrationBuff.Value / 4;
        private float _energyBuffPerTick = Settings.SpaceHeaterEnergyBuff.Value / 4;
        private float _hydrationBuffPerSecond = Settings.SpaceHeaterHydrationBuff.Value;
        private float _energyBuffPerSecond = Settings.SpaceHeaterEnergyBuff.Value;

        private WaitForSeconds _waitFor15Seconds = new(15);

        public void SetEnabled(bool enabled)
        {
            if (Enabled == enabled) return;
            Enabled = enabled;
            ActiveHealthController controller = HCSession.Instance.Player.ActiveHealthController;

            if (Enabled)
            {
                // these are visual only
                controller.HydrationRate += _hydrationBuffPerSecond;
                controller.EnergyRate += _energyBuffPerSecond;

                // this does the actual buff ticking
                _routine = StaticManager.BeginCoroutine(BuffRoutine());
            }
            else
            {
                controller.HydrationRate -= _hydrationBuffPerSecond;
                controller.EnergyRate -= _energyBuffPerSecond;

                if (_routine != null)
                {
                    StaticManager.KillCoroutine(_routine);
                    _routine = null;
                }
            }
        }

        private IEnumerator BuffRoutine()
        {
            while (true)
            {
                yield return _waitFor15Seconds;

                if (!Singleton<GameWorld>.Instantiated)
                {
                    yield break;
                }

                HCSession.Instance.Player.ActiveHealthController.ChangeHydration(_hydrationBuffPerTick);
                HCSession.Instance.Player.ActiveHealthController.ChangeEnergy(_energyBuffPerTick);
            }
        }
    }
}

using Comfort.Common;
using EFT;
using HomeComforts.Components;
using HomeComforts.Helpers;
using System;
using System.Collections;
using UnityEngine;

public class NeedsRateReductions
{
    public bool Enabled { get; private set; } = false;
    private Coroutine _routine = null;
    private WaitForSeconds _waitFor60Seconds = new(60);
    private float _espilon = 0.000001f;

    private float _hydrationBuff = Settings.HydrationFullBuff.Value * Settings.SafehouseHydrationDrainReduction.Value;
    private float _energyBuff = Settings.EnergyFullBuff.Value * Settings.SafehouseEnergyDrainReduction.Value;

    public void SetEnabled(bool enabled)
    {
        if (Enabled == enabled) return;
        Enabled = enabled;
        var controller = ModSession.Instance.Player.ActiveHealthController;

        if (Enabled)
        {
            controller.HydrationRate += _hydrationBuff;
            controller.EnergyRate += _energyBuff;

            _routine = StaticManager.BeginCoroutine(BuffRoutine());
        }
        else
        {
            controller.HydrationRate -= _hydrationBuff;
            controller.EnergyRate -= _energyBuff;

            if (_routine != null)
            {
                StaticManager.KillCoroutine(_routine);
                _routine = null;
            }
        }

        if (Math.Abs(controller.HydrationRate) < _espilon)
        {
            controller.HydrationRate = 0;
        }

        if (Math.Abs(controller.EnergyRate) < _espilon)
        {
            controller.EnergyRate = 0;
        }
    }

    private IEnumerator BuffRoutine()
    {
        while (true)
        {
            yield return _waitFor60Seconds;

            if (!Singleton<GameWorld>.Instantiated)
            {
                yield break;
            }

            ModSession.Instance.Player.ActiveHealthController.ChangeHydration(_hydrationBuff);
            ModSession.Instance.Player.ActiveHealthController.ChangeEnergy(_energyBuff);
        }
    }
}
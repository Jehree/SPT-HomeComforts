using EFT;
using HomeComforts.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HomeComforts.Helpers;

internal class Utils
{
    public static void ForceUpdatePlayerCollisions()
    {
        StaticManager.BeginCoroutine(ForceUpdatePlayerCollisionsRoutine());
    }

    private static IEnumerator ForceUpdatePlayerCollisionsRoutine()
    {
        HCSession.Instance.Player.gameObject.transform.position += new Vector3(0, 0.00001f, 0);
        yield return null;
        HCSession.Instance.Player.gameObject.transform.position -= new Vector3(0, 0.00001f, 0);
    }
}

﻿using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace RATS.LegendaryEffectWorkers;

public class JunkiesWorker : LegendaryEffectWorker
{
    public override void ApplyEffect(ref DamageInfo damageInfo, Pawn pawn)
    {
        if (pawn != null)
        {
            int addictions = pawn.health.hediffSet.hediffs.Count(hediff => hediff.def.defName.ToLower().Contains("addict"));

            float extraDamage = addictions * 0.15f + 1f;

            Type dType = typeof(DamageInfo);
            FieldInfo amountInt = dType.GetField("amountInt", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (amountInt != null)
            {
                float damageAmount = (float)amountInt.GetValue(damageInfo);
                amountInt.SetValueDirect(__makeref(damageInfo), damageAmount * extraDamage);
            }
        }
    }
}

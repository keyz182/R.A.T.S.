using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace RATS.LegendaryEffectWorkers;

public class StatPart_Rapid : StatPart
{
    private float multiplier = 1f;

    public override void TransformValue(StatRequest req, ref float val)
    {
        int count = ActiveFor(req.Thing);

        val *= Mathf.Pow(multiplier, count);
    }

    public override string ExplanationPart(StatRequest req)
    {
        int count = ActiveFor(req.Thing);
        if (count <= 0)
            return null;

        return "Rapid (Legendary Effect): (" + multiplier.ToString("0.00" + "%") + "^" + count + ") -> " + Mathf.Pow(multiplier, count).ToString("0.00" + "%");
    }

    private int ActiveFor(Thing t)
    {
        if (t is Pawn pawn)
        {
            int count = 0;
            if (pawn.apparel != null)
            {
                IEnumerable<Apparel> legendaryApparel = pawn.apparel.WornApparel.Where(app => app.TryGetQuality(out var quality) && quality == QualityCategory.Legendary);
                foreach (Apparel apparel in legendaryApparel)
                {
                    if (LegendaryEffectGameTracker.EffectsForThing(apparel, out List<LegendaryEffectDef> effects))
                    {
                        count += effects.Count(eff => eff == RATS_DefOf.Rats_LegendaryEffect_Rapid);
                    }
                }
            }

            if (pawn.equipment is { Primary: not null })
            {
                if (LegendaryEffectGameTracker.EffectsForThing(pawn.equipment.Primary, out List<LegendaryEffectDef> effects))
                {
                    count += effects.Count(eff => eff == RATS_DefOf.Rats_LegendaryEffect_Rapid);
                }
            }
            return count;
        }

        if (!LegendaryEffectGameTracker.EffectsForThing(t, out List<LegendaryEffectDef> effectsForT))
        {
            return 0;
        }

        return effectsForT.Count;
    }
}

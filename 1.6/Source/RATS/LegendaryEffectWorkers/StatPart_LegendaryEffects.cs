using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace RATS.LegendaryEffectWorkers;

public class StatPart_LegendaryEffects : StatPart
{
    public override void TransformValue(StatRequest req, ref float val)
    {
        if (req.Thing is Pawn pawn)
        {
            foreach (LegendaryEffectGameTracker.ThingAndEffect thingAndEffect in LegendaryEffectGameTracker.EffectsForPawn(pawn))
            {
                val =
                    thingAndEffect.effect.StatFactors.Where(sf => sf.stat == parentStat).Aggregate(val, (current, statModifier) => current * statModifier.value)
                    + thingAndEffect.effect.StatOffsets.Where(sf => sf.stat == parentStat).Sum(statModifier => statModifier.value);
            }
        }
        else if (req.Thing != null)
        {
            if (LegendaryEffectGameTracker.EffectsForThing(req.Thing, out List<LegendaryEffectDef> effects))
            {
                foreach (LegendaryEffectDef legendaryEffectDef in effects)
                {
                    val =
                        legendaryEffectDef.StatFactors.Where(sf => sf.stat == parentStat).Aggregate(val, (current, statModifier) => current * statModifier.value)
                        + legendaryEffectDef.StatOffsets.Where(sf => sf.stat == parentStat).Sum(statModifier => statModifier.value);
                }
            }
        }
    }

    public override string ExplanationPart(StatRequest req)
    {
        StringBuilder sb = new StringBuilder();
        if (req.Thing is Pawn pawn)
        {
            foreach (LegendaryEffectGameTracker.ThingAndEffect thingAndEffect in LegendaryEffectGameTracker.EffectsForPawn(pawn))
            {
                foreach (StatModifier effectStatOffset in thingAndEffect.effect.StatOffsets.Where(sf => sf.stat == parentStat))
                {
                    sb.AppendLine($"{thingAndEffect.effect.LabelCap} (Legendary Effect): {effectStatOffset.ValueToStringAsOffset}");
                }
                foreach (StatModifier effectStatFactor in thingAndEffect.effect.StatFactors.Where(sf => sf.stat == parentStat))
                {
                    sb.AppendLine($"{thingAndEffect.effect.LabelCap} (Legendary Effect): {effectStatFactor.ToStringAsFactor}");
                }
            }
        }
        else if (req.Thing != null)
        {
            if (LegendaryEffectGameTracker.EffectsForThing(req.Thing, out List<LegendaryEffectDef> effects))
            {
                foreach (LegendaryEffectDef legendaryEffectDef in effects)
                {
                    foreach (StatModifier effectStatOffset in legendaryEffectDef.StatOffsets.Where(sf => sf.stat == parentStat))
                    {
                        sb.AppendLine($"{legendaryEffectDef.LabelCap} (Legendary Effect): {effectStatOffset.ValueToStringAsOffset}");
                    }

                    foreach (StatModifier effectStatFactor in legendaryEffectDef.StatFactors.Where(sf => sf.stat == parentStat))
                    {
                        sb.AppendLine($"{legendaryEffectDef.LabelCap} (Legendary Effect): {effectStatFactor.ToStringAsFactor}");
                    }
                }
            }
        }
        else
        {
            return null;
        }

        return sb.ToString();
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using RimWorld;
using Verse;

namespace RATS;

public class LegendaryEffect : IExposable
{
    public Thing Owner = null;
    public List<LegendaryEffectDef> Effects = [];

    public LegendaryEffect() { }

    public LegendaryEffect(Thing owner)
    {
        Owner = owner;
    }

    public LegendaryEffect(Thing owner, List<LegendaryEffectDef> effects)
    {
        Owner = owner;
        Effects = effects;
    }

    public void ExposeData()
    {
        Scribe_References.Look(ref Owner, "Owner");
        Scribe_Collections.Look(ref Effects, "Effects", LookMode.Def);
    }
}

public class LegendaryEffectGameTracker : GameComponent
{
    public LegendaryEffectGameTracker(Game game) { }

    public static List<LegendaryEffect> Effects = new();

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref Effects, "Effects", LookMode.Deep);
    }

    public static bool EffectsForThing(Thing thing, out List<LegendaryEffectDef> effects)
    {
        effects = [];
        LegendaryEffect le = Effects.FirstOrFallback(le => le.Owner == thing);
        if (le == null)
            return false;
        effects = le.Effects;
        return true;
    }

    public static void SetEffectsForThing(Thing thing, List<LegendaryEffectDef> effects)
    {
        LegendaryEffect le = Effects.FirstOrDefault(le => le.Owner == thing);
        if (le == null)
        {
            le = new LegendaryEffect(thing);
            Effects.Add(le);
        }
        le.Effects = effects;
    }

    public static void AddNewLegendaryEffectFor(Thing thing)
    {
        List<LegendaryEffectDef> AllDefs = DefDatabase<LegendaryEffectDef>.AllDefsListForReading;
        LegendaryEffectDef effect;

        if (thing.def.IsApparel)
        {
            effect = AllDefs.Where(def => def.IsForApparel).RandomElement();
        }
        else if (thing.def.IsWeapon)
        {
            if (thing.def.weaponClasses.Any(cls => cls.defName.ToLower().Contains("melee")))
            {
                effect = AllDefs.Where(def => def.IsForWeapon && def.IsForMelee).RandomElement();
            }
            else
            {
                effect = AllDefs.Where(def => def.IsForWeapon && !def.IsForMelee).RandomElement();
            }
        }
        else
        {
            return;
        }

        if (!EffectsForThing(thing, out List<LegendaryEffectDef> effects))
            effects = new List<LegendaryEffectDef>();

        effects.Add(effect);

        SetEffectsForThing(thing, effects);
    }

    public static void ClearEffectsFor(Thing thing)
    {
        if (!EffectsForThing(thing, out List<LegendaryEffectDef> effects))
            effects = new List<LegendaryEffectDef>();

        effects.Clear();

        SetEffectsForThing(thing, effects);
    }

    public static bool HasEffect(Thing thing)
    {
        return Effects.Any(e => e.Owner == thing && !e.Effects.NullOrEmpty());
    }

    public static void Reroll(Thing thing)
    {
        if (!HasEffect(thing))
            return;

        ClearEffectsFor(thing);
        AddNewLegendaryEffectFor(thing);
    }

    public static string GetEffectDescription(Thing thing)
    {
        if (!HasEffect(thing))
        {
            return null;
        }

        StringBuilder body = new StringBuilder();
        if (EffectsForThing(thing, out List<LegendaryEffectDef> effects))
        {
            foreach (LegendaryEffectDef effect in effects)
            {
                body.AppendLine($" - {effect.LabelCap} - {effect.description}");
            }
        }

        return RemoveEmptyLines(body.ToString());
    }

    public static string RemoveEmptyLines(string lines)
    {
        return Regex.Replace(lines, @"^\s*$\n|\r", string.Empty, RegexOptions.Multiline).TrimEnd();
    }

    public static void MakeChangeEffectFloatMenu(Thing thing)
    {
        if (!EffectsForThing(thing, out List<LegendaryEffectDef> effects))
            effects = [];

        List<LegendaryEffectDef> AllDefs = DefDatabase<LegendaryEffectDef>.AllDefsListForReading;
        List<FloatMenuOption> options = [];
        IEnumerable<LegendaryEffectDef> validEffects;
        if (thing.def.IsApparel)
        {
            validEffects = AllDefs.Where(def => def.IsForApparel);
        }
        else if (thing.def.IsWeapon)
        {
            validEffects = thing.def.weaponClasses.Any(cls => cls.defName.ToLower().Contains("melee"))
                ? AllDefs.Where(def => def.IsForWeapon && def.IsForMelee)
                : AllDefs.Where(def => def.IsForWeapon);
        }
        else
        {
            return;
        }

        foreach (LegendaryEffectDef effect in validEffects.Where(eff => !effects.Contains(eff)))
        {
            options.Add(
                new FloatMenuOption(
                    effect.LabelCap,
                    () =>
                    {
                        effects.Clear();
                        effects.Add(effect);
                    }
                )
            );
        }
        Find.WindowStack.Add(new FloatMenu(options));
        SetEffectsForThing(thing, effects);
    }

    public struct ThingAndEffect
    {
        public Thing thing;
        public LegendaryEffectDef effect;
    }

    public static List<ThingAndEffect> EffectsForPawn(Pawn pawn)
    {
        List<ThingAndEffect> output = [];

        if (pawn == null)
            return output;

        if (pawn.apparel != null)
        {
            foreach (Apparel apparel in pawn.apparel.UnlockedApparel)
            {
                if (EffectsForThing(apparel, out List<LegendaryEffectDef> effects))
                    output.AddRange(effects.Select(eff => new ThingAndEffect { thing = apparel, effect = eff }));
            }
        }

        if (pawn.equipment != null && pawn.equipment.Primary != null)
        {
            if (EffectsForThing(pawn.equipment.Primary, out List<LegendaryEffectDef> effects))
                output.AddRange(effects.Select(eff => new ThingAndEffect { thing = pawn.equipment.Primary, effect = eff }));
        }

        return output;
    }

    public static float CooldownModifier(Pawn pawn)
    {
        float modifier = 1f;

        foreach (ThingAndEffect thingAndEffect in EffectsForPawn(pawn))
        {
            modifier *= thingAndEffect.effect.RATS_Multiplier;
        }

        return modifier;
    }
}

using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace RATS;

public class RATS_Settings : ModSettings
{
    public bool EnableSlowDownTime = true;
    public bool EnableZoom = true;
    public int ZoomTimeout = 150;
    public int CooldownTicks = 600;
    public float ClampingFactor = 0.5f;
    public float GainPerLevel = 0.15f;
    public Vector2 scrollPosition = Vector2.zero;

    public Dictionary<string, float> MultiplierLookup = new Dictionary<string, float>();

    public void ResetMultipliers()
    {
        MultiplierLookup = new Dictionary<string, float>
        {
            { "Leg", 1.4f },
            { "Eye", 0.6f },
            { "Shoulder", 0.9f },
            { "Arm", 1.3f },
            { "Hand", 0.8f },
            { "Head", 0.7f },
            { "Lung", 0.95f },
            { "Torso", 1.5f },
            { "Heart", 0.8f }
        };

        PopulateMissingMultipliers();
    }

    public void PopulateMissingMultipliers()
    {
        if (MultiplierLookup.Count() >= DefDatabase<BodyPartDef>.AllDefs.Count())
            return;
        IEnumerable<BodyPartDef> defs = DefDatabase<BodyPartDef>.AllDefs.Where(def =>
            !MultiplierLookup.Keys.Contains(def.defName)
        );

        foreach (BodyPartDef bodyPartDef in defs)
        {
            MultiplierLookup[bodyPartDef.defName] = 1.0f;
        }
    }

    public float MaxGain =>
        1f
        + Enumerable
            .Range(0, 20)
            .Sum(i => i == 0 ? 0 : Mathf.Pow(RATSMod.Settings.GainPerLevel, i));

    public float GetClampedValue(int level)
    {
        return MaxGain / ClampingFactor * GetScaledValue(level);
    }

    public float GetScaledValue(int level)
    {
        return Enumerable
            .Range(0, level)
            .Sum(i => i == 0 ? 0 : Mathf.Pow(RATSMod.Settings.GainPerLevel, i));
    }

    public void DoWindowContents(Rect wrect)
    {
        PopulateMissingMultipliers();
        int multiplierHeight = MultiplierLookup.Count * 56;
        int restHeight = 388;
        float scrollViewHeight = multiplierHeight + restHeight; // Adjust this value as needed
        Rect viewRect = new Rect(0, 0, wrect.width - 20, scrollViewHeight);
        scrollPosition = GUI.BeginScrollView(
            new Rect(0, 50, wrect.width, wrect.height - 50),
            scrollPosition,
            viewRect
        );
        Listing_Standard options = new Listing_Standard();
        options.Begin(viewRect);
        try
        {
            //30
            if (options.ButtonText("RATS_Settings_Reset".Translate()))
            {
                EnableSlowDownTime = true;
                EnableZoom = true;
                ZoomTimeout = 150;
                CooldownTicks = 600;
                GainPerLevel = 0.15f;
                ClampingFactor = 0.15f;
                ResetMultipliers();
            }
            //12
            options.Gap();

            //30
            options.CheckboxLabeled(
                "RATS_Settings_EnableSlowDownTime".Translate(),
                ref EnableSlowDownTime
            );
            //12
            options.Gap();
            //30

            options.CheckboxLabeled("RATS_Settings_EnableZoom".Translate(), ref EnableZoom);
            //12
            options.Gap();

            //22
            options.Label("RATS_Settings_ZoomTimeout".Translate(ZoomTimeout));
            //22
            options.IntAdjuster(ref ZoomTimeout, 1, 60);
            //12
            options.Gap();

            //22
            options.Label("RATS_Settings_CooldownTicks".Translate(CooldownTicks));
            //22
            options.IntAdjuster(ref CooldownTicks, 1, 60);
            //12
            options.Gap();

            //22
            options.Label(
                "RATS_Settings_Gain_Per_Level".Translate(GainPerLevel.ToString("F5")),
                tooltip: "RATS_Settings_Gain_Per_Level_Mouseover".Translate()
            );
            //30
            GainPerLevel = options.Slider(GainPerLevel, 0.01f, 4f);
            //12
            options.Gap();

            //22
            options.Label(
                "RATS_Settings_ClampingFactor".Translate(ClampingFactor.ToString("F5")),
                tooltip: "RATS_Settings_ClampingFactor".Translate()
            );
            //30
            ClampingFactor = options.Slider(ClampingFactor, 0.01f, 2f);
            //12
            options.Gap();

            //22
            options.Label("RATS_Settings_BodyPartMultipliers".Translate());

            foreach (string defName in MultiplierLookup.Keys.ToList())
            {
                BodyPartDef def = DefDatabase<BodyPartDef>.GetNamed(defName);
                if (def == null)
                    continue;
                string label = def.LabelCap;
                options.Label($"[{MultiplierLookup[defName]:F5}] {label} - {def.description}");
                MultiplierLookup[defName] = options.Slider(MultiplierLookup[defName], 0.01f, 10f);
                options.Gap();
            }
        }
        finally
        {
            options.End();
            GUI.EndScrollView();
        }
    }

    public override void ExposeData()
    {
        if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
        {
            ResetMultipliers();
        }
        Scribe_Values.Look(ref EnableSlowDownTime, "EnableSlowDownTime", true);
        Scribe_Values.Look(ref EnableZoom, "EnableZoom", true);
        Scribe_Values.Look(ref ZoomTimeout, "ZoomTimeout", 150);
        Scribe_Values.Look(ref CooldownTicks, "CooldownTicks", 150);
        Scribe_Values.Look(ref GainPerLevel, "GainPerLevel", 0.15f);
        Scribe_Values.Look(ref ClampingFactor, "ClampingFactor", 0.15f);
        Scribe_Collections.Look(
            ref MultiplierLookup,
            "MultiplierLookup",
            LookMode.Value,
            LookMode.Value
        );
    }
}

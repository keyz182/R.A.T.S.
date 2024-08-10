﻿using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace RATS;

public class Dialog_RATS(Verb_AbilityRats verb, LocalTargetInfo target, IWindowDrawing customWindowDrawing = null) : Window(customWindowDrawing)
{
    private readonly Color ButtonTextColour = new Color(0.357f, 0.825f, 0.278f);

    private readonly Texture2D Logo = ContentFinder<Texture2D>.Get("UI/RATS_Logo-Small");

    private readonly Verb_AbilityRats verb = verb;

    public Verb Selected;
    private LocalTargetInfo target = target;
    private string title = "R.A.T.S.";

    public Dictionary<string, float> MultiplierLookup => RATSMod.Settings.MultiplierLookup;

    public override Vector2 InitialSize => new Vector2(845f, 740f);
    protected virtual Vector2 ButtonSize => new Vector2(200f, 40f);
    public virtual TaggedString OkButtonLabel => "OK".Translate();

    public virtual TaggedString CancelButtonLabel => "CancelButton".Translate();
    public virtual TaggedString WarningText => "";

    public float GetPartMultiplier(BodyPartDef def)
    {
        return MultiplierLookup.GetWithFallback(def.defName, 1.0f);
    }

    public virtual void DoButtonRow(ref RectDivider layout)
    {
        RectDivider rectDivider1 = layout.NewRow(ButtonSize.y, VerticalJustification.Bottom, 0.0f);
        RectDivider rectDivider3 = rectDivider1.NewCol(ButtonSize.x);
        RectDivider rectDivider4 = rectDivider1.NewCol(rectDivider1.Rect.width, HorizontalJustification.Right);

        if (Widgets.ButtonText(rectDivider3, CancelButtonLabel))
        {
            Cancel();
        }

        Widgets.Label(rectDivider4, WarningText);
    }

    public virtual void DoRATS(ref RectDivider layout)
    {
        ShotReport shotReport = verb.GetShotReport();

        // Calculate the multiplier, 1 + the sum of level*multiplier over the range 0 - shooting level
        float pawnMultiplier = RATSMod.Settings.GetClampedValue(verb.CasterPawn.skills.GetSkill(SkillDefOf.Shooting).Level);

        float esitmatedHitChance = shotReport.TotalEstimatedHitChance;
        esitmatedHitChance = Mathf.Clamp01(esitmatedHitChance * pawnMultiplier);

        using (new ProfilerBlock(nameof(DoRATS)))
        {
            using (TextBlock.Default())
            {
                List<BodyPartRecord> parts = target
                    .Pawn.health.hediffSet.GetNotMissingParts()
                    .Where(p => p.def == target.Pawn.def.race.body.corePart.def || p.parent?.def == target.Pawn.def.race.body.corePart.def)
                    .Where(p => p.coverageAbs > 0.0)
                    .ToList();

                int partCount = parts.Count;

                RectDivider rowBtn = layout.NewRow(600f);

                RectDivider colLeft = rowBtn.NewCol(100f);
                RectDivider colMid = rowBtn.NewCol(InitialSize.x - 300f);
                RectDivider colRight = rowBtn.NewCol(100f, HorizontalJustification.Right);

                RectDivider logoRect = colMid.NewRow(100f);

                GUI.DrawTexture(logoRect, Logo, ScaleMode.ScaleToFit);

                Rect portraitRect = colMid.Rect.ContractedBy(30f);
                RenderTexture texture = PortraitsCache.Get(
                    target.Pawn,
                    portraitRect.size,
                    Rot4.South,
                    new Vector3(0f, 0f, 0.1f),
                    1.5f,
                    healthStateOverride: PawnHealthState.Mobile
                );

                GUI.DrawTexture(portraitRect, texture);

                for (int i = 0; i < partCount; i++)
                {
                    RectDivider rectDivider;
                    rectDivider = i <= partCount / 2 ? colLeft.NewRow(45f, marginOverride: 5f) : colRight.NewRow(45f, marginOverride: 5f);

                    float partAccuracy = esitmatedHitChance * GetPartMultiplier(parts[i].def);
                    int partAccuracyPct = Mathf.CeilToInt(partAccuracy * 100);

                    if (!Widgets.ButtonText(rectDivider, $"{parts[i].LabelCap} [{partAccuracyPct}%]", false, true, ButtonTextColour))
                    {
                        continue;
                    }

                    verb.RATS_Selection(target, parts[i], partAccuracy, shotReport);
                    Find.TickManager.CurTimeSpeed = TimeSpeed.Normal;
                    Close();
                }
            }
        }
    }

    protected virtual void Start()
    {
        Close();
    }

    protected virtual void Cancel()
    {
        Close();
    }

    public override void DoWindowContents(Rect inRect)
    {
        using (TextBlock.Default())
        {
            RectDivider layout1 = new RectDivider(inRect, 145235235);
            layout1.NewRow(0.0f, VerticalJustification.Bottom, 1f);
            layout1.NewRow(0.0f);
            DoRATS(ref layout1);
            DoButtonRow(ref layout1);
            layout1.NewRow(0.0f, marginOverride: 20f);
            layout1.NewCol(20f, marginOverride: 0.0f);
        }
    }
}

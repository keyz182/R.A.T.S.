using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace RATS;

public class Dialog_RATS(
    Verb_AbilityRats verb,
    LocalTargetInfo target,
    IWindowDrawing customWindowDrawing = null
) : Window(customWindowDrawing)
{
    private string title = "R.A.T.S.";

    private Verb_AbilityRats verb = verb;
    private LocalTargetInfo target = target;
    private Color ButtonTextColour = new Color(0.357f, 0.825f, 0.278f);

    private Texture2D Logo = ContentFinder<Texture2D>.Get("UI/RATS_Logo-Small");

    public Verb Selected;

    public override Vector2 InitialSize => new Vector2(845f, 740f);
    protected virtual Vector2 ButtonSize => new Vector2(200f, 40f);
    public virtual TaggedString OkButtonLabel => "OK".Translate();

    public virtual TaggedString CancelButtonLabel => "CancelButton".Translate();
    public virtual TaggedString WarningText => "";

    public Dictionary<BodyPartDef, float> MultiplierLookup = new Dictionary<BodyPartDef, float>
    {
        { BodyPartDefOf.Leg, 1.4f },
        { BodyPartDefOf.Eye, 0.6f },
        { BodyPartDefOf.Shoulder, 0.9f },
        { BodyPartDefOf.Arm, 1.3f },
        { BodyPartDefOf.Hand, 0.8f },
        { BodyPartDefOf.Head, 0.7f },
        { BodyPartDefOf.Lung, 0.95f },
        { BodyPartDefOf.Torso, 1.5f },
        { BodyPartDefOf.Heart, 0.8f }
    };

    public float GetPartMultiplier(BodyPartDef def) => MultiplierLookup.GetWithFallback(def, 1.0f);

    public virtual void DoButtonRow(ref RectDivider layout)
    {
        RectDivider rectDivider1 = layout.NewRow(ButtonSize.y, VerticalJustification.Bottom, 0.0f);
        RectDivider rectDivider2 = rectDivider1.NewCol(
            ButtonSize.x,
            HorizontalJustification.Right,
            10f
        );
        RectDivider rectDivider3 = rectDivider1.NewCol(ButtonSize.x);
        RectDivider rectDivider4 = rectDivider1.NewCol(
            rectDivider1.Rect.width,
            HorizontalJustification.Right
        );

        if (Widgets.ButtonText(rectDivider3, CancelButtonLabel))
            Cancel();
        Widgets.Label(rectDivider4, WarningText);
    }

    public virtual void DoRATS(ref RectDivider layout)
    {
        ShotReport shotReport = verb.GetShotReport();

        // Calculate the multiplier, 1 + the sum of level*multiplier over the range 0 - shooting level
        float pawnMultiplier = RATSMod.Settings.GetClampedValue(
            verb.CasterPawn.skills.GetSkill(SkillDefOf.Shooting).Level
        );

        float esitmatedHitChance = shotReport.TotalEstimatedHitChance;
        esitmatedHitChance = Mathf.Clamp01(esitmatedHitChance * pawnMultiplier);

        using (new ProfilerBlock(nameof(DoRATS)))
        {
            using (TextBlock.Default())
            {
                List<BodyPartRecord> parts = target
                    .Pawn.health.hediffSet.GetNotMissingParts()
                    .Where(p =>
                        p.def == BodyPartDefOf.Torso || p.parent?.def == BodyPartDefOf.Torso
                    )
                    .ToList();

                int partCount = parts.Count();

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
                    if (i <= partCount / 2)
                    {
                        rectDivider = colLeft.NewRow(45f, marginOverride: 5f);
                    }
                    else
                    {
                        rectDivider = colRight.NewRow(45f, marginOverride: 5f);
                    }

                    float partAccuracy = esitmatedHitChance * GetPartMultiplier(parts[i].def);
                    int partAccuracyPct = Mathf.CeilToInt(partAccuracy * 100);

                    if (
                        Widgets.ButtonText(
                            rect: rectDivider,
                            label: $"{parts[i].LabelCap} [{partAccuracyPct}%]",
                            drawBackground: false,
                            doMouseoverSound: true,
                            textColor: ButtonTextColour
                        )
                    )
                    {
                        verb.RATS_Selection(target, parts[i], partAccuracy, shotReport);
                        Find.TickManager.CurTimeSpeed = TimeSpeed.Normal;
                        Close();
                    }
                }
            }
        }
    }

    protected virtual void Start() => Close();

    protected virtual void Cancel() => Close();

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

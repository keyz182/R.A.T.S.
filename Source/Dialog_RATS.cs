using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace RATS;

public class Dialog_RATS(Verb_AbilityRats verb, LocalTargetInfo target, IWindowDrawing customWindowDrawing = null)
    : Window(customWindowDrawing)
{
    private string title = "R.A.T.S.";

    private Verb_AbilityRats verb = verb;
    private LocalTargetInfo target = target;

    public override Vector2 InitialSize => new Vector2(845f, 740f);
    protected virtual Vector2 ButtonSize => new Vector2(200f, 40f);
    public virtual TaggedString OkButtonLabel => "OK".Translate();

    public virtual TaggedString CancelButtonLabel => "CancelButton".Translate();
    public virtual TaggedString WarningText => "";
    

    public virtual void DoHeader(ref RectDivider layout)
    {
        RectDivider headerLabelRow;
        using (new TextBlock(GameFont.Medium))
        {
            Vector2 vector2 = Text.CalcSize((string) this.title);
            headerLabelRow = layout.NewRow(vector2.y);
            Widgets.Label((Rect) headerLabelRow.NewCol(vector2.x), this.title);
        }
    }

    public virtual void DoButtonRow(ref RectDivider layout)
    {
        RectDivider rectDivider1 = layout.NewRow(this.ButtonSize.y, VerticalJustification.Bottom, new float?(0.0f));
        RectDivider rectDivider2 = rectDivider1.NewCol(this.ButtonSize.x, HorizontalJustification.Right, new float?(10f));
        RectDivider rectDivider3 = rectDivider1.NewCol(this.ButtonSize.x);
        RectDivider rectDivider4 = rectDivider1.NewCol(rectDivider1.Rect.width, HorizontalJustification.Right);
        
        if (Widgets.ButtonText((Rect) rectDivider3, (string) this.CancelButtonLabel))
            this.Cancel();
        var textBlock = new TextBlock(TextAnchor.MiddleRight, ColorLibrary.RedReadable);
        try
        {
            Widgets.Label((Rect) rectDivider4, this.WarningText);
        }
        finally
        {
            textBlock.Dispose();
        }
    }

    public virtual void DoRATS(ref RectDivider layout)
    {
        using (new ProfilerBlock(nameof (DoRATS)))
        {
            using (TextBlock.Default())
            {
                var parts = target.Pawn.health.hediffSet.GetNotMissingParts().Where(p=>p.def == BodyPartDefOf.Torso || p.parent?.def == BodyPartDefOf.Torso).ToList();

                var partCount = parts.Count();

                RectDivider rowBtn = layout.NewRow(600f, VerticalJustification.Top);

                RectDivider colLeft = rowBtn.NewCol(100f, HorizontalJustification.Left);
                RectDivider colMid = rowBtn.NewCol(InitialSize.x - 300f, HorizontalJustification.Left);
                RectDivider colRight = rowBtn.NewCol(100f, HorizontalJustification.Right);

                var portraitRect = colMid.Rect.ContractedBy(30f);
                RenderTexture texture = PortraitsCache.Get(target.Pawn, portraitRect.size, Rot4.South, new Vector3(0f, 0f, 0.1f), 1.5f, healthStateOverride: PawnHealthState.Mobile);
                
                GUI.DrawTexture(portraitRect, texture);

                for (var i = 0; i < partCount; i++)
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

                    if (Widgets.ButtonText((Rect)rectDivider, (string)parts[i].LabelCap, active: true))
                    {
                        verb.RATS_Selection(target, parts[i]);
                        this.Close();
                    }
                }    
            }
        }
    }
    protected virtual void Start() => this.Close();

    protected virtual void Cancel() => this.Close();
    
    public override void DoWindowContents(Rect inRect)
    {
        using (TextBlock.Default())
        {
            RectDivider layout1 = new RectDivider(inRect, 145235235);
            layout1.NewRow(0.0f, VerticalJustification.Bottom, 1f);
            this.DoHeader(ref layout1);
            layout1.NewRow(0.0f);
            this.DoRATS(ref layout1);
            this.DoButtonRow(ref layout1);
            layout1.NewRow(0.0f, marginOverride: new float?(20f));
            layout1.NewCol(20f, marginOverride: new float?(0.0f));
        }
    }
}
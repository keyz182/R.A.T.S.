using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RATS;

public class LegendaryStatDrawEntry : StatDrawEntry
{
    private LegendaryEffectDef LegendaryEffect;

    public LegendaryStatDrawEntry(
        LegendaryEffectDef legendaryEffectDef,
        StatCategoryDef category,
        StatDef stat,
        float value,
        StatRequest optionalReq,
        ToStringNumberSense numberSense = ToStringNumberSense.Undefined,
        int? overrideDisplayPriorityWithinCategory = null,
        bool forceUnfinalizedMode = false
    )
        : base(category, stat, value, optionalReq, numberSense, overrideDisplayPriorityWithinCategory, forceUnfinalizedMode)
    {
        LegendaryEffect = legendaryEffectDef;
    }

    public LegendaryStatDrawEntry(
        StatCategoryDef category,
        StatDef stat,
        float value,
        StatRequest optionalReq,
        ToStringNumberSense numberSense = ToStringNumberSense.Undefined,
        int? overrideDisplayPriorityWithinCategory = null,
        bool forceUnfinalizedMode = false
    )
        : base(category, stat, value, optionalReq, numberSense, overrideDisplayPriorityWithinCategory, forceUnfinalizedMode) { }

    public LegendaryStatDrawEntry(StatCategoryDef category, StatDef stat, string value)
        : base(category, stat, value) { }

    public LegendaryStatDrawEntry(
        StatCategoryDef category,
        string label,
        string valueString,
        string reportText,
        int displayPriorityWithinCategory,
        string overrideReportTitle = null,
        IEnumerable<Dialog_InfoCard.Hyperlink> hyperlinks = null,
        bool forceUnfinalizedMode = false,
        bool overridesHideStats = false
    )
        : base(category, label, valueString, reportText, displayPriorityWithinCategory, overrideReportTitle, hyperlinks, forceUnfinalizedMode, overridesHideStats) { }

    public LegendaryStatDrawEntry(StatCategoryDef category, StatDef stat)
        : base(category, stat) { }
}

using Verse;

namespace RATS;

public class HediffCompProperties_CripplePart : HediffCompProperties
{
    public DamageDef damageDef;

    public HediffCompProperties_CripplePart() => compClass = typeof(HediffComp_CripplePart);
}

using Verse;

namespace RATS;

public class Thing_Zoomer : ThingWithComps
{
    public int startTicks = -1;

    public override void PostMake()
    {
        base.PostMake();
        startTicks = Find.TickManager.TicksGame;
    }

    public override void Tick()
    {
        base.Tick();

        if (startTicks < 0)
            return;

        if (startTicks + RATSMod.Settings.ZoomTimeout < Find.TickManager.TicksGame)
            Destroy();
    }
}
